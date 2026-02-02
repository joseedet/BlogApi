using System;
using System.Security.Claims;
using BlogApi.Services.Interfaces;

namespace BlogApi.Services;

/// <summary>
/// Servicio de autorización
/// </summary>
public class AuthorizationService : IAuthorizationServiceBlog
{
    public bool EsAdmin(ClaimsPrincipal user) => user.IsInRole("Admin");

    public bool EsEditor(ClaimsPrincipal user) => user.IsInRole("Editor");

    public bool EsUsuario(ClaimsPrincipal user) => user.IsInRole("Usuario");

    public int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);

    public bool EsPropietario(int recursoUserId, ClaimsPrincipal user) =>
        recursoUserId == GetUserId(user);
}
