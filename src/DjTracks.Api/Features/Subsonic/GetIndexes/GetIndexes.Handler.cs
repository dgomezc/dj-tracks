using DjTracks.Api.Features.Subsonic.GetMusicDirectory;
using DjTracks.Api.Features.Subsonic.GetMusicFolders;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic.GetIndexes;

public sealed class GetIndexesHandler(GetMusicDirectoryHandler directory, GetMusicFoldersHandler folders)
{
    public Result<SubsonicIndexes> Handle(string? folderId, CancellationToken ct)
    {
        var libraries = folders.Handle();
        if (libraries.IsFailed)
            return libraries.ToResult();
        if (!string.IsNullOrEmpty(folderId)
            && !libraries.Value.Any(folder => folder.Id == folderId))
            return new AppError(ErrorKind.NotFound, "Biblioteca no encontrada.", "La biblioteca seleccionada no existe.");

        List<SubsonicFolder> items = [];
        foreach (var folder in libraries.Value.Where(folder => folderId is null || folder.Id == folderId))
        {
            SubsonicId.TryParse(folder.Id, out var root);
            var result = directory.Handle(root, ct);
            if (result.IsFailed)
                return result.ToResult();
            items.AddRange(result.Value.Child.Where(child => child.IsDir)
                .Select(child => new SubsonicFolder(child.Id, child.Title)));
        }
        var groups = items.GroupBy(item => item.Name[..1].ToUpperInvariant())
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .Select(group => new SubsonicIndex(group.Key, group.ToArray())).ToArray();
        return new SubsonicIndexes(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "", groups);
    }
}

public sealed record SubsonicIndexes(long LastModified, string IgnoredArticles, SubsonicIndex[] Index);
public sealed record SubsonicIndex(string Name, SubsonicFolder[] Artist);
