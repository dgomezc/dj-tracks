using DjTracks.Api.Features.Library.GetTracks;
using DjTracks.Api.Features.Library.GetSessions;
using DjTracks.Api.Features.Library.GetFilteredTracks;
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

public sealed class GetSessionsRequestValidator : AbstractValidator<GetSessionsRequest>
{
    public GetSessionsRequestValidator()
    {
        RuleFor(request => request.Year).NotNull().When(request => request.Session is not null)
            .WithMessage("Selecciona un año antes de la sesión.");
        RuleFor(request => request.Year).Must(BeFolderName).When(request => request.Year is not null)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Session).Must(BeFolderName).When(request => request.Session is not null)
            .WithMessage("La sesión debe ser un nombre de carpeta, no una ruta.");
    }

    private static bool BeFolderName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name);
}

public sealed class GetFilteredTracksRequestValidator : AbstractValidator<GetFilteredTracksRequest>
{
    public GetFilteredTracksRequestValidator()
    {
        RuleForEach(request => request.Year).Must(BeFolderName)
            .When(request => request.Year is not null)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleForEach(request => request.Group).Must(BeFolderName)
            .When(request => request.Group is not null)
            .WithMessage("El grupo debe ser un nombre de carpeta, no una ruta.");
        RuleForEach(request => request.Substyle).Must(BeSubstyle)
            .When(request => request.Substyle is not null)
            .WithMessage("El subestilo debe ser Instrumental o Vocal.");
    }

    private static bool BeSubstyle(string? value) =>
        value is not null && (value.Equals("Instrumental", StringComparison.OrdinalIgnoreCase)
            || value.Equals("Vocal", StringComparison.OrdinalIgnoreCase));

    private static bool BeFolderName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name);
}
