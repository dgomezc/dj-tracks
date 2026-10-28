namespace DjTracks.UI.Shared;

/// <summary>Temas de tweakcn definidos en wwwroot/theme.css; el elegido se guarda en una cookie.</summary>
public static class AppThemes
{
    public const string CookieName = "djt-theme";
    public const string Default = "light-green";

    public static readonly IReadOnlyList<(string Id, string Name)> All =
    [
        (Default, "Light Green"),
        ("radioactive", "Radioactive"),
        ("cyberpunk", "Cyberpunk"),
        ("agentica-node", "Agentica Node"),
        ("bugchud", "Bugchud!"),
        ("cyberpook", "Cyberpook"),
        ("eslinks", "ESLinks"),
    ];

    public static string Resolve(string? id) => All.Any(theme => theme.Id == id) ? id! : Default;
}
