using BlogApi.Models;

namespace BlogApi.Data;

/// <summary>
/// Seeder
/// </summary>
public static class DbSeeder
{
    public static void Seed(BlogDbContext context)
    {
        SeedRoles(context);
        SeedAdmin(context);
    }

    /// <summary>
    /// Seeder usuario
    /// </summary>
    /// <param name="context"></param>
    private static void SeedAdmin(BlogDbContext context)
    {
        // Aquí puedes agregar datos iniciales a la base de datos si es necesario.
        if (!context.Usuarios.Any())
        {
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Email = "admin@blog.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                //Rol = RolUsuario.Administrador,
            };
            context.Usuarios.Add(admin);
            context.SaveChanges();

            // Asignar el rol Administrador (Id = 1)
            var adminRole = context.Roles.First(r => r.Nombre == "Administrador");
            context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = admin.Id, RolId = adminRole.Id });
            //context.UsuarioRoles.Add(adminRole);
            context.SaveChanges();
        }
    }

    /// <summary>
    /// /// Cargamos los roles
    /// </summary>
    /// <param name="context"></param>
    private static void SeedRoles(BlogDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = new List<Rol>
            {
                new Rol
                {
                    Nombre = "Administrador",
                    Descripcion = "Acceso completo al panel y configuración.",
                },
                new Rol
                {
                    Nombre = "Editor",
                    Descripcion = "Puede gestionar y publicar contenido, moderar comentarios.",
                },
                new Rol
                {
                    Nombre = "Autor",
                    Descripcion = "Puede crear y gestionar su propio contenido.",
                },
                new Rol
                {
                    Nombre = "Colaborador",
                    Descripcion = "Puede crear borradores, pero no publicar.",
                },
                new Rol
                {
                    Nombre = "Suscriptor",
                    Descripcion = "Acceso limitado, principalmente a su perfil.",
                },
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }

    private static void SeedPermisos(BlogDbContext context)
    {
        if (!context.Permisos.Any())
        {
            var permisos = new List<Permiso>
            {
                // Posts
                new Permiso { Clave = "Posts.Crear", Descripcion = "Crear nuevos posts" },
                new Permiso { Clave = "Posts.EditarPropios", Descripcion = "Editar posts propios" },
                new Permiso
                {
                    Clave = "Posts.EditarTodos",
                    Descripcion = "Editar posts de otros usuarios",
                },
                new Permiso
                {
                    Clave = "Posts.EliminarPropios",
                    Descripcion = "Eliminar posts propios",
                },
                new Permiso
                {
                    Clave = "Posts.EliminarTodos",
                    Descripcion = "Eliminar posts de otros usuarios",
                },
                new Permiso { Clave = "Posts.Publicar", Descripcion = "Publicar posts" },
                new Permiso { Clave = "Posts.Despublicar", Descripcion = "Despublicar posts" },
                // Categorías
                new Permiso { Clave = "Categorias.Crear", Descripcion = "Crear categorías" },
                new Permiso { Clave = "Categorias.Editar", Descripcion = "Editar categorías" },
                new Permiso { Clave = "Categorias.Eliminar", Descripcion = "Eliminar categorías" },
                // Tags
                new Permiso { Clave = "Tags.Crear", Descripcion = "Crear etiquetas" },
                new Permiso { Clave = "Tags.Editar", Descripcion = "Editar etiquetas" },
                new Permiso { Clave = "Tags.Eliminar", Descripcion = "Eliminar etiquetas" },
                // Usuarios
                new Permiso { Clave = "Usuarios.Ver", Descripcion = "Ver usuarios" },
                new Permiso { Clave = "Usuarios.Crear", Descripcion = "Crear usuarios" },
                new Permiso { Clave = "Usuarios.Editar", Descripcion = "Editar usuarios" },
                new Permiso { Clave = "Usuarios.Eliminar", Descripcion = "Eliminar usuarios" },
                new Permiso
                {
                    Clave = "Usuarios.AsignarRoles",
                    Descripcion = "Asignar roles a usuarios",
                },
                new Permiso { Clave = "Usuarios.Ver", Descripcion = "Ver listado de usuarios" },
                new Permiso
                {
                    Clave = "Usuarios.Bloquear",
                    Descripcion = "Bloquear o desbloquear usuarios",
                },
                new Permiso { Clave = "Usuarios.Ver", Descripcion = "Ver listado de usuarios" },

                // Comentarios
                new Permiso
                {
                    Clave = "Comentarios.Moderar",
                    Descripcion = "Aprobar, rechazar o marcar comentarios.",
                },
                new Permiso
                {
                    Clave = "Comentarios.Eliminar",
                    Descripcion = "Eliminar comentarios permanentemente.",
                },
                // Configuración
                new Permiso
                {
                    Clave = "Configuracion.Editar",
                    Descripcion = "Editar configuración del sistema",
                },
                // Banners
                new Permiso { Clave = "Banners.Crear", Descripcion = "Crear banners" },
                new Permiso { Clave = "Banners.Editar", Descripcion = "Editar banners" },
                new Permiso { Clave = "Banners.Eliminar", Descripcion = "Eliminar banners" },
                // Categorías
                new Permiso { Clave = "Categorias.Ver", Descripcion = "Ver categorías" },
                new Permiso { Clave = "Categorias.Crear", Descripcion = "Crear categorías" },
                new Permiso { Clave = "Categorias.Editar", Descripcion = "Editar categorías" },
                new Permiso { Clave = "Categorias.Eliminar", Descripcion = "Eliminar categorías" },
                // Email Logs
                new Permiso
                {
                    Clave = "EmailLogs.Ver",
                    Descripcion = "Ver registros de envío de emails",
                },
                // Email Settings
                new Permiso
                {
                    Clave = "EmailSettings.Ver",
                    Descripcion = "Ver configuración de email",
                },
                new Permiso
                {
                    Clave = "EmailSettings.Editar",
                    Descripcion = "Editar configuración de email",
                },
                new Permiso
                {
                    Clave = "EmailSettings.Test",
                    Descripcion = "Enviar email de prueba",
                },
                // Notification Settings
                new Permiso
                {
                    Clave = "NotificationSettings.Ver",
                    Descripcion = "Ver configuración global de notificaciones",
                },
                new Permiso
                {
                    Clave = "NotificationSettings.Editar",
                    Descripcion = "Editar configuración global de notificaciones",
                },
                // Páginas
                new Permiso
                {
                    Clave = "Paginas.Ver",
                    Descripcion = "Ver listado completo de páginas",
                },
                new Permiso { Clave = "Paginas.Crear", Descripcion = "Crear páginas" },
                new Permiso { Clave = "Paginas.Editar", Descripcion = "Editar páginas" },
                new Permiso { Clave = "Paginas.Eliminar", Descripcion = "Eliminar páginas" },
                new Permiso
                {
                    Clave = "Paginas.VerVersiones",
                    Descripcion = "Ver historial de versiones de páginas",
                },
                new Permiso
                {
                    Clave = "Paginas.RestaurarVersion",
                    Descripcion = "Restaurar versiones anteriores de páginas",
                },
                // Estadísticas
                new Permiso
                {
                    Clave = "Stats.Ver",
                    Descripcion = "Ver estadísticas generales del sistema",
                },
                new Permiso
                {
                    Clave = "Stats.VerActividad",
                    Descripcion = "Ver actividad reciente del sistema",
                },
                // Media
                new Permiso
                {
                    Clave = "Media.Subir",
                    Descripcion = "Subir imágenes o archivos multimedia",
                },
                // Tags
                new Permiso { Clave = "Tags.Ver", Descripcion = "Ver listado de tags" },
                new Permiso { Clave = "Tags.Crear", Descripcion = "Crear tags" },
                new Permiso { Clave = "Tags.Editar", Descripcion = "Editar tags" },
                new Permiso { Clave = "Tags.Eliminar", Descripcion = "Eliminar tags" },
            };

            context.Permisos.AddRange(permisos);
            context.SaveChanges();
        }
    }
}
