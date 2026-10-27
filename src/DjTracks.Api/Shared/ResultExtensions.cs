using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Api.Shared;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        var error = result.Errors.OfType<AppError>().First();
        var status = error.Kind switch
        {
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Unavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
        return error.FieldErrors.Count > 0
            ? Results.ValidationProblem(error.FieldErrors, error.Message, title: error.Title, statusCode: status)
            : Results.Problem(error.Message, title: error.Title, statusCode: status);
    }
}
