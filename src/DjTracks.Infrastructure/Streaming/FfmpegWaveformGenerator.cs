using System.Buffers.Binary;
using DjTracks.Domain;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace DjTracks.Infrastructure.Streaming;

public sealed class FfmpegWaveformGenerator
    : IWaveformGenerator
{
    private const int SampleRate = 44100;
    private const int Channels = 2;
    private const int BytesPerFrame = sizeof(short) * Channels;

    public FfmpegWaveformGenerator(string? binaryFolder)
    {
        if (!string.IsNullOrWhiteSpace(binaryFolder))
            GlobalFFOptions.Configure(options => options.BinaryFolder = binaryFolder);
    }

    public async Task<TrackWaveform> GenerateAsync(FileInfo file, int peakCount,
        CancellationToken cancellationToken)
    {
        using var audio = TagLib.File.Create(file.FullName);
        var duration = audio.Properties.Duration.TotalSeconds;
        if (!double.IsFinite(duration) || duration <= 0)
            throw new InvalidDataException("El MP3 no contiene una duración válida.");

        var sink = new PeakPipeSink(peakCount, (long)(duration * SampleRate));
        await FFMpegArguments.FromFileInput(file.FullName)
            .OutputToPipe(sink, options => options
                .ForceFormat("s16le")
                .WithCustomArgument($"-acodec pcm_s16le -ac {Channels} -ar {SampleRate}"))
            .CancellableThrough(cancellationToken)
            .ProcessAsynchronously();

        if (sink.FramesRead == 0)
            throw new InvalidDataException("FFmpeg no devolvió muestras de audio.");

        return new TrackWaveform(duration, sink.Peaks);
    }

    private sealed class PeakPipeSink(int peakCount, long expectedFrames) : IPipeSink
    {
        public float[] Peaks { get; } = new float[peakCount];
        public long FramesRead { get; private set; }

        public string GetFormat() => "s16le";

        public async Task ReadAsync(Stream inputStream, CancellationToken cancellationToken)
        {
            var buffer = new byte[64 * 1024 + BytesPerFrame];
            var bufferedBytes = 0;
            int read;
            while ((read = await inputStream.ReadAsync(
                buffer.AsMemory(bufferedBytes, buffer.Length - bufferedBytes), cancellationToken)) > 0)
            {
                var totalBytes = bufferedBytes + read;
                var completeBytes = totalBytes - totalBytes % BytesPerFrame;
                for (var offset = 0; offset < completeBytes; offset += BytesPerFrame)
                {
                    var left = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(offset, sizeof(short)));
                    var right = BinaryPrimitives.ReadInt16LittleEndian(
                        buffer.AsSpan(offset + sizeof(short), sizeof(short)));
                    var peak = Math.Max(Math.Abs((int)left), Math.Abs((int)right)) / 32768f;
                    var bucket = (int)Math.Min(Peaks.Length - 1,
                        FramesRead * Peaks.Length / expectedFrames);
                    Peaks[bucket] = Math.Max(Peaks[bucket], peak);
                    FramesRead++;
                }

                bufferedBytes = totalBytes - completeBytes;
                if (bufferedBytes > 0)
                    Buffer.BlockCopy(buffer, completeBytes, buffer, 0, bufferedBytes);
            }

            if (bufferedBytes != 0)
                throw new InvalidDataException("FFmpeg devolvió una trama PCM incompleta.");
        }
    }
}
