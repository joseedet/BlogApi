📘 README — APIBlog
<div align="center">

APIBlog — Plataforma Modular de Blog Profesional
Backend en ASP.NET Core · Frontend en Blazor Server · Tests con xUnit + Moq
Badges
https://img.shields.io/badge/build-passing-brightgreen
https://img.shields.io/badge/tests-ongoing-blue
https://img.shields.io/badge/license-MIT-lightgrey
https://img.shields.io/badge/status-active-success

</div>

📑 Índice
Descripción del Proyecto

Tecnologías

Estructura de la Solución

Arquitectura

Checklist de Progreso

Backend

Frontend

Tests

Cómo Ejecutar el Proyecto

Cómo Ejecutar los Tests

Contribución

Licencia

🧩 Descripción del Proyecto
APIBlog es una plataforma modular de blogging inspirada en WordPress, pero diseñada desde cero con:

Arquitectura limpia

Código mantenible

Servicios desacoplados

Tests unitarios profesionales

Panel de administración moderno en Blazor Server

Notificaciones en tiempo real con SignalR

El objetivo es construir un sistema escalable, seguro y extensible, apto para producción y para aprendizaje avanzado.

🛠 Tecnologías
Backend
ASP.NET Core 8

Entity Framework Core

JWT Authentication

FluentValidation (opcional)

SignalR

Sanitización HTML

Frontend (Admin Panel)
Blazor Server

SignalR Client

Bootstrap 5

Tests
xUnit

Moq

TestBase reutilizable

Arquitectura de tests modular

🗂 Estructura de la Solución
Código
APIBlog.sln
 ├── Backend/
 │    ├── BlogApi/
 │    │    ├── Controllers/
 │    │    ├── Services/
 │    │    ├── Repositories/
 │    │    ├── DTOs/
 │    │    ├── Entities/
 │    │    └── Utils/
 ├── Frontend/
 │    ├── BlogAdmin/
 │    │    ├── Pages/
 │    │    ├── Components/
 │    │    ├── Services/
 │    │    └── Hubs/
 └── Tests/
      ├── BlogApi.Tests/
      │    ├── Common/
      │    ├── Controllers/
      │    ├── Services/
      │    └── Utils/
🧱 Arquitectura
El proyecto sigue principios de:

Clean Architecture

Separación de responsabilidades

Inyección de dependencias

Servicios desacoplados

Repositorios genéricos

DTOs explícitos y mappers claros

Tests aislados con mocks

📋 Checklist de Progreso
Este checklist documenta el estado actual del proyecto y las tareas pendientes.

🟦 Backend
✔ Modernización del API
[x] PostsController actualizado

[x] Manejo profesional de errores

[x] Validación de ModelState

[x] Extracción correcta de usuarioId y permisos

[x] Limpieza de firmas antiguas

✔ Modernización del PostService
[x] Firmas modernas implementadas

[x] Integración de ICategoriaRepository

[x] Integración de ISanitizerService

[x] Integración de INotificacionService

[x] Lógica de permisos profesional

✔ Notificaciones
[x] DTO moderno

[x] Mapper ToDto()

[x] Payload JSON

[x] Integración con SignalR

🔶 Pendiente
[ ] Revisar CategoríasController

[ ] Revisar TagsController

[ ] Revisar NotificacionesController

[ ] Revisar AuthController

[ ] Revisar servicios auxiliares

🟦 Frontend (Blazor Server)
✔ Completado
[x] Login y autenticación

[x] Panel de administración

[x] Integración con API

[x] SignalR para notificaciones

🔶 Pendiente
[ ] Tests de componentes

[ ] Tests de servicios HttpClient

🟦 Tests
✔ Infraestructura
[x] PostServiceTestBase creado

[x] Mocks centralizados

[x] CreateService()

[x] SetupExistingPost actualizado

✔ Tests del PostsController
[x] Create

[x] Update

[x] Delete

[x] NotFound

[x] BadRequest

[x] Excepciones

[x] Permisos

🔶 Tests del PostService
CreateAsync
[ ] Crea correctamente

[ ] Categoría inexistente

[ ] Tags inexistentes

[ ] Sanitización aplicada

[ ] Devuelve null

UpdateAsync
[ ] Post no encontrado

[ ] Usuario sin permisos

[ ] Usuario con permisos

[ ] Tags actualizados

[ ] Categoría inexistente

[ ] Sanitización aplicada

DeleteAsync
[ ] Post no encontrado

[ ] Usuario sin permisos

[ ] Usuario con permisos

[ ] Eliminación correcta

🔶 Tests de Notificaciones
[ ] NuevoComentario

[ ] NuevoLike

[ ] MarcarComoLeida

[ ] ObtenerNotificaciones

🔶 Tests de Mappers
[ ] NotificacionExtensions.ToDto()

[ ] Post → PostDto

🔶 Tests de Categorías y Tags
[ ] CategoríasController

[ ] TagsController

[ ] Servicios correspondientes

▶️ Cómo Ejecutar el Proyecto
Backend
Código
cd BlogApi
dotnet run
Frontend
Código
cd BlogAdmin
dotnet run
🧪 Cómo Ejecutar los Tests
Código
dotnet test
🤝 Contribución
Haz un fork

Crea una rama

Envía un PR

Se revisará siguiendo estándares de arquitectura limpia

📄 Licencia
MIT License — libre para uso personal y comercial.
