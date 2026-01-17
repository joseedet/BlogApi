using System.Security.Claims;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogApi.Tests.Common;

public abstract class ControllerTestBase
{
    protected readonly Mock<IPostService> PostService = new();
    protected readonly Mock<INotificacionService> Notificaciones = new();

    /// <summary>
    /// Crea un controller del tipo T inyectando los servicios mockeados.
    /// </summary>
    protected T CreateController<T>()
        where T : ControllerBase
    {
        return (T)Activator.CreateInstance(typeof(T), PostService.Object, Notificaciones.Object)!;
    }

    /// <summary>
    /// Crea un usuario simulado con ID y rol.
    /// </summary>
    protected ClaimsPrincipal FakeUser(int id = 123, string role = "Autor")
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Role, role),
                },
                "TestAuth"
            )
        );
    }

    /// <summary>
    /// Asigna un usuario simulado al HttpContext del controller.
    /// </summary>
    protected void SetUser(ControllerBase controller, ClaimsPrincipal user)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user },
        };
    }
}
