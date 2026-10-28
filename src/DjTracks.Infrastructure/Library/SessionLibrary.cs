using System.Text;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Infrastructure.Library;

public sealed class SessionLibrary(TrackLibrary tracks)
{
    public Result<SessionListing> Browse(string root, string? year, string? session, CancellationToken ct)
    {
        // Ambas bibliotecas tienen dos niveles de carpetas antes de los MP3.
        var result = tracks.Browse(root, year, session, ct);
        if (result.IsFailed)
            return result.ToResult();

        var listing = result.Value;
        var folders = listing.Folders;
        if (year is not null && session is null)
            Array.Reverse(folders);

        string[] tracklists = [];
        if (year is not null && session is not null)
        {
            var yearDirectory = TrackLibrary.FindChild(new DirectoryInfo(root), year);
            var directory = yearDirectory is null ? null : TrackLibrary.FindChild(yearDirectory, session);
            if (directory is null)
                return new AppError(ErrorKind.NotFound, "Carpeta no encontrada.",
                    "La carpeta seleccionada ya no existe. Vuelve al nivel anterior.");

            tracklists = FindTracklists(directory, session, ct).Select(file => file.Name).ToArray();
        }

        return new SessionListing(year, session, folders,
            listing.Tracks.Select(file => file with { Style = null }).ToArray(), tracklists);
    }

    public Result<FileStream> OpenAudio(string root, string year, string session, string fileName,
        CancellationToken ct) => tracks.OpenTrack(root, year, session, fileName, ct);

    public Result<FileInfo> ResolveAudioFile(string root, string year, string session, string fileName,
        CancellationToken ct) => tracks.ResolveTrackFile(root, year, session, fileName, ct);

    public Result<TrackArtwork> ReadArtwork(string root, string year, string session, string fileName,
        CancellationToken ct) => tracks.ReadArtwork(root, year, session, fileName, ct);

    public async Task<Result<SessionTracklist>> ReadTracklistAsync(string root, string year,
        string session, string fileName, CancellationToken ct)
    {
        var audio = ResolveAudioFile(root, year, session, fileName, ct);
        if (audio.IsFailed)
            return audio.ToResult();

        var file = FindTracklists(audio.Value.Directory!, session, ct).FirstOrDefault();
        if (file is null)
            return new SessionTracklist(null, null);

        await using var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read,
            FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var reader = new StreamReader(stream, new UTF8Encoding(false, true),
            detectEncodingFromByteOrderMarks: true);
        return new SessionTracklist(file.Name, await reader.ReadToEndAsync(ct));
    }

    private static FileInfo[] FindTracklists(DirectoryInfo directory, string session, CancellationToken ct)
    {
        var files = directory.EnumerateFiles()
            .Where(file => (file.Attributes & FileAttributes.ReparsePoint) == 0).ToArray();
        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase) { "Tracklist" };
        foreach (var name in files.Where(file => file.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            .Select(file => Path.GetFileNameWithoutExtension(file.Name)).Append(session))
        {
            ct.ThrowIfCancellationRequested();
            names.Add(name);
            names.Add($"Tracklist - {name}");
            names.Add($"{name} - Tracklist");
        }

        return files.Where(file => file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                && names.Contains(Path.GetFileNameWithoutExtension(file.Name)))
            .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(file => file.Name, StringComparer.Ordinal).ToArray();
    }
}
