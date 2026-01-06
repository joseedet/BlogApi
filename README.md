📘 BlogApi – Backend del Sistema de Blog con Notificaciones en Tiempo Real
Bienvenido a BlogApi, una API REST modular, limpia y extensible diseñada para gestionar un sistema de blog profesional con:

Posts

Comentarios y respuestas (comentarios anidados)

Notificaciones en tiempo real vía SignalR

Paginación

Arquitectura por capas (Models, DTOs, Repositories, Services, Controllers, Domain/Factories)

Este backend está pensado para ser consumido por un frontend moderno, como un panel de administración en Blazor Server, aunque puede integrarse con cualquier cliente.

🚀 Características principales
📝 Gestión de Posts
Crear posts

Asociar posts a usuarios

Extensible para listados, filtros, slugs SEO, etc.

💬 Gestión de Comentarios
Comentar posts

Responder a comentarios (comentarios anidados)

Estructura lista para hilos profundos

🔔 Sistema de Notificaciones
Notificaciones automáticas cuando:

Un usuario comenta un post → notificación al autor del post

Un usuario responde a un comentario → notificación al autor del comentario original

Cada notificación:

Se guarda en base de datos

Se envía en tiempo real vía SignalR

Incluye un Payload JSON con datos relevantes (postId, comentarioId, contenido, etc.)

Puede consultarse (no leídas, paginadas)

Puede marcarse como leída

⚡ Tiempo real con SignalR
Hub dedicado: /hubs/notificaciones

El cliente recibe eventos NuevaNotificacion

Ideal para campanitas de notificaciones en Blazor o SPA

🧱 Arquitectura limpia
Separación clara por capas:

Código
BlogApi/
 ├── Controllers/
 ├── Services/
 ├── Repositories/
 ├── Domain/
 │    └── Factories/
 ├── DTO/
 ├── Models/
 ├── Hubs/
 └── Data/
🧩 Tecnologías utilizadas
ASP.NET Core 8

Entity Framework Core

SignalR

SQL Server (o cualquier provider compatible)

C# 12

Arquitectura por capas y principios SOLID

📂 Estructura del proyecto
Models
Entidades principales:

Usuario

Post

Comentario

Notificacion

TipoNotificacion (enum)

DTO
Objetos de transferencia:

PostDto, CreatePostDto

ComentarioDto, CreateComentarioDto

NotificacionDto

PaginacionResultado<T>

Extensiones .ToDto()

Domain / Factories
NotificacionFactory  
Genera notificaciones tipadas:

NuevoPost

NuevoComentario

RespuestaComentario

Repositories
IComentarioRepository, ComentarioRepository

IPostRepository, PostRepository

Acceso a datos encapsulado

Services
ComentarioService

PostService

NotificacionesService  
(guarda + emite notificaciones vía SignalR)

Hubs
NotificacionesHub

Controllers
ComentariosController

PostsController (si lo tienes)

Endpoints REST

🔄 Flujo funcional resumido
1. Crear un post
El usuario envía CreatePostDto

Se guarda en BD

Se devuelve PostDto

2. Comentar un post
El usuario envía CreateComentarioDto sin ComentarioPadreId

Se guarda el comentario

Se notifica al autor del post

3. Responder a un comentario
El usuario envía CreateComentarioDto con ComentarioPadreId

Se guarda el comentario hijo

Se notifica:

al autor del post (nuevo comentario)

al autor del comentario original (respuesta)

4. Notificaciones
Se guardan en BD

Se envían por SignalR

Se pueden consultar:

No leídas

Paginadas

Se pueden marcar como leídas

📡 SignalR – Tiempo real
Hub:

Código
/hubs/notificaciones
Evento emitido por el servidor:

Código
NuevaNotificacion
El cliente debe:

Conectarse al hub

Escuchar NuevaNotificacion

Actualizar UI en tiempo real

📘 Configuración básica (Program.cs)
Incluye:

DbContext

Repositorios

Servicios

SignalR

Swagger

🧪 Estado actual del proyecto
✔ Posts funcionando
✔ Comentarios funcionando
✔ Respuestas funcionando
✔ Notificaciones funcionando
✔ SignalR funcionando
✔ Paginación funcionando
✔ Arquitectura limpia y modular
