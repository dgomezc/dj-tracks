using DjTracks.Domain;

namespace DjTracks.UI.Shared;

public sealed class AudioPlayerState
{
    private readonly List<QueuedTrack> _queue = [];
    private readonly List<Guid> _order = [];

    public IReadOnlyList<QueuedTrack> Queue => _queue.AsReadOnly();
    public QueuedTrack? Current { get; private set; }
    public string? AudioUrl => Current?.AudioUrl;
    public string? WaveformUrl => Current?.WaveformUrl;
    public string? Title => Current?.Title;
    public string? Artist => Current?.Artist;
    public long PlaybackVersion { get; private set; }
    public bool Shuffle { get; private set; }
    public QueueRepeatMode Repeat { get; private set; }
    public bool CanPrevious => CanMove(-1);
    public bool CanNext => CanMove(1);

    public event Action? Changed;

    public void Play(string audioUrl, string waveformUrl, string artworkUrl, TrackFile details)
    {
        var track = _queue.FirstOrDefault(item => item.AudioUrl == audioUrl)
            ?? AddTrack(audioUrl, waveformUrl, artworkUrl, details);
        Select(track);
    }

    public void Enqueue(string audioUrl, string waveformUrl, string artworkUrl, TrackFile details)
    {
        if (_queue.Any(item => item.AudioUrl == audioUrl))
            return;

        AddTrack(audioUrl, waveformUrl, artworkUrl, details);
        Changed?.Invoke();
    }

    public void PlayQueued(Guid id)
    {
        var track = _queue.Single(item => item.Id == id);
        Select(track);
    }

    public void Remove(Guid id)
    {
        var index = _order.IndexOf(id);
        var removingCurrent = Current?.Id == id;
        _queue.RemoveAll(item => item.Id == id);
        _order.Remove(id);
        if (removingCurrent)
        {
            Current = null;
            if (_order.Count > 0)
                Current = _queue.Single(item => item.Id == _order[Math.Min(index, _order.Count - 1)]);
            PlaybackVersion++;
        }
        Changed?.Invoke();
    }

    public void Clear()
    {
        _queue.Clear();
        _order.Clear();
        Current = null;
        PlaybackVersion++;
        Changed?.Invoke();
    }

    public void Previous() => Move(-1);
    public void Next() => Move(1);

    public void OnEnded(long playbackVersion)
    {
        // Descarta eventos atrasados del elemento de audio de la pista anterior.
        if (playbackVersion != PlaybackVersion || Current is null)
            return;
        if (Repeat == QueueRepeatMode.One)
            Select(Current);
        else
            Move(1);
    }

    public void ToggleShuffle()
    {
        Shuffle = !Shuffle;
        _order.Clear();
        _order.AddRange(_queue.Select(item => item.Id));
        if (Shuffle)
        {
            var shuffled = _order.ToArray();
            Random.Shared.Shuffle(shuffled);
            _order.Clear();
            _order.AddRange(shuffled);
            if (Current is not null)
            {
                _order.Remove(Current.Id);
                _order.Insert(0, Current.Id);
            }
        }
        Changed?.Invoke();
    }

    public void CycleRepeat()
    {
        Repeat = Repeat switch
        {
            QueueRepeatMode.Off => QueueRepeatMode.All,
            QueueRepeatMode.All => QueueRepeatMode.One,
            _ => QueueRepeatMode.Off
        };
        Changed?.Invoke();
    }

    private QueuedTrack AddTrack(string audioUrl, string waveformUrl, string artworkUrl, TrackFile details)
    {
        var track = new QueuedTrack(Guid.NewGuid(), audioUrl, waveformUrl, artworkUrl, details);
        _queue.Add(track);
        _order.Add(track.Id);
        return track;
    }

    private bool CanMove(int offset)
    {
        if (Current is null)
            return _order.Count > 0 && offset > 0;
        var index = _order.IndexOf(Current.Id) + offset;
        return index >= 0 && index < _order.Count
            || Repeat == QueueRepeatMode.All && _order.Count > 0;
    }

    private void Move(int offset)
    {
        if (!CanMove(offset))
            return;
        var index = Current is null ? 0 : _order.IndexOf(Current.Id) + offset;
        index = (index + _order.Count) % _order.Count;
        Select(_queue.Single(item => item.Id == _order[index]));
    }

    private void Select(QueuedTrack track)
    {
        Current = track;
        PlaybackVersion++;
        Changed?.Invoke();
    }
}
