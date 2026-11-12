using DjTracks.Api.Features.Stats.RecordPlayback;
using DjTracks.Domain;
using FluentValidation;

namespace DjTracks.Api.Features.Stats;

public sealed class RecordPlaybackRequestValidator : AbstractValidator<RecordPlaybackRequest>
{
    public RecordPlaybackRequestValidator() =>
        RuleFor(request => request.Playback).Must(IsValidPlayback)
            .WithMessage("La referencia de reproducción no es válida.");

    private static bool IsValidPlayback(PlaybackRecord? playback)
    {
        if (playback is null || playback.EventId == Guid.Empty
            || string.IsNullOrEmpty(playback.FileName) || playback.FileName.Length > 255
            || !playback.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
            || playback.FileName.IndexOfAny(['/', '\\']) >= 0
            || playback.FileName.Any(char.IsControl)
            || playback.Title?.Length > 512
            || playback.Artists?.Length > 20
            || playback.Artists?.Any(artist => artist is null || artist.Length > 200 || artist.Any(char.IsControl)) == true)
            return false;

        static bool Segment(string? value, int maxLength) =>
            !string.IsNullOrWhiteSpace(value) && value.Length <= maxLength
            && value is not "." and not ".." && value.IndexOfAny(['/', '\\']) < 0
            && !value.Any(char.IsControl);

        return playback.Library switch
        {
            "Tracks" => Segment(playback.Year, 64) && Segment(playback.Style, 128)
                && playback.Category is null && playback.SessionFolder is null,
            "Sessions" => Segment(playback.Year, 64) && Segment(playback.SessionFolder, 255)
                && playback.Style is null && playback.Category is null,
            "Remember" => playback.Category is "Cortas" or "Largas"
                && playback.Year is null && playback.Style is null && playback.SessionFolder is null,
            _ => false
        };
    }
}
