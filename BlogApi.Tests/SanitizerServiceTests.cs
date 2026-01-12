using BlogApi.Services.Security;
using Xunit;

namespace BlogApi.Tests;

public class SanitizerServiceTests
{
    private readonly SanitizerService _sanitizer = new();

    // ------------------------------------------------------------
    // 1. TESTS DE SANITIZACIÓN HTML
    // ------------------------------------------------------------

    [Fact]
    public void SanitizeHtml_RemovesScriptTags()
    {
        var input = "<p>Hola</p><script>alert('xss')</script>";
        var result = _sanitizer.SanitizeHtml(input);

        Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Hola", result);
    }

    [Fact]
    public void SanitizeHtml_AllowsBasicFormatting()
    {
        var input = "<b>Negrita</b> <i>Cursiva</i>";
        var result = _sanitizer.SanitizeHtml(input);

        Assert.Contains("<b>", result);
        Assert.Contains("<i>", result);
    }

    [Fact]
    public void SanitizeHtml_RemovesOnErrorAttributes()
    {
        var input = "<img src='x' onerror='alert(1)'>";
        var result = _sanitizer.SanitizeHtml(input);

        Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
    }

    // ------------------------------------------------------------
    // 2. TESTS DE SANITIZACIÓN MARKDOWN
    // ------------------------------------------------------------

    [Fact]
    public void SanitizeMarkdown_ConvertsAndSanitizes()
    {
        var input = "# Título\n\n<script>alert('xss')</script>";
        var result = _sanitizer.SanitizeMarkdown(input);

        Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<h1>", result);
    }

    [Fact]
    public void SanitizeMarkdown_RemovesJavascriptLinks()
    {
        var input = "[haz clic](javascript:alert('xss'))";
        var result = _sanitizer.SanitizeMarkdown(input);

        Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SanitizeMarkdown_AllowsBasicMarkdown()
    {
        var input = "**negrita**";
        var result = _sanitizer.SanitizeMarkdown(input);

        Assert.Contains("<strong>", result);
    }

    // ------------------------------------------------------------
    // 3. TESTS DE SANITIZACIÓN DE TEXTO PLANO
    // ------------------------------------------------------------

    [Fact]
    public void SanitizePlainText_StripsHtmlTags()
    {
        var input = "<b>Hola</b> <i>mundo</i>";
        var result = _sanitizer.SanitizePlainText(input);

        Assert.Equal("Hola mundo", result.Trim());
    }

    [Fact]
    public void SanitizePlainText_RemovesScriptTags()
    {
        var input = "<script>alert('xss')</script>Hola";
        var result = _sanitizer.SanitizePlainText(input);

        Assert.Equal("Hola", result.Trim());
    }

    // ------------------------------------------------------------
    // 4. TESTS DE DETECCIÓN DE PATRONES PELIGROSOS
    // ------------------------------------------------------------

    [Fact]
    public void ContainsDangerousPattern_DetectsScript()
    {
        Assert.True(_sanitizer.ContainsDangerousPattern("<script>alert(1)</script>"));
    }

    [Fact]
    public void ContainsDangerousPattern_DetectsJavascriptProtocol()
    {
        Assert.True(_sanitizer.ContainsDangerousPattern("javascript:alert(1)"));
    }

    [Fact]
    public void ContainsDangerousPattern_DetectsOnError()
    {
        Assert.True(_sanitizer.ContainsDangerousPattern("<img src=x onerror=alert(1)>"));
    }

    [Fact]
    public void ContainsDangerousPattern_DetectsSvg()
    {
        Assert.True(_sanitizer.ContainsDangerousPattern("<svg onload=alert(1)>"));
    }

    [Fact]
    public void ContainsDangerousPattern_ReturnsFalseForSafeText()
    {
        Assert.False(_sanitizer.ContainsDangerousPattern("hola mundo"));
    }
}
