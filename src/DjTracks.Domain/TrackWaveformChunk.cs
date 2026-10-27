namespace DjTracks.Domain;

public sealed record TrackWaveformChunk(int StartIndex, float[] Peaks, int TotalPeakCount,
    double DurationSeconds, bool IsComplete, string? Error = null);
