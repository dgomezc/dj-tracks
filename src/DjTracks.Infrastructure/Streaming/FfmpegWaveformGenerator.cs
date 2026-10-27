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
        Func<TrackWaveformChunk, CancellationToken, ValueTask>? onChunk,
        CancellationToken cancellationToken)
    {
        using var audio = TagLib.File.Create(file.FullName);
        var duration = audio.Properties.Duration.TotalSeconds;
        if (!double.IsFinite(duration) || duration <= 0)
            throw new InvalidDataException("El MP3 no contiene una duración válida.");

        var sink = new PeakPipeSink(peakCount, Math.Max(1, (long)(duration * SampleRate)),
            duration, onChunk);
        await FFMpegArguments.FromFileInput(file.FullName)
            .OutputToPipe(sink, options => options
                .ForceFormat("s16le")
                .WithCustomArgument($"-acodec pcm_s16le -ac {Channels} -ar {SampleRate}"))
            .CancellableThrough(cancellationToken)
            .ProcessAsynchronously();

        if (sink.FramesRead == 0)
            throw new InvalidDataException("FFmpeg no devolvió muestras de audio.");

        await sink.FlushRemainingAsync(cancellationToken);
        return new TrackWaveform(duration, sink.Peaks);
    }

    private sealed class PeakPipeSink(int peakCount, long expectedFrames, double durationSeconds,
        Func<TrackWaveformChunk, CancellationToken, ValueTask>? onChunk) : IPipeSink
    {
        private const int ChunkSize = 16;
        private int _nextPeak;

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

                var completedBuckets = (int)Math.Min(Peaks.Length,
                    FramesRead * Peaks.Length / expectedFrames);
                await EmitCompletedChunksAsync(completedBuckets, cancellationToken);

                bufferedBytes = totalBytes - completeBytes;
                if (bufferedBytes > 0)
                    Buffer.BlockCopy(buffer, completeBytes, buffer, 0, bufferedBytes);
            }

            if (bufferedBytes != 0)
                throw new InvalidDataException("FFmpeg devolvió una trama PCM incompleta.");
        }

        public ValueTask FlushRemainingAsync(CancellationToken cancellationToken) =>
            EmitChunkAsync(Peaks.Length, cancellationToken);

        private async ValueTask EmitCompletedChunksAsync(int completedBuckets,
            CancellationToken cancellationToken)
        {
            while (completedBuckets - _nextPeak >= ChunkSize)
                await EmitChunkAsync(_nextPeak + ChunkSize, cancellationToken);
        }

        private async ValueTask EmitChunkAsync(int endIndex, CancellationToken cancellationToken)
        {
            if (onChunk is null || endIndex <= _nextPeak)
                return;

            var startIndex = _nextPeak;
            var peaks = Peaks[startIndex..endIndex];
            _nextPeak = endIndex;
            await onChunk(new TrackWaveformChunk(startIndex, peaks, Peaks.Length,
                durationSeconds, IsComplete: false), cancellationToken);
        }
    }
}
