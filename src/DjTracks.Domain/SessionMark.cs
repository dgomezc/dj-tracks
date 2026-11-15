using System.Text.Json.Serialization;

namespace DjTracks.Domain;

public sealed record SessionMark([property: JsonRequired] string Line, [property: JsonRequired] double Seconds);
