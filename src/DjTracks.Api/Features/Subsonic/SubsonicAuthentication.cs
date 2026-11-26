using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace DjTracks.Api.Features.Subsonic;

public static class SubsonicAuthentication
{
    public static ValueTask<object?> FilterAsync(EndpointFilterInvocationContext invocation,
        EndpointFilterDelegate next)
    {
        var context = invocation.HttpContext;
        var query = context.Request.Query;
        var config = context.RequestServices.GetRequiredService<IConfiguration>();
        var username = config["Subsonic:Username"];
        var password = config["Subsonic:Password"];
        if (query.TryGetValue("f", out var format)
            && (format.Count != 1 || format[0] is not ("json" or "xml")))
            return Fail(0, "Formato de respuesta no admitido.");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Fail(0, "Subsonic no está configurado.");

        if (!Single(query["u"], out var user) || !Single(query["v"], out var version)
            || !Single(query["c"], out _))
            return Fail(10, "Falta un parámetro obligatorio.");
        if (!Version.TryParse(version, out var clientVersion))
            return Fail(10, "Versión de protocolo inválida.");
        if (clientVersion > new Version(1, 16, 1))
            return Fail(30, "El servidor no admite esta versión.");
        if (query.ContainsKey("p") || query.ContainsKey("apiKey"))
            return Fail(query.ContainsKey("t") || query.ContainsKey("s") ? 43 : 42,
                "Método de autenticación no admitido.");
        if (!Single(query["t"], out var token) || !Single(query["s"], out var salt))
            return Fail(10, "Falta el token o salt.");
        if (salt.Length > 512 || token.Length != 32 || token.Any(c => !Uri.IsHexDigit(c)))
            return Fail(40, "Credenciales incorrectas.");
        var expected = Convert.ToHexStringLower(MD5.HashData(Encoding.UTF8.GetBytes(password + salt)));
        var validToken = CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expected), Encoding.ASCII.GetBytes(token.ToLowerInvariant()));
        var validUser = CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(username)),
            SHA256.HashData(Encoding.UTF8.GetBytes(user)));
        return validToken && validUser ? next(invocation) : Fail(40, "Credenciales incorrectas.");

        ValueTask<object?> Fail(int code, string message) =>
            ValueTask.FromResult<object?>(SubsonicResponse.Error(context, code, message));
    }

    private static bool Single(StringValues values, out string value)
    {
        value = values.Count == 1 ? values[0] ?? "" : "";
        return !string.IsNullOrWhiteSpace(value);
    }
}
