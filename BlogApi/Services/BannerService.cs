using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// clase de servicio de Banner
/// </summary>
public class BannerService : IBannerService
{
    private readonly BlogDbContext _db;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Constructor de Banner
    /// </summary>
    /// <param name="db"></param>
    /// <param name="env"></param>
    public BannerService(BlogDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // ------------------------------------------------------------
    // Crear Banner
    // ------------------------------------------------------------

    /// <summary>
    /// Creación del Banner
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>BannerDto</returns>
    public async Task<BannerDto> CrearAsync(BannerCreateDto dto)
    {
        var imagenUrl = await GuardarImagenAsync(dto.ImagenFile);

        var banner = new Banner
        {
            Titulo = dto.Titulo,
            Subtitulo = dto.Subtitulo,
            ImagenUrl = imagenUrl,
            Enlace = dto.Enlace,
            Activo = dto.Activo,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Orden = dto.Orden,
            Alt = dto.Alt,
            AbrirEnNuevaPestana = dto.AbrirEnNuevaPestana,
            Descripcion = dto.Descripcion,
            Tipo = dto.Tipo,
        };

        _db.Banners.Add(banner);
        await _db.SaveChangesAsync();

        return ToDto(banner);
    }

    // ------------------------------------------------------------
    // Obtener por ID
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>BannerDto</returns>
    public async Task<BannerDto?> ObtenerPorIdAsync(int id)
    {
        var banner = await _db.Banners.FindAsync(id);
        return banner == null ? null : ToDto(banner);
    }

    // ------------------------------------------------------------
    // Obtener todos
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener todos los banners
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    public async Task<IEnumerable<BannerDto>> ObtenerTodosAsync()
    {
        return await _db.Banners.OrderBy(b => b.Orden).Select(b => ToDto(b)).ToListAsync();
    }

    // ------------------------------------------------------------
    // Obtener solo activos (y dentro de fecha)
    // ------------------------------------------------------------

    /// <summary>
    /// Obtener banners activos
    /// </summary>
    /// <returns>IEnumerable BannerDto</returns>
    public async Task<IEnumerable<BannerDto>> ObtenerActivosAsync()
    {
        var ahora = DateTime.UtcNow;

        return await _db
            .Banners.Where(b =>
                b.Activo
                && (b.FechaInicio == null || b.FechaInicio <= ahora)
                && (b.FechaFin == null || b.FechaFin >= ahora)
            )
            .OrderBy(b => b.Orden)
            .Select(b => ToDto(b))
            .ToListAsync();
    }

    // ------------------------------------------------------------
    // Actualizar Banner
    // ------------------------------------------------------------

    /// <summary>
    /// Actualización del Banner
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>BannerDto</returns>
    public async Task<BannerDto?> ActualizarAsync(int id, BannerUpdateDto dto)
    {
        var banner = await _db.Banners.FindAsync(id);
        if (banner == null)
            return null;

        banner.Titulo = dto.Titulo;
        banner.Subtitulo = dto.Subtitulo;
        banner.Enlace = dto.Enlace;
        banner.Activo = dto.Activo;
        banner.FechaInicio = dto.FechaInicio;
        banner.FechaFin = dto.FechaFin;
        banner.Orden = dto.Orden;
        banner.Alt = dto.Alt;
        banner.AbrirEnNuevaPestana = dto.AbrirEnNuevaPestana;
        banner.Descripcion = dto.Descripcion;
        banner.Tipo = dto.Tipo;

        // Si se sube una nueva imagen, reemplazar
        if (dto.ImagenFile != null)
        {
            banner.ImagenUrl = await GuardarImagenAsync(dto.ImagenFile);
        }

        await _db.SaveChangesAsync();

        return ToDto(banner);
    }

    // ------------------------------------------------------------
    // Eliminar Banner
    // ------------------------------------------------------------

    /// <summary>
    /// Elimina un banner
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Verdadero si se ha eliminado en caso contrario falso</returns>
    public async Task<bool> EliminarAsync(int id)
    {
        var banner = await _db.Banners.FindAsync(id);
        if (banner == null)
            return false;

        _db.Banners.Remove(banner);
        await _db.SaveChangesAsync();
        return true;
    }

    // ------------------------------------------------------------
    // Guarder imagen en /uploads/banners/
    // ------------------------------------------------------------

    private async Task<string> GuardarImagenAsync(IFormFile file)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads", "banners");
        Directory.CreateDirectory(folder);

        var nombre = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var ruta = Path.Combine(folder, nombre);

        using (var stream = new FileStream(ruta, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/banners/{nombre}";
    }

    // ------------------------------------------------------------
    // Conversion a DTO
    // ------------------------------------------------------------
    private static BannerDto ToDto(Banner b) =>
        new()
        {
            Id = b.Id,
            Titulo = b.Titulo,
            Subtitulo = b.Subtitulo,
            ImagenUrl = b.ImagenUrl,
            Enlace = b.Enlace,
            Activo = b.Activo,
            FechaInicio = b.FechaInicio,
            FechaFin = b.FechaFin,
            Orden = b.Orden,
            Alt = b.Alt,
            AbrirEnNuevaPestana = b.AbrirEnNuevaPestana,
            Descripcion = b.Descripcion,
            Tipo = b.Tipo,
        };
}
