using System;
using System.Security.Claims;
using BlogApi.Services.Interfaces;

namespace BlogApi.Middleware;

/// <summary>
/// Middleware para bloquear usuarios bloqueados
/// </summary>
public class UsuarioBloqueadoMiddleware
{
    private readonly RequestDelegate _next;
    /// <summary>
    /// Un middleware que verifica si el usuario autenticado está bloqueado. Si lo está, corta la request y devuelve un error 401. Si no, continúa con la request normal.
    /// </summary>
    /// <param name="next"></param>
    public UsuarioBloqueadoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Método que se ejecuta para cada request. Verifica si el usuario autenticado está
    /// bloqueado y actúa en consecuencia.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="usuarioService"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context, IUsuarioService usuarioService)
    {
        // Si el usuario NO está autenticado → continuar
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }
        // Obtener el ID del usuario desde el JWT
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            await _next(context);
            return;
        }
        int userId = int.Parse(userIdClaim.Value);
        // Consultar el usuario en la base de datos

        var usuario = await usuarioService.BuscarUsuarioPorIdAsync(userId);
        // Si el usuario existe y está bloqueado → cortar la request
        if (usuario != null && usuario.EstaBloqueado)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("El usuario está bloqueado.");
            return;
        }
        // Continuar con la request normal
        await _next(context);
    }
}
