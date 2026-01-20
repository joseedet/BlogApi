using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Tests.Services.Email;

public class EmailSettingsServiceTests
{
    private BlogDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BlogDbContext(options);
    }

    // ------------------------------------------------------------
    // Caso 1: ObtenerAsync crea registro si no existe
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerAsync_ShouldCreateDefaultSettings_WhenNoneExist()
    {
        using var db = CreateDbContext();
        var service = new EmailSettingsService(db);

        var result = await service.ObtenerAsync();

        Assert.NotNull(result);
        Assert.Equal(587, result.Puerto); // valor por defecto
        Assert.True(db.EmailSettings.Any());
    }

    // ------------------------------------------------------------
    // Caso 2: ObtenerAsync devuelve configuración existente
    // ------------------------------------------------------------
    [Fact]
    public async Task ObtenerAsync_ShouldReturnExistingSettings()
    {
        using var db = CreateDbContext();

        db.EmailSettings.Add(
            new EmailSettings
            {
                Host = "smtp.test.com",
                Puerto = 25,
                Usuario = "user",
                Password = "pass",
                Remitente = "admin@test.com",
                NombreRemitente = "Admin",
                UsarSSL = false,
                Activo = true,
            }
        );

        await db.SaveChangesAsync();

        var service = new EmailSettingsService(db);

        var result = await service.ObtenerAsync();

        Assert.Equal("smtp.test.com", result.Host);
        Assert.Equal(25, result.Puerto);
        Assert.False(result.UsarSSL);
    }

    // ------------------------------------------------------------
    // Caso 3: ActualizarAsync modifica la configuración existente
    // ------------------------------------------------------------
    [Fact]
    public async Task ActualizarAsync_ShouldUpdateSettings()
    {
        using var db = CreateDbContext();

        db.EmailSettings.Add(
            new EmailSettings
            {
                Host = "old.com",
                Puerto = 25,
                Usuario = "old",
                Password = "oldpass",
                Remitente = "old@test.com",
                NombreRemitente = "Old",
                UsarSSL = false,
                Activo = false,
            }
        );

        await db.SaveChangesAsync();

        var service = new EmailSettingsService(db);

        var updateDto = new EmailSettingsUpdateDto
        {
            Host = "smtp.new.com",
            Puerto = 587,
            Usuario = "newuser",
            Password = "newpass",
            Remitente = "new@test.com",
            NombreRemitente = "Nuevo",
            UsarSSL = true,
            Activo = true,
        };

        var result = await service.ActualizarAsync(updateDto);

        var entity = await db.EmailSettings.FirstAsync();

        Assert.Equal("smtp.new.com", entity.Host);
        Assert.Equal(587, entity.Puerto);
        Assert.Equal("newuser", entity.Usuario);
        Assert.Equal("newpass", entity.Password);
        Assert.Equal("new@test.com", entity.Remitente);
        Assert.Equal("Nuevo", entity.NombreRemitente);
        Assert.True(entity.UsarSSL);
        Assert.True(entity.Activo);
    }

    // ------------------------------------------------------------
    // Caso 4: ActualizarAsync crea registro si no existe
    // ------------------------------------------------------------
    [Fact]
    public async Task ActualizarAsync_ShouldCreateSettings_WhenNoneExist()
    {
        using var db = CreateDbContext();
        var service = new EmailSettingsService(db);

        var dto = new EmailSettingsUpdateDto
        {
            Host = "smtp.test.com",
            Puerto = 587,
            Usuario = "user",
            Password = "pass",
            Remitente = "admin@test.com",
            NombreRemitente = "Admin",
            UsarSSL = true,
            Activo = true,
        };

        await service.ActualizarAsync(dto);

        var entity = await db.EmailSettings.FirstAsync();

        Assert.Equal("smtp.test.com", entity.Host);
        Assert.Equal("user", entity.Usuario);
        Assert.Equal("pass", entity.Password);
        Assert.True(entity.Activo);
    }
}
