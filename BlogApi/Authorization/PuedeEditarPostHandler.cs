using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApi.Data;
using Microsoft.AspNetCore.Authorization;

namespace BlogApi.Authorization;

/// <summary>
/// Manejador de autorización para editar un post
/// </summary>
public class PuedeEditarPostHandler : AuthorizationHandler<PuedeEditarPostRequirement>
{
    private readonly BlogDbContext _context;

    /// <summary>
    /// Constructor de PuedeEditarPostHandler
    /// </summary>
    /// <param name="context"></param>
    public PuedeEditarPostHandler(BlogDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Maneja el requisito de autorización para editar un post
    /// </summary>
    /// <param name="context"></param>
    /// <param name="requirement"></param>
    /// /// <returns>Task</returns>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PuedeEditarPostRequirement requirement
    )
    {
        var rol = context.User.FindFirst("rol")?.Value;
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (rol == "Administrador" || rol == "Editor")
        {
            context.Succeed(requirement);
            return;
        }
        if (rol == "Autor" && userId != null)
        {
            context.Succeed(requirement);
            return;
        }
    }
}
