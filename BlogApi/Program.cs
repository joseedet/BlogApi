using BlogApi.Authorization;
using BlogApi.Data;
using BlogApi.DTO;
using BlogApi.Hubs;
using BlogApi.Middleware;
using BlogApi.Models;
using BlogApi.Repositories;
using BlogApi.Repositories.Interfaces;
using BlogApi.Services;
using BlogApi.Services.Interfaces;
using BlogApi.Services.Security;

var builder = WebApplication.CreateBuilder(args);

//Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.WithExceptionDetails()
    .Enrich.WithSpan()
    // correlación de trazas
    .WriteTo.Console()
    .WriteTo.Async(a =>
        a.File(
            "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            fileSizeLimitBytes: 10_000_000,
            rollOnFileSizeLimit: true
        )
    )
    .WriteTo.Async(a =>
        a.File("logs/efcore-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CrearPageDtoValidator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Default",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true); // Permite cualquier origen
        }
    );
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
var provider = builder.Configuration["Database:Provider"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (provider == "SqlServer")
    {
        var connection = builder.Configuration["Database:SqlServer"];
        options.UseSqlServer(connection)
         .EnableSensitiveDataLogging()
        .LogTo(Log.Information, LogLevel.Information);
    }
    else if (provider == "MariaDb")
    {
        var connection = builder.Configuration["Database:MariaDb"];
        options.UseMySql(connection, ServerVersion.AutoDetect(connection))
        .EnableSensitiveDataLogging()
        .LogTo(Log.Information, LogLevel.Information);
    }
    else
    {
        throw new Exception("Proveedor de base de datos no soportado.");
    }
});


/*builder.Services.AddDbContext<BlogDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .EnableSensitiveDataLogging()
        .LogTo(Log.Information, LogLevel.Information)
);*/
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Repositorios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<ILikeComentarioRepository, LikeComentarioRepository>();
builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Servicios de la aplicación
builder.Services.AddSingleton<IAuthorizationHandler, PuedeEditarPostHandler>();
builder.Services.AddScoped<IAuthorizationServiceBlog, AuthorizationService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailSettingsService, EmailSettingsService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<INotificacionesService, NotificacionesService>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ISanitizerService, SanitizerService>();
builder.Services.AddScoped<IStatsService, EstadisticasService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICacheConfigService, CacheConfigService>();

var cacheProvider = builder.Configuration["Cache:Provider"];

if (cacheProvider == "Redis")
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Cache:RedisConnection"];
    });

    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ICacheService, MemoryCacheService>();
}

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
            ClockSkew = TimeSpan.Zero,
        };
        // 🔥 Necesario para SignalR
        // Necesario para SignalR con WebSockets
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
        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    }
);

// Políticas de autorización
builder.Services.AddAuthorization(options =>
{
    // Banners
    options.AddPolicy("Permiso:Banners.Crear", p => p.RequireClaim("permiso", "Banners.Crear"));
    options.AddPolicy("Permiso:Banners.Editar", p => p.RequireClaim("permiso", "Banners.Editar"));
    options.AddPolicy(
        "Permiso:Banners.Eliminar",
        p => p.RequireClaim("permiso", "Banners.Eliminar")
    );

    //Caché
    options.AddPolicy("Permiso:Cache.Editar", p => p.RequireClaim("permiso", "Cache.Editar"));

    // Categorías
    options.AddPolicy("Permiso:Categorias.Ver", p => p.RequireClaim("permiso", "Categorias.Ver"));
    options.AddPolicy(
        "Permiso:Categorias.Crear",
        p => p.RequireClaim("permiso", "Categorias.Crear")
    );
    options.AddPolicy(
        "Permiso:Categorias.Editar",
        p => p.RequireClaim("permiso", "Categorias.Editar")
    );
    options.AddPolicy(
        "Permiso:Categorias.Eliminar",
        p => p.RequireClaim("permiso", "Categorias.Eliminar")
    );

    // Comentarios
    options.AddPolicy(
        "Permiso:Comentarios.Moderar",
        p => p.RequireClaim("permiso", "Comentarios.Moderar")
    );
    options.AddPolicy(
        "Permiso:Comentarios.Eliminar",
        p => p.RequireClaim("permiso", "Comentarios.Eliminar")
    );

    // Email Logs
    options.AddPolicy("Permiso:EmailLogs.Ver", p => p.RequireClaim("permiso", "EmailLogs.Ver"));

    // Email Settings
    options.AddPolicy(
        "Permiso:EmailSettings.Ver",
        p => p.RequireClaim("permiso", "EmailSettings.Ver")
    );
    options.AddPolicy(
        "Permiso:EmailSettings.Editar",
        p => p.RequireClaim("permiso", "EmailSettings.Editar")
    );
    options.AddPolicy(
        "Permiso:EmailSettings.Test",
        p => p.RequireClaim("permiso", "EmailSettings.Test")
    );

    // Notification Settings
    options.AddPolicy(
        "Permiso:NotificationSettings.Ver",
        p => p.RequireClaim("permiso", "NotificationSettings.Ver")
    );
    options.AddPolicy(
        "Permiso:NotificationSettings.Editar",
        p => p.RequireClaim("permiso", "NotificationSettings.Editar")
    );

    // Páginas
    options.AddPolicy("Permiso:Paginas.Ver", p => p.RequireClaim("permiso", "Paginas.Ver"));
    options.AddPolicy("Permiso:Paginas.Crear", p => p.RequireClaim("permiso", "Paginas.Crear"));
    options.AddPolicy("Permiso:Paginas.Editar", p => p.RequireClaim("permiso", "Paginas.Editar"));
    options.AddPolicy(
        "Permiso:Paginas.Eliminar",
        p => p.RequireClaim("permiso", "Paginas.Eliminar")
    );
    options.AddPolicy(
        "Permiso:Paginas.VerVersiones",
        p => p.RequireClaim("permiso", "Paginas.VerVersiones")
    );
    options.AddPolicy(
        "Permiso:Paginas.RestaurarVersion",
        p => p.RequireClaim("permiso", "Paginas.RestaurarVersion")
    );
    options.AddPolicy("Permiso:Posts.Publicar", p => p.RequireClaim("permiso", "Posts.Publicar"));
    options.AddPolicy("Permiso:Posts.Destacar", p => p.RequireClaim("permiso", "Posts.Destacar"));

    // Estadísticas
    options.AddPolicy("Permiso:Stats.Ver", p => p.RequireClaim("permiso", "Stats.Ver"));
    options.AddPolicy(
        "Permiso:Stats.VerActividad",
        p => p.RequireClaim("permiso", "Stats.VerActividad")
    );

    // Media
    options.AddPolicy("Permiso:Media.Subir", p => p.RequireClaim("permiso", "Media.Subir"));

    // Usuarios
    options.AddPolicy("Permiso:Usuarios.Ver", p => p.RequireClaim("permiso", "Usuarios.Ver"));
    options.AddPolicy(
        "Permiso:Usuarios.Bloquear",
        p => p.RequireClaim("permiso", "Usuarios.Bloquear")
    );
    options.AddPolicy("Permiso:Usuarios.Ver", p => p.RequireClaim("permiso", "Usuarios.Ver"));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    DbSeeder.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option =>
    {
        option.RoutePrefix = "docs";
    });
}
app.UseCors("Default");

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseMiddleware<UsuarioBloqueadoMiddleware>();

app.UseAuthorization();

app.UseSecurityHeaders();

app.UseStaticFiles();

app.MapControllers();

app.MapHub<NotificacionesHub>("/hubs/notificaciones");

app.Run();
