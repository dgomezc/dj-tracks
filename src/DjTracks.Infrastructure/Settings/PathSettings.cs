namespace DjTracks.Infrastructure.Settings;

public sealed class PathSettings
{
    public string Tracks { get; set; } = "";
    public string Sessions { get; set; } = "";
    public string Remember { get; set; } = "";
    public string Data { get; set; } = "";

    public string Playlists => Path.Combine(Data, "playlists");
    public string SessionMarks => Path.Combine(Data, "session-marks");
    public string TrackWaves => Path.Combine(Data, "tracks-waves");

    public void InitializeDataDirectories()
    {
        // No crear la raíz: podría ser un montaje desconectado.
        using (var entries = Directory.EnumerateFileSystemEntries(Data).GetEnumerator())
            entries.MoveNext();

        foreach (var directory in new[] { Playlists, SessionMarks, TrackWaves })
        {
            Directory.CreateDirectory(directory);
            using var probe = new FileStream(Path.Combine(directory, $".djtracks-{Guid.NewGuid():N}.tmp"),
                FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose);
        }
    }
}
