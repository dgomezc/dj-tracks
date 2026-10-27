using FluentResults;

namespace DjTracks.Domain;

public enum ErrorKind { Validation, NotFound, Conflict, Unavailable, Unexpected }

public sealed class AppError(ErrorKind kind, string title, string detail,
    IDictionary<string, string[]>? fieldErrors = null) : Error(detail)
{
    public ErrorKind Kind { get; } = kind;
    public string Title { get; } = title;
    public IDictionary<string, string[]> FieldErrors { get; } = fieldErrors ?? new Dictionary<string, string[]>();
}
