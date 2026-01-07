using System.Security.Claims;
using System.Text;
using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Repositories;
using BlogApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/// <summary>
///   Punto de entrada de la aplicación
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
///   Configuración de servicios y dependencias
/// </summary>
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

/// Inyección de dependencias para repositorios y servicios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

/// Repositorios y Servicios
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<IPostRepository, PostRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

///... Other repository and service registrations
builder.Services.AddScoped<IPostService, PostService>();

///... Other repository and service registrations
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

///... Other repository and service registrations
builder.Services.AddScoped<IComentarioService, ComentarioService>();

///... Other repository and service registrations
builder.Services.AddScoped<ITokenService, TokenService>();

///... Other repository and service registrations
builder.Services.AddScoped<ITagRepository, TagRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<ITagService, TagService>();

///... Other repository and service registrations
builder.Services.AddScoped<INotificacionService, NotificacionService>();

///... Other repository and service registrations
builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();

///... Other repository and service registrations
builder.Services.AddScoped<ILikeComentarioRepository, LikeComentarioRepository>();

//builder.Services.AddScoped<IEmailService, EmailService>();
/// SMTP Email Service Configuration
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

/// Email Template Service Configuration
builder.Services.AddSingleton<EmailTemplateService>();

/// Notification Service Configuration
builder.Services.AddScoped<INotificacionesService, NotificacionesService>();

/// Like Service Configuration
builder.Services.AddScoped<ILikeService, LikeService>();


/// JWT Authentication Configuration
var key = builder.Configuration["Jwt:Key"];
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
        };
        // 🔥 Necesario para SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (
                    !string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/hubs/notificaciones")
                )
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
        };
    });

/// Authorization Policies Configuration
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "PuedeEditarPost",
        policy =>
            policy.RequireAssertion(context =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var rol = context.User.FindFirst(ClaimTypes.Role)?.Value;

                // Admin y Editor pueden editar cualquier post
                if (rol == "Administrador" || rol == "Editor")
                    return true;

                // Autor solo puede editar sus posts
                if (rol == "Autor" && userId != null)
                    return true;

                return false;
            })
    );
});

/// Build the application
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    DbSeeder.SeedAdmin(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option =>
    {
        option.RoutePrefix = "docs";
    });
}

/// Middleware Configuration
app.UseAuthentication();

/// Authorization Configuration
app.UseAuthorization();

/// Static Files Configuration
app.UseStaticFiles();

/// Map Controllers and Hubs
app.MapControllers();

/// SignalR Hub Mapping
app.MapHub<NotificacionesHub>("/hubs/notificaciones");

/// Run the application
app.Run();
