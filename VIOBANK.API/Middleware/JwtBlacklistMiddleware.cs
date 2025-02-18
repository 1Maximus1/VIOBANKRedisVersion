using VIOBANK.RedisPersistence.Services;

namespace VIOBANK.API.Middleware
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtBlacklistService _jwtBlacklistService;

        public JwtBlacklistMiddleware(RequestDelegate next, JwtBlacklistService jwtBlacklistService)
        {
            _next = next;
            _jwtBlacklistService = jwtBlacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/auth/login"))
            {
                await _next(context);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/auth/register"))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Cookies["tasty-cookies"];

            if (!string.IsNullOrEmpty(token) && await _jwtBlacklistService.IsBlacklistedAsync(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Token has been blacklisted.");
                return;
            }

            await _next(context);
        }
    }
}
