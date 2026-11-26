using System.Text.Json;
using DjTracks.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace DjTracks.Api.Features.Subsonic;

public sealed record SubsonicId(string Library, string[] Parts)
{
    public bool IsSong => Library == "Remember" ? Parts.Length == 2 : Parts.Length == 3;
    public bool IsDirectory => !IsSong;

    public string Encode() =>
        WebEncoders.Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(
            new[] { Library }.Concat(Parts).ToArray()));

    public static bool TryParse(string? value, out SubsonicId id)
    {
        id = null!;
        if (string.IsNullOrEmpty(value) || value.Length > 4096)
            return false;
        try
        {
            var parts = JsonSerializer.Deserialize<string[]>(WebEncoders.Base64UrlDecode(value));
            if (parts is null || parts.Length is < 1 or > 4
                || parts[0] is not ("Tracks" or "Sessions" or "Remember")
                || parts.Skip(1).Any(part => !PlaylistTrackReference.IsSegment(part)))
                return false;

            var candidate = new SubsonicId(parts[0], parts[1..]);
            if (candidate.Library == "Remember"
                && (candidate.Parts.Length > 2
                    || (candidate.Parts.Length > 0 && candidate.Parts[0] is not ("Cortas" or "Largas"))))
                return false;
            if (candidate.IsSong && !candidate.Parts[^1].EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.Equals(candidate.Encode(), value, StringComparison.Ordinal))
                return false;
            id = candidate;
            return true;
        }
        catch (Exception ex) when (ex is FormatException or JsonException or ArgumentException)
        {
            return false;
        }
    }

    public SubsonicId Child(string name) => new(Library, [.. Parts, name]);
    public SubsonicId Parent() => new(Library, Parts[..^1]);
}
