# DJ Tracks — Instrucciones para agentes (Codex y GitHub Copilot)

Reproductor autoalojado de música electrónica. Plan completo en `docs/PLAN.md`; léelo antes de trabajar.

## Reglas
- Responde y documenta en español.
- KISS y YAGNI: la solución más simple; sin abstracciones ni mejoras especulativas.
- Respeta las convenciones existentes; no refactorices salvo que sea necesario.
- No generes tests salvo petición explícita. Valida compilando (`dotnet build DjTracks.slnx`) y con comprobaciones manuales.
- Trabaja una fase del plan cada vez; al terminar, indica cómo probarla en el navegador.
- Consulta la documentación actualizada con context7 (MudBlazor, ASP.NET Core, EF Core, TagLibSharp, FFMpegCore, NAudio).
- El código debe ejecutarse y depurarse desde Visual Studio 2026 (.NET 10). No tocar Docker hasta la fase F6.1.

## Flujo de Git
- Una rama por fase, desde `develop`: `fase/f1-2-filtros` (id de fase + nombre corto).
- Commits pequeños y atómicos, en español, con mensaje descriptivo (`feat:`, `fix:`, `docs:`, `chore:`).
- Al terminar la fase: push y PR hacia `develop` (repositorio `dgomezc/dj-tracks`) con resumen y pasos de prueba manual. No hacer merge sin validación del usuario.
- Una fase no empieza hasta que la anterior esté mergeada, salvo petición expresa.

## Arquitectura
- `src/DjTracks.Api`: Minimal API con vertical slices en `Features/<Feature>`.
- `src/DjTracks.UI`: Blazor Server interactivo + MudBlazor.
- `src/DjTracks.Domain`: entidades y enums. `src/DjTracks.Infrastructure`: ficheros, tags, onda, EF Core.
- Biblioteca y playlists en ficheros; PostgreSQL solo para estadísticas.
- No commitear secretos (usar user-secrets para la conexión a BD).
