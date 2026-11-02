using DjTracks.Api.Features.Playlists.AddPlaylistTrack;
using DjTracks.Api.Features.Playlists.CreatePlaylist;
using DjTracks.Api.Features.Playlists.DeletePlaylist;
using DjTracks.Api.Features.Playlists.GetPlaylist;
using DjTracks.Api.Features.Playlists.LoadPlaylist;
using DjTracks.Api.Features.Playlists.RemovePlaylistTrack;
using DjTracks.Api.Features.Playlists.RenamePlaylist;
using DjTracks.Infrastructure.Playlists;
using FluentValidation;

namespace DjTracks.Api.Features.Playlists;

public sealed class CreatePlaylistRequestValidator : AbstractValidator<CreatePlaylistRequest>
{
    public CreatePlaylistRequestValidator() =>
        RuleFor(request => request.Name).Must(JsonPlaylistStore.IsValidName)
            .WithMessage("Escribe un nombre de 1 a 120 caracteres, sin caracteres de control.");
}

public sealed class RenamePlaylistRequestValidator : AbstractValidator<RenamePlaylistRequest>
{
    public RenamePlaylistRequestValidator() =>
        RuleFor(request => request.Name).Must(JsonPlaylistStore.IsValidName)
            .WithMessage("Escribe un nombre de 1 a 120 caracteres, sin caracteres de control.");
}

public sealed class AddPlaylistTrackRequestValidator : AbstractValidator<AddPlaylistTrackRequest>
{
    public AddPlaylistTrackRequestValidator() =>
        RuleFor(request => request.Track).Must(track => track is not null && track.IsValid())
            .WithMessage("Selecciona un MP3 de Tracks (año/estilo) o Remember (Cortas/Largas), sin rutas. Sessions no se admite.");
}

public sealed class GetPlaylistRequestValidator : AbstractValidator<GetPlaylistRequest>
{
    public GetPlaylistRequestValidator() =>
        RuleFor(request => request.Id).NotEmpty().WithMessage("Selecciona una playlist válida.");
}

public sealed class DeletePlaylistRequestValidator : AbstractValidator<DeletePlaylistRequest>
{
    public DeletePlaylistRequestValidator() =>
        RuleFor(request => request.Id).NotEmpty().WithMessage("Selecciona una playlist válida.");
}

public sealed class LoadPlaylistRequestValidator : AbstractValidator<LoadPlaylistRequest>
{
    public LoadPlaylistRequestValidator() =>
        RuleFor(request => request.Id).NotEmpty().WithMessage("Selecciona una playlist válida.");
}

public sealed class RemovePlaylistTrackRequestValidator : AbstractValidator<RemovePlaylistTrackRequest>
{
    public RemovePlaylistTrackRequestValidator()
    {
        RuleFor(request => request.Id).NotEmpty().WithMessage("Selecciona una playlist válida.");
        RuleFor(request => request.EntryId).NotEmpty().WithMessage("Selecciona una entrada válida.");
    }
}
