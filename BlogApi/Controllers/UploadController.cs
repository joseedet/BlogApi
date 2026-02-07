using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

/// <summary>
/// Controlador para subir archivos (imágenes) al servidor
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    [Authorize(Policy = "Permiso:Media.Subir")]
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Archivo vacío");
        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(ext))
            return BadRequest("Formato no permitido");
        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine("wwwroot/uploads", fileName);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new { url });
    }
}
