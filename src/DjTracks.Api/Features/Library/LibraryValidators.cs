using DjTracks.Api.Features.Library.GetTracks;
using FluentValidation;

namespace DjTracks.Api.Features.Library;

public sealed class GetTracksRequestValidator : AbstractValidator<GetTracksRequest>
{
    private const string FolderNameMessage = "El año y el estilo deben ser nombres de carpeta, no rutas.";

    public GetTracksRequestValidator()
    {
        RuleFor(request => request.Year).NotNull().When(request => request.Style is not null)
            .WithMessage("Selecciona un año antes del estilo.");
        RuleFor(request => request.Year).Must(BeFolderName).When(request => request.Year is not null)
            .WithMessage(FolderNameMessage);
        RuleFor(request => request.Style).Must(BeFolderName).When(request => request.Style is not null)
            .WithMessage(FolderNameMessage);
    }

    private static bool BeFolderName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name);
}
