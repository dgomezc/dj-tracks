# DJ Tracks — Instrucciones para agentes (Codex y GitHub Copilot)

Reproductor autoalojado de música electrónica. Plan completo en `docs/PLAN.md`; léelo antes de trabajar.

## Al iniciar o retomar una sesión
- Lee este archivo, `docs/PLAN.md`, `docs/ARQUITECTURA.md` y `docs/VALIDACIONES.md`. Estas son las instrucciones persistentes; no dependas del historial del chat.
- Comprueba la rama, los cambios locales y el estado actual del PR antes de continuar. No asumas que un PR sigue abierto o que una fase está aprobada.
- Retoma la fase pendiente y conserva las decisiones confirmadas del plan. No avances automáticamente si falta validación del usuario.

## Reglas
- Responde y documenta en español.
- KISS y YAGNI: la solución más simple; sin abstracciones ni mejoras especulativas.
- Respeta las convenciones existentes; no refactorices salvo que sea necesario.
- No generes tests salvo petición explícita. Valida compilando (`dotnet build DjTracks.slnx`) y con comprobaciones manuales.
- Trabaja una fase del plan cada vez; al terminar, indica cómo probarla en el navegador.
- Consulta la documentación actualizada con context7 (ASP.NET Core, EF Core, TagLibSharp, FFMpegCore, NAudio). Para Blazor Blueprint usa su servidor MCP (`.mcp.json`).
- El código debe ejecutarse y depurarse desde Visual Studio 2026 (.NET 10). No tocar Docker hasta la fase F6.1.

## Flujo de Git
- Una rama por fase, desde `develop`: `fase/f1-2-filtros` (id de fase + nombre corto).
- Commits pequeños y atómicos, en español, con mensaje descriptivo (`feat:`, `fix:`, `docs:`, `chore:`).
- Al terminar la fase: push y PR hacia `develop` (repositorio `dgomezc/dj-tracks`) con resumen y pasos de prueba manual. No hacer merge sin validación del usuario.
- Una fase no empieza hasta que la anterior esté mergeada, salvo petición expresa.
- Incluye el trailer `Co-authored-by: Copilot App <223556219+Copilot@users.noreply.github.com>` en los commits, salvo petición contraria.

## Entrega y validación manual
- En cada entrega que requiera validación, notifica en español qué ha cambiado y exactamente qué debe probar el usuario; no basta con enlazar el PR.
- Incluye los requisitos de arranque (proyectos, perfil de Visual Studio, URL y configuración o datos necesarios), pasos concretos y resultado esperado de cada comprobación.
- Distingue las comprobaciones realizadas por el agente de las pendientes del usuario. Compilar o recibir HTTP 200 no equivale a validar visualmente la interfaz.
- Mantén `docs/VALIDACIONES.md` con la fase y PR actuales, estado, checklist y limitaciones conocidas. Copia la checklist pertinente en el PR y en el mensaje de entrega.
- Si cambia algo después de la entrega, explica qué comprobaciones hay que repetir y cuáles son nuevas.
- No marques una fase como validada o terminada hasta que el usuario lo confirme. Registra su confirmación o los fallos reportados; corrige los fallos en la misma rama y solicita otra validación.
- No hagas merge sin autorización del usuario. Al autorizarlo, comprueba el estado del PR antes de actuar.

## Arquitectura
- **Obligatorio**: sigue `docs/ARQUITECTURA.md` (estructura, nomenclatura, flujo UI → HTTP → endpoint → validador → handler → `Result`, errores con `AppError`). Todo caso de uso nuevo debe seguir su receta.
- `src/DjTracks.Api`: Minimal API con vertical slices en `Features/<Feature>/<Caso>` (Endpoint + Handler), validación con FluentValidation y patrón Result con FluentResults.
- `src/DjTracks.UI`: Blazor Server interactivo + Blazor Blueprint (componentes `Bb*` con el tema "Light Green" de tweakcn, modos claro y oscuro; ver `docs/ARQUITECTURA.md`, sección 8). Llama a la API por HTTP desde el code-behind con `HttpClientResultExtensions`.
- `src/DjTracks.Domain`: entidades y enums. `src/DjTracks.Infrastructure`: ficheros, tags, onda, EF Core.
- Biblioteca y playlists en ficheros; PostgreSQL solo para estadísticas.
- No commitear secretos (usar user-secrets para la conexión a BD).
- NuGet: `NuGet.Config` limita las fuentes a nuget.org e ignora los feeds privados heredados. Usa la restauración normal, sin `--source`.
