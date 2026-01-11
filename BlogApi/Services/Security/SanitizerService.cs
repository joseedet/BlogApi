using Ganss.Xss;
using Markdig;

namespace BlogApi.Services.Security;

/// <summary>
/// Servicio para sanitizar entradas de usuario y prevenir ataques XSS
/// </summary>
public class SanitizerService : ISanitizerService
{
    private readonly HtmlSanitizer _htmlSanitizer;
    private readonly MarkdownPipeline _markdownPipeline;

    /// <summary>
    /// Constructor de SanitizerService
    /// </summary>
    public SanitizerService()
    { // Whitelist de HTML permitido
        _htmlSanitizer = new HtmlSanitizer();
        // Limpiamos todo y luego añadimos solo lo que queremos permitir
        _htmlSanitizer.AllowedTags.Clear();
        _htmlSanitizer.AllowedAttributes.Clear();
        // Ejemplo de whitelist (ajustable según tus necesidades):
        _htmlSanitizer.AllowedTags.Add("b");
        _htmlSanitizer.AllowedTags.Add("strong");
        _htmlSanitizer.AllowedTags.Add("i");
        _htmlSanitizer.AllowedTags.Add("em");
        _htmlSanitizer.AllowedTags.Add("u");
        _htmlSanitizer.AllowedTags.Add("p");
        _htmlSanitizer.AllowedTags.Add("br");
        _htmlSanitizer.AllowedTags.Add("ul");
        _htmlSanitizer.AllowedTags.Add("ol");
        _htmlSanitizer.AllowedTags.Add("li");
        _htmlSanitizer.AllowedTags.Add("h1");
        _htmlSanitizer.AllowedTags.Add("h2");
        _htmlSanitizer.AllowedTags.Add("h3");
        _htmlSanitizer.AllowedTags.Add("h4");
        _htmlSanitizer.AllowedTags.Add("blockquote");
        _htmlSanitizer.AllowedTags.Add("code");
        _htmlSanitizer.AllowedTags.Add("pre");
        _htmlSanitizer.AllowedTags.Add("a");
        _htmlSanitizer.AllowedAttributes.Add("href");
        _htmlSanitizer.AllowedAttributes.Add("title");
        // Evitar URLs peligrosas tipo javascript:
        _htmlSanitizer.AllowedSchemes.Add("http");
        _htmlSanitizer.AllowedSchemes.Add("https");
        _htmlSanitizer.AllowedSchemes.Add("mailto");
        // Pipeline de Markdown -> HTML
        _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    /// <summary>
    /// Sanitiza texto plano eliminando caracteres peligrosos
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Texto plano sanitizado</returns>
    public string SanitizePlainText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        // Para texto plano, simplemente quitamos cualquier HTML
        var sanitizedHtml = _htmlSanitizer.Sanitize(input);
        return StripHtmlTags(sanitizedHtml);
    }

    /// <summary>
    /// Sanitiza HTML eliminando etiquetas y atributos peligrosos
    /// </summary>
    /// <param name="input"></param>
    /// <returns>HTML sanitizado</returns>
    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;
        return _htmlSanitizer.Sanitize(input);
    }

    /// <summary>
    /// Sanitiza contenido Markdown eliminando scripts y etiquetas peligrosas
    /// </summary>
    /// <param name="markdownInput"></param>
    /// <returns>Markdown sanitizado</returns>
    public string SanitizeMarkdown(string markdownInput)
    {
        if (string.IsNullOrWhiteSpace(markdownInput))
            return string.Empty;
        var html = Markdig.Markdown.ToHtml(markdownInput, _markdownPipeline);
        var sanitizedHtml = _htmlSanitizer.Sanitize(html);
        return sanitizedHtml;
    }

    private string StripHtmlTags(string input)
    {
        // Implementación sencilla para quitar etiquetas HTML
        var array = new char[input.Length];
        var arrayIndex = 0;
        var inside = false;
        foreach (var @let in input)
        {
            if (@let == '<')
            {
                inside = true;
                continue;
            }
            if (@let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex] = @let;
                arrayIndex++;
            }
        }
        return new string(array, 0, arrayIndex);
    }
}
