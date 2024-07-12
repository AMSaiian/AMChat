using System.Security.Claims;
using AMChat.Common.Constants;

namespace AMChat.Middlewares;

public class MoqAuthenticationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CustomHeaders.UserIdHeader, out var userId))
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId!) };
            var identity = new ClaimsIdentity(claims, CustomHeaders.UserIdHeader);
            var principal = new ClaimsPrincipal(identity);

            context.User = principal;
        }

        await _next(context);
    }
}
