using DjTracks.Api.Features.Streaming.GetTrackWaveform;
using DjTracks.Api.Features.Streaming.GetTrackArtwork;
using DjTracks.Api.Features.Streaming.StreamTrack;
using DjTracks.Api.Features.Streaming.StreamSession;
using FluentValidation;

namespace DjTracks.Api.Features.Streaming;

public sealed class StreamTrackRequestValidator : AbstractValidator<StreamTrackRequest>
{
    public StreamTrackRequestValidator()
    {
        RuleFor(request => request.Year).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Style).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El estilo debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.FileName).Must(StreamingRequestRules.BeAudioFileName)
            .WithMessage("El fichero debe ser un nombre de archivo MP3, no una ruta.");
    }
}

public sealed class GetTrackWaveformRequestValidator : AbstractValidator<GetTrackWaveformRequest>
{
    public GetTrackWaveformRequestValidator()
    {
        RuleFor(request => request.Year).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Style).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El estilo debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.FileName).Must(StreamingRequestRules.BeAudioFileName)
            .WithMessage("El fichero debe ser un nombre de archivo MP3, no una ruta.");
    }
}

public sealed class GetTrackArtworkRequestValidator : AbstractValidator<GetTrackArtworkRequest>
{
    public GetTrackArtworkRequestValidator()
    {
        RuleFor(request => request.Year).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Style).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El estilo debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.FileName).Must(StreamingRequestRules.BeAudioFileName)
            .WithMessage("El fichero debe ser un nombre de archivo MP3, no una ruta.");
    }
}

public sealed class StreamSessionRequestValidator : AbstractValidator<StreamSessionRequest>
{
    public StreamSessionRequestValidator()
    {
        RuleFor(request => request.Year).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("El año debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.Session).Must(StreamingRequestRules.BeFolderName)
            .WithMessage("La sesión debe ser un nombre de carpeta, no una ruta.");
        RuleFor(request => request.FileName).Must(StreamingRequestRules.BeAudioFileName)
            .WithMessage("El fichero debe ser un nombre de archivo MP3, no una ruta.");
    }
}

internal static class StreamingRequestRules
{
    public static bool BeFolderName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name);

    public static bool BeAudioFileName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name is not ("." or "..")
        && name.IndexOfAny(['/', '\\', ':']) < 0 && !Path.IsPathRooted(name)
        && Path.GetExtension(name).Equals(".mp3", StringComparison.OrdinalIgnoreCase);
}
