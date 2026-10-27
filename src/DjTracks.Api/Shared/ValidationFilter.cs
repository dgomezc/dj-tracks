using FluentValidation;

namespace DjTracks.Api.Shared;

public static class ValidationFilter
{
    // Se ejecuta al construir cada endpoint del grupo /api: todo parámetro *Request debe tener validador.
    public static EndpointFilterDelegate Create(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var validators = context.MethodInfo.GetParameters()
            .Where(parameter => parameter.ParameterType.Name.EndsWith("Request", StringComparison.Ordinal))
            .Select(parameter => (parameter.Position, Validator: (IValidator)(context.ApplicationServices
                .GetService(typeof(IValidator<>).MakeGenericType(parameter.ParameterType))
                ?? throw new InvalidOperationException($"{parameter.ParameterType.Name} no tiene validador."))))
            .ToArray();
        if (validators.Length == 0)
            return next;

        return async invocation =>
        {
            foreach (var (position, validator) in validators)
            {
                var validation = await validator.ValidateAsync(
                    new ValidationContext<object>(invocation.Arguments[position]!),
                    invocation.HttpContext.RequestAborted);
                if (!validation.IsValid)
                    return Results.ValidationProblem(validation.ToDictionary(),
                        validation.Errors[0].ErrorMessage, title: "Revisa los datos enviados.");
            }
            return await next(invocation);
        };
    }
}
