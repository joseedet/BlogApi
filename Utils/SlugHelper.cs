using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlogApi.Utils;

public class SlugHelper
{
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
