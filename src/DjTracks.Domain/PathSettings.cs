using System.Text.Json.Serialization;

namespace DjTracks.Domain;

public sealed record PathSettings
{
    [JsonRequired]
    public string Tracks { get; set; } = "";

    [JsonRequired]
    public string Sessions { get; set; } = "";

    [JsonRequired]
    public string Remember { get; set; } = "";

    [JsonRequired]
    public string Playlists { get; set; } = "";

    [JsonRequired]
    public string M3uRoot { get; set; } = "";
}
