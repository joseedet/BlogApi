using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

//using Microsoft.EntityFrameworkCore.Sqlite;

namespace BlogApi.Data;

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
    ///     Posts del blog
    /// </summary>
    public DbSet<Post> Posts { get; set; }

    /// <summary>
    ///    Tags del blog
    /// </summary>
    public DbSet<Categoria> Categorias { get; set; }

    /// <summary>
    ///   Usuarios del blog
    /// </summary>
    public DbSet<Usuario> Usuarios { get; set; }

    /// <summary>
    ///   Comentarios del blog
    /// /// </summary>
    public DbSet<Comentario> Comentarios { get; set; }

    /// <summary>
    ///  Notificaciones del blog
    /// </summary>
    public DbSet<Notificacion> Notificaciones { get; set; }

    /// <summary>
    ///   Tags del blog
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    ///  Likes en publicaciones del blog
    /// </summary>
    public DbSet<LikePost> LikesPost { get; set; }

    /// <summary>
    ///  Likes en comentarios del blog
    /// </summary>
    public DbSet<LikeComentario> LikesComentario { get; set; }

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

        base.OnModelCreating(modelBuilder);
    }
}
