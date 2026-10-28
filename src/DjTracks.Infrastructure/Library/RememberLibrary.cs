using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Infrastructure.Library;

public sealed class RememberLibrary
{
    public Result<RememberListing> Browse(string root, string category, CancellationToken ct)
    {
        var directory = ResolveCategory(root, category);
        if (directory.IsFailed)
            return directory.ToResult();

        return new RememberListing(category, TrackLibrary.ReadTracks(directory.Value, ct)
            .Select(file => file with { Year = null, Style = null }).ToArray());
    }

    public Result<FileInfo> ResolveAudioFile(string root, string category, string fileName,
        CancellationToken ct)
    {
        var directory = ResolveCategory(root, category);
        return directory.IsFailed
            ? directory.ToResult()
            : TrackLibrary.ResolveAudioFile(directory.Value, fileName, ct);
    }

    public Result<FileStream> OpenAudio(string root, string category, string fileName, CancellationToken ct)
    {
        var file = ResolveAudioFile(root, category, fileName, ct);
        return file.IsFailed ? file.ToResult() : new FileStream(file.Value.FullName, FileMode.Open,
            FileAccess.Read, FileShare.Read, 64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    public Result<TrackArtwork> ReadArtwork(string root, string category, string fileName, CancellationToken ct)
    {
        var file = ResolveAudioFile(root, category, fileName, ct);
        return file.IsFailed ? file.ToResult() : TrackLibrary.ReadArtwork(file.Value, ct);
    }

    private static Result<DirectoryInfo> ResolveCategory(string root, string category)
    {
        var directory = TrackLibrary.FindChild(new DirectoryInfo(root), category);
        return directory is null
            ? new AppError(ErrorKind.NotFound, "Carpeta no encontrada.",
                "La carpeta de Remember seleccionada no existe. Comprueba que la raíz contiene Cortas y Largas.")
            : directory;
    }
}
