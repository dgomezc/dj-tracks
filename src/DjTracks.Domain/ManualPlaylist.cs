namespace DjTracks.Domain;

public sealed record ManualPlaylist(Guid Id, string Name, PlaylistEntry[] Entries,
    AutomaticPlaylistFilters? Filters = null);

public sealed record AutomaticPlaylistFilters(string[] Years, string[] Styles, bool IncludeRemember)
{
    public bool IsValid() =>
        Years is not null && Styles is not null
        && Years.All(PlaylistTrackReference.IsSegment) && Styles.All(PlaylistTrackReference.IsSegment);

    public AutomaticPlaylistFilters Normalize() => new(
        Years.Distinct(StringComparer.OrdinalIgnoreCase).OrderDescending(StringComparer.OrdinalIgnoreCase).ToArray(),
        Styles.Select(CanonicalStyle).Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase).ToArray(), IncludeRemember);

    public static string CanonicalStyle(string style) =>
        style.Equals("Night Intrumental", StringComparison.OrdinalIgnoreCase) ? "Night Instrumental" : style;
}

public sealed record AutomaticPlaylistOptions(string[] Years, string[] Styles);

public sealed record PlaylistEntry(Guid Id, PlaylistTrackReference Track);

public sealed record PlaylistTrackReference(string Library, string FileName,
    string? Year = null, string? Style = null, string? Category = null)
{
    public bool IsValid() =>
        IsSegment(FileName) && FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
        && (Library == "Tracks" && IsSegment(Year) && IsSegment(Style) && Category is null
            || Library == "Remember" && Category is "Cortas" or "Largas" && Year is null && Style is null);

    public static bool IsSegment(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value is not ("." or "..")
        && value.IndexOfAny(['/', '\\', ':', '\0']) < 0 && !value.Any(char.IsControl);
}

public sealed record ResolvedPlaylistTrack(PlaylistTrackReference Reference, TrackFile Details);

public sealed record PlaylistPlayback(ResolvedPlaylistTrack[] Tracks);
