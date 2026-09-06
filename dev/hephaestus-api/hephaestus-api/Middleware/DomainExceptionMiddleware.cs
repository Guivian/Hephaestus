using Hephaestus.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hephaestus.Api.Middleware;

public sealed class DomainExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (DomainException exception)
        {
            context.Response.StatusCode = exception.StatusCode;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = exception.StatusCode,
                Title = "Não foi possível concluir o pedido.",
                Detail = exception.Message
            });
        }
    }
}
