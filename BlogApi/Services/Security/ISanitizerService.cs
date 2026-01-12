using System;

namespace BlogApi.Services.Interfaces;

/// <summary>
/// Servicio para sanitizar entradas de usuario y prevenir ataques XSS
/// </summary>
public interface ISanitizerService
{
    /// <summary>
    /// Sanitiza texto plano (títulos, inputs sin HTML)
    /// </summary>
    string SanitizePlainText(string input);

    /// <summary>
    /// Sanitiza contenido Markdown o HTML permitido
    /// </summary>
    string SanitizeMarkdown(string input);

    /// <summary>
    /// Detecta patrones peligrosos (XSS)
    /// </summary>
    bool ContainsDangerousPattern(string input);
}
