using System.Text;
using BlogApi.Authorization;
using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Middleware;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using BlogApi.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositorios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();
builder.Services.AddScoped<ILikeComentarioRepository, LikeComentarioRepository>();

// Servicios de la aplicación
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<EmailTemplateService>();
builder.Services.AddScoped<INotificacionesService, NotificacionesService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ISanitizerService, SanitizerService>();
builder.Services.AddSingleton<IAuthorizationHandler, PuedeEditarPostHandler>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configuración de autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),
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

builder.Services.PostConfigure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        options.TokenValidationParameters.RoleClaimType = "rol";
    }
);

// Políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "PuedeEditarPost",
        policy =>
        {
            policy.Requirements.Add(new PuedeEditarPostRequirement());
        }
    );
});

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

app.UseAuthentication();

app.UseSecurityHeaders();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapHub<NotificacionesHub>("/hubs/notificaciones");

app.Run();
