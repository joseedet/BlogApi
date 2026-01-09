using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Data;

public static class DbSeeder
{
    public static void SeedAdmin(BlogDbContext context)
    {
        // Aquí puedes agregar datos iniciales a la base de datos si es necesario.
        if (!context.Usuarios.Any())
        {
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Email = "admin@blog.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Rol = RolUsuario.Administrador,
            };
            context.Usuarios.Add(admin);
            context.SaveChanges();
        }
    }
}
