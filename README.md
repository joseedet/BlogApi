📘 BlogApi – Backend Modular para Plataforma de Blog Profesional

BlogApi es una API REST robusta, modular y extensible diseñada para gestionar un sistema de blog profesional con:

Autenticación JWT

Roles y permisos

Posts con slugs SEO

Categorías y etiquetas

Comentarios y respuestas (anidados)

Notificaciones en tiempo real y por email

Búsqueda avanzada y filtros

Arquitectura limpia por capas

Paginación, ordenación y extensibilidad total

Este backend está optimizado para ser consumido por un frontend moderno como Blazor Server, aunque puede integrarse con cualquier cliente.

🚀 Características principales

🔐 Autenticación JWT
Login con email/usuario + contraseña.

Tokens JWT firmados.

Refresh tokens (si se desea).

Endpoints protegidos con [Authorize].

Roles integrados en el token.

🛡 Roles y permisos

Roles disponibles:

Admin

Editor

Usuario

Control de acceso granular:

Admin → acceso total.

Editor → gestión de posts y comentarios.

Usuario → creación de contenido propio.

Middleware de autorización por rol.

Decoradores como:

csharp
[Authorize(Roles = "Admin,Editor")]
📝 Gestión de Posts

✔ Crear, editar, eliminar posts
✔ Slugs SEO automáticos
Ejemplo:

Código
"Mi Primer Post" → "mi-primer-post"
✔ Categorías
Un post pertenece a una categoría.

Las categorías pueden listarse, filtrarse y administrarse.

✔ Etiquetas (tags)
Un post puede tener múltiples etiquetas.

Las etiquetas permiten búsquedas más precisas.

Sistema many-to-many con tabla intermedia.

✔ Búsqueda avanzada
Por título

Por contenido

Por slug

Por categoría

Por etiquetas

Por autor

Por fecha

Combinación de filtros

✔ Ordenación
Por fecha

Por relevancia

Por popularidad (si lo implementas más adelante)

💬 Comentarios y respuestas

✔ Comentarios directos al post
✔ Respuestas a comentarios (comentarios anidados)
✔ Estructura lista para hilos profundos
✔ Paginación opcional
Cada comentario incluye:

Autor

Fecha

Contenido

PostId

ComentarioPadreId (si es respuesta)

🔔 Sistema de notificaciones

✔ Notificaciones automáticas
Nuevo comentario en un post → notificación al autor del post.

Respuesta a un comentario → notificación al autor del comentario original.

Nuevo post (opcional) → notificación a seguidores o administradores.

✔ Tipos de notificación
NuevoPost

NuevoComentario

RespuestaComentario

✔ Canales
Base de datos

Tiempo real (SignalR)

Email (opcional)

✔ Payload JSON
Incluye datos como:

json
{
  "postId": 10,
  "comentarioId": 55,
  "contenido": "Texto del comentario"
}
📡 Notificaciones en tiempo real (SignalR)

Hub:
Código
/hubs/notificaciones
Evento:
Código
NuevaNotificacion
Flujo:
Se crea una notificación.

Se guarda en BD.

Se envía por SignalR al usuario destinatario.

El frontend actualiza la UI en tiempo real.

📧 Notificaciones por email

✔ Emails automáticos en:
Nuevo comentario en tu post

Respuesta a tu comentario

Nuevo post (opcional)

✔ Plantillas HTML
Personalizables

Variables dinámicas (nombre del usuario, título del post, etc.)

✔ Integración SMTP
Compatible con:

Gmail

Outlook

SendGrid

Cualquier servidor SMTP

🧩 Categorías y Etiquetas

Categorías
Un post pertenece a una categoría.

CRUD completo.

Filtros por categoría.

Etiquetas (Tags)
Un post puede tener múltiples etiquetas.

CRUD completo.

Filtros por etiquetas.

Búsqueda combinada:

Código
posts?tag=aspnet&tag=backend&categoria=programacion

🔍 Búsqueda y filtros avanzados
✔ Búsqueda por texto completo
Título

Contenido

Slug

Etiquetas

Categoría

✔ Filtros combinados
Ejemplo:

Código
/api/posts?search=blazor&categoria=frontend&tag=signalr&page=1&pageSize=10
✔ Ordenación
sort=fecha_desc

sort=fecha_asc

sort=popularidad

🧱 Arquitectura del proyecto
Código
BlogApi/
 ├── Controllers/        → Endpoints REST
 ├── Services/           → Lógica de negocio
 ├── Repositories/       → Acceso a datos
 ├── Domain/
 │    └── Factories/     → Creación de entidades (ej. Notificaciones)
 ├── DTO/                → Transferencia de datos
 ├── Models/             → Entidades EF Core
 ├── Hubs/               → SignalR
 ├── Data/               → DbContext
 ├── Auth/               → JWT, roles, permisos
 ├── Email/              → Servicio de email + plantillas
 └── Utils/              → Helpers (slug generator, filtros, etc.)
🔄 Flujos funcionales
1. Crear un post
Se genera slug SEO.

Se asigna categoría.

Se asignan etiquetas.

Se guarda en BD.

Se notifica (opcional).

2. Comentar un post
Se guarda comentario.

Se notifica al autor del post.

Se envía email (opcional).

Se envía SignalR.

3. Responder a un comentario
Se guarda respuesta.

Se notifica al autor del post.

Se notifica al autor del comentario original.

Se envía email (opcional).

Se envía SignalR.

4. Búsqueda avanzada
Se combinan filtros.

Se aplica paginación.

Se devuelve resultado optimizado.

🧪 Estado actual del proyecto
✔ Autenticación JWT
✔ Roles y permisos
✔ Slugs SEO
✔ Categorías
✔ Etiquetas
✔ Posts
✔ Comentarios
✔ Respuestas
✔ Notificaciones en BD
✔ Notificaciones en tiempo real
✔ Notificaciones por email
✔ Búsqueda avanzada
✔ Filtros avanzados
✔ Paginación
✔ Arquitectura limpia
✔ Factories
✔ Repositorios
✔ Servicios
✔ SignalR
✔ Sistema modular y extensible

