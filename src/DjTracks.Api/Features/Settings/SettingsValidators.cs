using System.Linq.Expressions;
using DjTracks.Api.Features.Settings.SaveSettings;
using FluentValidation;

namespace DjTracks.Api.Features.Settings;

public sealed class SaveSettingsRequestValidator : AbstractValidator<SaveSettingsRequest>
{
    public SaveSettingsRequestValidator()
    {
        PathRule(request => request.Tracks);
        PathRule(request => request.Sessions);
        PathRule(request => request.Remember);
        PathRule(request => request.Playlists);
        PathRule(request => request.M3uRoot);
    }

    private void PathRule(Expression<Func<SaveSettingsRequest, string?>> path) =>
        RuleFor(path).Cascade(CascadeMode.Stop)
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Introduce una ruta.")
            .Must(BeAbsolutePath)
            .WithMessage("Introduce una ruta absoluta del servidor (local o de red).");

    private static bool BeAbsolutePath(string? path) => Path.IsPathFullyQualified(path!.Trim());
}
