📘 APIBlog — Resumen del Proyecto
APIBlog es una plataforma modular de blogging construida con ASP.NET Core y un panel de administración en Blazor Server.
Incluye autenticación, permisos avanzados, sanitización, notificaciones en tiempo real y una arquitectura limpia pensada para crecer.

🚀 Tecnologías
Backend: ASP.NET Core, EF Core, JWT, SignalR

Frontend: Blazor Server, Bootstrap

Tests: xUnit, Moq

🗂 Estructura
Código
APIBlog.sln
 ├── Backend/
 │    └── BlogApi/
 ├── Frontend/
 │    └── BlogAdmin/
 └── Tests/
      └── BlogApi.Tests/
📋 Checklist de Progreso (Versión Reducida)
🟦 Backend
[x] PostsController modernizado

[x] PostService actualizado (permisos, sanitización, categorías, tags)

[x] Notificaciones con payload JSON + mapper ToDto

[ ] Revisar CategoríasController

[ ] Revisar TagsController

[ ] Revisar NotificacionesController

🟦 Frontend (Blazor Server)
[x] Login + autenticación

[x] Panel de administración

[x] SignalR para notificaciones

[ ] Tests de componentes (opcional)

🟦 Tests
[x] TestBase para PostService

[x] Tests del PostsController

[ ] Tests del PostService (Create, Update, Delete)

[ ] Tests de Notificaciones

[ ] Tests de Mappers

[ ] Tests de Categorías y Tags

▶️ Ejecutar el proyecto
Backend
Código
cd BlogApi
dotnet run
Frontend
Código
cd BlogAdmin
dotnet run
Tests
Código
dotnet test
🤝 Contribución
PRs bienvenidos.
El proyecto sigue principios de arquitectura limpia y buenas prácticas de testing.
