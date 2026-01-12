using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Middleware;

/// <summary>
/// Extensiones para el middleware de cabeceras de seguridad
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de cabeceras de seguridad HTTP al pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <returns>IApplicationBuilder</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
