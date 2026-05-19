using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Mapper;

/// <summary>
/// Clase estática que contiene métodos de mapeo entre la entidad MensajeContacto y sus DTOs correspondientes (MensajeContactoDto y MensajeContactoCrearDto).
/// </summary>
public static class MensajeContactoMapper
{
    /// <summary>
    /// Convierte un DTO de creación de mensaje de contacto en una entidad de mensaje de contacto.
    /// </summary>
    /// <param name="dto">El DTO de creación de mensaje de contacto.</param>
    /// <param name="ip">La dirección IP del cliente (opcional).</param>
    /// <returns>La entidad de mensaje de contacto.</returns>
    public static MensajeContacto ToModel(this MensajeContactoCrearDto dto, string? ip = null)
    {
        return new MensajeContacto
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Asunto = dto.Asunto,
            Mensaje = dto.Mensaje,
            DireccionIp = ip,
        };
    }
    /// <summary>
    /// Convierte una entidad de mensaje de contacto en su DTO correspondiente.
    /// </summary>
    /// <param name="model">La entidad de mensaje de contacto.</param>
    /// <returns>El DTO de mensaje de contacto.</returns>
    public static MensajeContactoDto ToDto(this MensajeContacto model)
    {
        return new MensajeContactoDto
        {
            Id = model.Id,
            Nombre = model.Nombre,
            Email = model.Email,
            Asunto = model.Asunto,
            Mensaje = model.Mensaje,
            FechaCreacion = model.FechaCreacion,
        };
    }
}
