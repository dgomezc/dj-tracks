namespace DjTracks.UI.Shared;

public sealed class AudioPlayerState
{
    public string? AudioUrl { get; private set; }
    public string? Title { get; private set; }
    public string? Artist { get; private set; }

    public event Action? Changed;

    public void Play(string audioUrl, string title, string? artist)
    {
        AudioUrl = audioUrl;
        Title = title;
        Artist = artist;
        Changed?.Invoke();
    }
}
