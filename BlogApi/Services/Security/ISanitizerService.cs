using System;

namespace BlogApi.Services.Security;

/// <summary>
/// Servicio para sanitizar entradas de usuario y prevenir ataques XSS
/// </summary>
public interface ISanitizerService
{
    /// <summary>
    /// Sanitiza texto plano eliminando caracteres peligrosos
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Texto plano sanitizado</returns>
    string SanitizePlainText(string input);

    /// <summary>
    /// Sanitiza HTML eliminando etiquetas y atributos peligrosos
    /// </summary>
    /// <param name="input"></param>
    /// <returns>HTML sanitizado</returns>
    string SanitizeHtml(string input);

    /// <summary>
    /// Sanitiza contenido Markdown eliminando scripts y etiquetas peligrosas
    /// </summary>
    /// <param name="markdownInput"></param>
    /// <returns>Markdown sanitizado</returns>
    string SanitizeMarkdown(string markdownInput);
}
