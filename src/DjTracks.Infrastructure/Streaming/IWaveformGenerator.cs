using DjTracks.Domain;

namespace DjTracks.Infrastructure.Streaming;

public interface IWaveformGenerator
{
    Task<TrackWaveform> GenerateAsync(FileInfo file, int peakCount,
        Func<TrackWaveformChunk, CancellationToken, ValueTask>? onChunk,
        CancellationToken cancellationToken);
}
