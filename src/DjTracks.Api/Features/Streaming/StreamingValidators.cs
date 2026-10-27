using DjTracks.Api.Features.Streaming.StreamTrack;
using FluentValidation;

namespace DjTracks.Api.Features.Streaming;

public sealed class StreamTrackRequestValidator : AbstractValidator<StreamTrackRequest>
{
    public StreamTrackRequestValidator()
    {
        RuleFor(request => request.Year).Must(BeFolderName)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Style).Must(BeFolderName)
            .WithMessage("El estilo debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.FileName).Must(BeAudioFileName)
            .WithMessage("El fichero debe ser un nombre de archivo MP3, no una ruta.");
    }

    private static bool BeFolderName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name);

    private static bool BeAudioFileName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name)
        && Path.GetExtension(name).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
}
