using BlogApi.Services.Security;
using Xunit;

namespace BlogApi.Tests;

public class SanitizerServiceTests
{
    private readonly SanitizerService _sanitizer = new();

    [Fact]
    public void SanitizeHtml_RemovesScriptTags()
    {
        var input = "<p>Hola</p><script>alert('xss')</script>";
        var result = _sanitizer.SanitizeHtml(input);

        Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Hola", result);
    }

    [Fact]
    public void SanitizeMarkdown_ConvertsAndSanitizes()
    {
        var input = "# Título\n\n<script>alert('xss')</script>";
        var result = _sanitizer.SanitizeMarkdown(input);

        Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<h1>", result);
    }

    [Fact]
    public void SanitizePlainText_StripsHtmlTags()
    {
        var input = "<b>Hola</b> <i>mundo</i>";
        var result = _sanitizer.SanitizePlainText(input);

        Assert.Equal("Hola mundo", result.Trim());
    }

    [Fact]
    public void SanitizeHtml_AllowsBasicFormatting()
    {
        var input = "<b>Negrita</b> <i>Cursiva</i>";
        var result = _sanitizer.SanitizeHtml(input);

        Assert.Contains("<b>", result);
        Assert.Contains("<i>", result);
    }
}

