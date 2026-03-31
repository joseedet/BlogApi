using BlogApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

//using Microsoft.EntityFrameworkCore.Sqlite;

namespace BlogApi.Data;

/// <summary>
/// Clase del contexto
/// </summary>
public class BlogDbContext : DbContext
{
    /// <summary>
    ///   Constructor del contexto de la base de datos
    /// </summary>
    /// <param name="options"></param>
    ///// <returns></returns>
    /// <summary>
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options) { }

    /// <summary>
    /// Log de Acceso
    /// </summary>
    public DbSet<AccessLog> AccessLogs { get; set; }

    /// <summary>
    /// Banner
    /// </summary>
    public DbSet<Banner> Banners { get; set; }

    /// <summary>
    /// Configuración de caché para la aplicación, incluyendo tiempos de expiración para diferentes tipos de datos.
    /// </summary>
    public DbSet<CacheConfig> CacheConfig { get; set; }


    /// <summary>
    /// Tags del blog
    /// </summary>
    public DbSet<Categoria> Categorias { get; set; }

    /// <summary>
    /// Comentarios del blog
    /// </summary>
    public DbSet<Comentario> Comentarios { get; set; }

    /// <summary>
    /// Log de Email
    /// </summary>
    public DbSet<EmailLog> EmailLogs { get; set; }

    /// <summary>
    /// EmailSettings
    /// </summary>
    public DbSet<EmailSettings> EmailSettings { get; set; }

    /// <summary>
    /// Verificación de Email por tokens
    /// </summary>
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

    /// <summary>
    ///  Likes en comentarios del blog
    /// </summary>
    public DbSet<LikeComentario> LikesComentario { get; set; }

    /// <summary>
    ///  Likes en publicaciones del blog
    /// </summary>
    public DbSet<LikePost> LikesPost { get; set; }

    /// <summary>
    /// Log administrativo para acciones como bloqueos de usuarios, eliminación de contenido, etc.
    /// </summary>
    public DbSet<LogAdmin> LogAdmins { get; set; }

    /// <summary>
    /// Elementos del menú de la aplicación, que pueden incluir enlaces a diferentes secciones como publicaciones, categorías, autores, etc. Cada elemento del menú puede tener una jerarquía (padre-hijo) para organizar mejor la navegación.
    /// </summary>
    public DbSet<MenuItem> MenuItems { get; set; }    

    /// <summary>
    ///  Notificaciones del blog
    /// </summary>
    public DbSet<Notificacion> Notificaciones { get; set; }

    /// <summary>
    /// Ajustes de notificación
    /// </summary>
    public DbSet<NotificationSettings> NotificationSettings { get; set; }

    /// <summary>
    /// Pagina
    /// </summary>
    public DbSet<Page> Pages { get; set; }

    /// <summary>
    /// Versiones de página
    /// </summary>
    public DbSet<PageVersion> PageVersions { get; set; }

    /// <summary>
    /// Token para resetear la contraseña.
    /// </summary>
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    /// <summary>
    /// Permisos
    /// </summary>
    public DbSet<Permiso> Permisos { get; set; }

    /// <summary>
    /// Posts del blog
    /// </summary>
    public DbSet<Post> Posts { get; set; }

    /// <summary>
    /// Refresco del token
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    /// Roles
    /// </summary>
    public DbSet<Rol> Roles { get; set; }

    /// <summary>
    /// Rol Permisos
    /// </summary>
    public DbSet<RolPermiso> RolPermisos { get; set; }

    /// <summary>
    ///   Tags del blog
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    /// Preferencias del usuarios sobre notificaciones
    /// </summary>
    public DbSet<UserNotificationPreferences> UserNotificationPreferences { get; set; }

    /// <summary>
    ///   Usuarios del blog
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; }

    /// <summary>
    /// Usuario Roles
    /// </summary>
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }

    /// <summary>
    ///  Configuración de las relaciones entre entidades
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relación Post → Categoria (muchos a uno)
        modelBuilder
            .Entity<Post>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Post → Usuario (muchos a uno)
        modelBuilder
            .Entity<Post>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comentarios anidados

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.ComentarioPadre)
            .WithMany(c => c.Respuestas)
            .HasForeignKey(c => c.ComentarioPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity(j => j.ToTable("PostTags"));

        modelBuilder.Entity<Banner>().Property(b => b.Tipo).HasConversion<string>();

        modelBuilder
            .Entity<EmailSettings>()
            .HasData(
                new EmailSettings
                {
                    Id = 1,
                    Host = "",
                    Puerto = 587,
                    Usuario = "",
                    Password = "",
                    Remitente = "",
                    NombreRemitente = "",
                    UsarSSL = true,
                    Activo = false,
                }
            );

        modelBuilder
            .Entity<RefreshToken>()
            .HasOne(rt => rt.Usuario)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UsuarioId);

        modelBuilder
            .Entity<Notificacion>()
            .HasOne(n => n.UsuarioOrigen)
            .WithMany()
            .HasForeignKey(n => n.UsuarioOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Notificacion>()
            .HasOne(n => n.UsuarioDestino)
            .WithMany()
            .HasForeignKey(n => n.UsuarioDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<PageVersion>()
            .HasOne(v => v.Page)
            .WithMany()
            .HasForeignKey(v => v.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        //Permisos

        modelBuilder.Entity<UsuarioRol>().HasKey(ur => new { ur.UsuarioId, ur.RolId });

        modelBuilder
            .Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(ur => ur.UsuarioId);

        modelBuilder
            .Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(ur => ur.RolId);

        modelBuilder.Entity<RolPermiso>().HasKey(rp => new { rp.RolId, rp.PermisoId });

        modelBuilder
            .Entity<RolPermiso>()
            .HasOne(rp => rp.Rol)
            .WithMany(r => r.RolPermisos)
            .HasForeignKey(rp => rp.RolId);

        modelBuilder
            .Entity<RolPermiso>()
            .HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolPermisos)
            .HasForeignKey(rp => rp.PermisoId);

        modelBuilder
            .Entity<AccessLog>()
            .HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}
