using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador de EmailLogs
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class EmailLogsController : ControllerBase
{
    private readonly IEmailLogService _service;

    /// <summary>
    /// Constructor de EmailLogs
    /// </summary>
    /// <param name="service"></param>
    public EmailLogsController(IEmailLogService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtenemos los ultimos logs
    /// </summary>
    /// <param name="cantidad"></param>
    /// <returns></returns>
    [HttpGet("{cantidad:int}")]
    public async Task<IActionResult> Obtener(int cantidad = 50)
    {
        var logs = await _service.ObtenerUltimosAsync(cantidad);
        return Ok(logs);
    }
}
