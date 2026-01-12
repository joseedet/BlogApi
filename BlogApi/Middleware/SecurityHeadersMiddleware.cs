using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Middleware;

/// <summary>
/// Middleware para agregar cabeceras de seguridad HTTP
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public SecurityHeadersMiddleware(
        RequestDelegate next,
        ILogger<SecurityHeadersMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    { // Protección básica XSS en navegadores antiguos
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        // Evitar content-type sniffing
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        // Evitar que se embeba en iframes
        context.Response.Headers["X-Frame-Options"] = "DENY";
        // Content Security Policy (ajústala según tu frontend)
        context.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self';";
        await _next(context);
    }
}
