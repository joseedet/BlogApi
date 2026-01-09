using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogApi.Data;

/// <summary>
/// Factoría para crear instancias del contexto de base de datos en tiempo de diseño
/// </summary>
public class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    /// <summary>
    /// Crea una instancia del contexto de la base de datos para tiempo de diseño
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public BlogDbContext CreateDbContext(string[] args)
    {
        // Cargar configuración manualmente
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            // ← aquí ocurre la magia
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new BlogDbContext(optionsBuilder.Options);
    }
}
