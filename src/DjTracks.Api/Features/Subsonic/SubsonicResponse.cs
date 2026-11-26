using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using DjTracks.Domain;
using FluentResults;

namespace DjTracks.Api.Features.Subsonic;

public static class SubsonicResponse
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static IResult Ok(HttpContext context, string? name = null, object? value = null) =>
        Write(context, "ok", name, value);

    public static IResult Error(HttpContext context, int code, string message) =>
        Write(context, "failed", "error", new { code, message });

    public static IResult FromError(HttpContext context, ResultBase result)
    {
        var error = result.Errors.FirstOrDefault();
        return Error(context, error is AppError { Kind: ErrorKind.NotFound } ? 70 : 0,
            error?.Message ?? "La operación no se pudo completar.");
    }

    private static IResult Write(HttpContext context, string status, string? name, object? value)
    {
        context.Response.Headers.CacheControl = "no-store";
        var response = new Dictionary<string, object> { ["status"] = status, ["version"] = "1.16.1" };
        if (name is not null && value is not null)
            response[name] = value;
        if (context.Request.Query["f"] == "json")
            return Results.Json(new Dictionary<string, object> { ["subsonic-response"] = response }, Options);

        XNamespace ns = "http://subsonic.org/restapi";
        var root = new XElement(ns + "subsonic-response");
        AddProperties(root, JsonSerializer.SerializeToElement(response, Options), ns);
        return Results.Text(new XDocument(root).ToString(), "text/xml", Encoding.UTF8);
    }

    private static void AddProperties(XElement parent, JsonElement value, XNamespace ns)
    {
        foreach (var property in value.EnumerateObject())
        {
            var item = property.Value;
            if (item.ValueKind == JsonValueKind.Null)
                continue;
            if (item.ValueKind == JsonValueKind.Array)
            {
                foreach (var child in item.EnumerateArray())
                    AddElement(parent, property.Name, child, ns);
            }
            else if (item.ValueKind == JsonValueKind.Object)
                AddElement(parent, property.Name, item, ns);
            else
                parent.Add(new XAttribute(property.Name, Scalar(item)));
        }
    }

    private static void AddElement(XElement parent, string name, JsonElement value, XNamespace ns)
    {
        var element = new XElement(ns + name);
        if (value.ValueKind == JsonValueKind.Object)
            AddProperties(element, value, ns);
        else
            element.Value = Scalar(value);
        parent.Add(element);
    }

    private static string Scalar(JsonElement value) =>
        value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText();
}
