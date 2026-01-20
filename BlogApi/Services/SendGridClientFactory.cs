using BlogApi.Services.Interfaces;
using SendGrid;

/// <summary>
/// Clase de servicio de SendGrid
/// </summary>
public class SendGridClientFactory : ISendGridClientFactory
{
    /// <summary>
    /// Creamos el cliente
    /// </summary>
    /// <param name="apiKey"></param>
    /// <returns></returns>
    public SendGridClient Create(string apiKey) => new SendGridClient(apiKey);
}
