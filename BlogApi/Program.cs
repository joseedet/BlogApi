using System.Security.Claims;
using System.Text;
using BlogApi.Data;
using BlogApi.Hubs;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IComentarioService, ComentarioService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddScoped<INotificacionService, NotificacionService>();

builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();

builder.Services.AddScoped<ILikeComentarioRepository, LikeComentarioRepository>();

//builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddSingleton<EmailTemplateService>();

builder.Services.AddScoped<INotificacionesService, NotificacionesService>();

builder.Services.AddScoped<ILikeService, LikeService>();

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

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.MapHub<NotificacionesHub>("/hubs/notificaciones");

app.Run();
