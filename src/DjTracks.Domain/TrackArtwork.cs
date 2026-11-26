namespace DjTracks.Domain;

public sealed record TrackArtwork(string? DataUrl)
{
    public bool TryGetImage(out byte[] bytes, out string contentType)
    {
        bytes = [];
        contentType = "";
        if (DataUrl is null)
            return false;
        var separator = DataUrl.IndexOf(";base64,", StringComparison.Ordinal);
        if (!DataUrl.StartsWith("data:", StringComparison.Ordinal) || separator < 5)
            return false;
        var type = DataUrl[5..separator];
        if (type is not ("image/jpeg" or "image/png" or "image/gif" or "image/webp"))
            return false;
        try
        {
            bytes = Convert.FromBase64String(DataUrl[(separator + 8)..]);
            contentType = type;
            return bytes.Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
