# Changelog
Todas las modificaciones relevantes de este proyecto se documentarán en este archivo.

El formato sigue las recomendaciones de **Keep a Changelog**  
y el versionado se basa en **Semantic Versioning**.

---

## [Unreleased]
### Añadido


## [1.5.0] – Fase 4 · Bloque 2 · SEO, Auditoría y Versionado de Pages

### Añadido
- Campos SEO en la entidad Page:
  - MetaTitulo
  - MetaDescripcion
  - MetaKeywords
- Campos de auditoría adicionales expuestos en PageDto:
  - IpCreacion
  - UserAgentCreacion
  - IpActualizacion
  - UserAgentActualizacion
- Regla automática de borrador:
  - Si el contenido está vacío, la página se guarda como borrador (Publicado = false).
- Sistema de versionado en ActualizarAsync:
  - Se guarda una entrada en PageVersion antes de aplicar cambios.
- Mapeo completo de SEO y auditoría en PageDto.

###Cambiado
- CrearAsync:
  - Ahora guarda la página antes de desmarcar otras páginas de inicio.
  - Se aplica la regla de borrador automático.
  - Se integran los campos SEO.
- ActualizarAsync:
  - Se aplica la regla de borrador automático.
  - Se integran los campos SEO.
  - Se actualiza la auditoría de modificación.
  - Se guarda la versión previa mediante _pageRepository.GuardarVersionAsync.
- Mapper:
  - Ampliado para incluir SEO y campos de auditoría en PageDto.

###Corregido
- Falta de asignación de EsInicio en la creación de páginas.
- Error conceptual en CrearAsync:
  - Antes se desmarcaban páginas de inicio antes de guardar la nueva.
  - Ahora se guarda primero la nueva página y luego se desmarcan las demás.

---

## [1.4.0] – Fase 4 · Bloque 1 · Analíticas y Relacionados
### Añadido
- Implementación de **posts relacionados** por tags y categoría.
- Endpoint público: `GET /posts/{id}/related`.
- Métodos en `IPostRepository`:
  - `GetWithTagsAndCategoryAsync`
  - `GetRelatedByTagsAsync`
  - `GetRelatedByCategoryAsync`
- Servicio `GetRelatedPostsAsync` con combinación inteligente de tags + categoría.
- Implementación de **posts más vistos**.
- Endpoint: `GET /posts/most-viewed`.
- Implementación de **posts más comentados**.
- Endpoint: `GET /posts/most-commented`.
- Creación de la enumeración `ComentarioEstado` para moderación profesional.
- Añadido **contador de visitas** por post (`ViewsCount`).
- Incremento automático de visitas en el endpoint público de detalle.
- Índices SQL para optimizar:
  - Posts más vistos
  - Posts más comentados
  - Relacionados por categoría
  - Relacionados por tags
  - Ordenación por fecha
  - Slug único

### Cambiado
- `GetByIdAsync` ahora usa un método especializado con includes (`GetPostCompletoAsync`).
- Ajustes en el mapper manual para soportar nuevos campos y estructuras.

### Corregido
- Error al contar comentarios aprobados (no existía `IsApproved`).
- Se añadió `ComentarioEstado.Aprobado` para resolverlo correctamente.
- Conflicto de rutas entre endpoints `GetById` (se separó endpoint interno y público).

---

## [1.3.0] – Fase 3 · Bloque 1 · Sistema de Email Profesional
### Añadido
- Motor de plantillas de email basado en archivos HTML.
- Estructura modular para notificaciones por correo.
- Documentación completa del sistema de plantillas.
- Preparación del entorno para SMTP (pendiente de VPS).

### Cambiado
- Refactor del módulo de notificaciones para soportar payloads dinámicos.

---

## [1.2.0] – Fase 2 · Bloque 2 · Moderación y Comentarios
### Añadido
- Sistema de comentarios con respuestas anidadas.
- Moderación básica de comentarios.
- DTOs completos para comentarios y respuestas.
- Mapeo manual extendido para estructuras jerárquicas.

---

## [1.1.0] – Fase 2 · Bloque 1 · Categorías, Tags y Slugs
### Añadido
- CRUD completo de categorías.
- CRUD completo de tags.
- Generación automática de slugs únicos.
- Validación de duplicados.

---

## [1.0.0] – Fase 1 · Estructura Base del Proyecto
### Añadido
- Arquitectura inicial del backend.
- Repositorio genérico + repositorios específicos.
- Sistema de usuarios y roles.
- Autenticación JWT.
- Mapeo manual inicial.
- Controladores base.
