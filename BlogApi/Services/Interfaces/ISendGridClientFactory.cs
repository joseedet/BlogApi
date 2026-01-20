using System;
using SendGrid;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Interfaz de creacion 
/// </summary>
public interface ISendGridClientFactory
{
    /// <summary>
    /// Creacion de envio de prueba
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    SendGridClient Create(string apiKey);
}
