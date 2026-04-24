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
    /// <summary>
    /// Servicio para obtener la estructura del menú. Se inyecta a través del constructor del controlador. El servicio se encarga de la lógica de negocio para construir el menú, que puede incluir la consulta a la base de datos, la aplicación de reglas de negocio y la transformación de datos en el formato adecuado para la respuesta.
    /// El método GetMenuAsync del servicio devuelve la estructura del menú de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. El controlador expone un endpoint GET /api/menu que devuelve la estructura del menú en formato JSON, lo que facilita su consumo por parte de clientes frontend o aplicaciones móviles.
    /// </summary>
    private readonly IMenuService _menuService;

    /// <summary>
    /// Constructor de menu, que recibe una instancia del servicio de menú a través de inyección de dependencias. Esto permite que el controlador utilice el servicio para obtener la estructura del menú sin acoplarse a una implementación específica, lo que facilita la mantenibilidad y la testabilidad del código.
    /// El constructor asigna la instancia del servicio a un campo privado, que luego se utiliza en el método GetMenu para obtener la estructura del menú de manera asíncrona. Al utilizar  la inyección de dependencias, se promueve la separación de responsabilidades y se facilita la reutilización del servicio en otros controladores o componentes de la aplicación. Además, esto permite que el servicio sea fácilmente mockeable en pruebas unitarias, lo que mejora la calidad del código y la confiabilidad de la aplicación.
    /// </summary>
    /// <param name="menuService"></param>
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }
    /// <summary>
    /// Obtiene la estructura del menú de la aplicación. Este método maneja las solicitudes GET a /api/menu y devuelve la estructura del menú en formato JSON. El método utiliza el servicio IMenuService para obtener los datos del menú de manera asíncrona, lo que permite manejar solicitudes de manera eficiente sin bloquear el hilo de ejecución. La estructura del menú puede incluir enlaces a diferentes secciones de la aplicación, como publicaciones, categorías, autores, etc. El método devuelve un resultado Ok con la estructura del menú, lo que facilita su consumo por parte de clientes frontend o aplicaciones móviles. En caso de que ocurra un error al obtener la estructura del menú, el método puede devolver un resultado de error adecuado, como InternalServerError, para informar al cliente sobre el problema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetMenu()
    {
        var menu = await _menuService.GetMenuAsync();
        return Ok(menu);
    }
}
