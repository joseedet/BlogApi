using System.Text;
using System.Text.RegularExpressions;

namespace BlogApi.Utils;

/// <summary>
/// Helper para generar slugs a partir de textos
/// </summary>
public class SlugHelper
{
    /// <summary>
    /// Genera un slug a partir de un texto
    /// </summary>
    /// <param name="text">Texto de entrada</param>
    /// <returns>Slug generado</returns>
    public static string GenerateSlug(string text)
    {
        text = text.ToLowerInvariant().Trim();
        // Quitar acentos
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
        text = Encoding.ASCII.GetString(bytes);
        // Reemplazar caracteres no válidos
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        //Reemplazar espacios por guiones
        text = Regex.Replace(text, @"\s+", "-").Trim('-');
        // Evitar guiones repetidos
        text = Regex.Replace(text, @"-+", "-");
        return text;
    }
}
