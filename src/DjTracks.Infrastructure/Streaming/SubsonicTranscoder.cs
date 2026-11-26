using FFMpegCore;
using FFMpegCore.Pipes;

namespace DjTracks.Infrastructure.Streaming;

public sealed class SubsonicTranscoder
{
    public SubsonicTranscoder(string? binaryFolder)
    {
        if (!string.IsNullOrWhiteSpace(binaryFolder))
            GlobalFFOptions.Configure(options => options.BinaryFolder = binaryFolder);
    }

    public Task TranscodeAsync(Stream input, Stream output, string codec, string format,
        int? bitrate, CancellationToken ct) =>
        FFMpegArguments.FromPipeInput(new StreamPipeSource(input))
            .OutputToPipe(new StreamPipeSink(output), options =>
            {
                options.WithAudioCodec(codec).ForceFormat(format);
                if (bitrate is > 0)
                    options.WithAudioBitrate(bitrate.Value);
            })
            .CancellableThrough(ct)
            .ProcessAsynchronously();
}
