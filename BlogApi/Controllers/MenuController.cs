using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Services;
using BlogApi.Services.Interfaces;

namespace BlogApi.Controllers;
/// <summary>
/// Controlador para manejar las solicitudes relacionadas con el menú de la aplicación. Proporciona un endpoint para obtener la estructura del menú, que puede incluir enlaces a diferentes secciones de la aplicación, como publicaciones, categorías, autores, etc. El controlador utiliza el servicio IMenuService para obtener los datos del menú de manera asíncrona.
/// </summary>
[ApiController]
[Route("api/[controller]")]

public class MenuController : ControllerBase
{
     private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMenu()
    {
        var menu = await _menuService.GetMenuAsync();
        return Ok(menu);
    }
}
