using DjTracks.Domain;

namespace DjTracks.Infrastructure.Streaming;

public interface IWaveformGenerator
{
    Task<TrackWaveform> GenerateAsync(FileInfo file, int peakCount, CancellationToken cancellationToken);
}
