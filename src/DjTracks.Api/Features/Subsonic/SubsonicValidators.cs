using FluentValidation;

namespace DjTracks.Api.Features.Subsonic;

public sealed record GetSubsonicDirectoryRequest(string? Id);
public sealed record GetSubsonicPlaylistRequest(string? Id);
public sealed record GetSubsonicAudioRequest(string? Id);

public sealed class GetSubsonicDirectoryRequestValidator : AbstractValidator<GetSubsonicDirectoryRequest>
{
    public GetSubsonicDirectoryRequestValidator() =>
        RuleFor(request => request.Id)
            .Must(value => SubsonicId.TryParse(value, out var id) && id.IsDirectory)
            .WithMessage("Carpeta no encontrada.");
}

public sealed class GetSubsonicPlaylistRequestValidator : AbstractValidator<GetSubsonicPlaylistRequest>
{
    public GetSubsonicPlaylistRequestValidator() =>
        RuleFor(request => request.Id)
            .Must(value => Guid.TryParse(value, out var id) && id != Guid.Empty)
            .WithMessage("Playlist no encontrada.");
}

public sealed class GetSubsonicAudioRequestValidator : AbstractValidator<GetSubsonicAudioRequest>
{
    public GetSubsonicAudioRequestValidator() =>
        RuleFor(request => request.Id)
            .Must(value => SubsonicId.TryParse(value, out var id) && id.IsSong)
            .WithMessage("Canción no encontrada.");
}
