# Validaciones manuales y continuidad

Este archivo guarda el estado de entrega para retomar el proyecto en otra sesión.
Las preferencias permanentes están en `../AGENTS.md` y las decisiones funcionales en `PLAN.md`.
Actualizar esta información en cada entrega, revisión o confirmación del usuario.
Consultar siempre el estado real de GitHub antes de actuar sobre un PR.

## Estado actual

- Planificación: PR #1 mergeado.
- Fase activa: F0.1, layout base.
- PR: [#2](https://github.com/dgomezc/dj-tracks/pull/2), dirigido a `develop`.
- Rama: `fase/f0-1-layout`.
- Estado: pendiente de validación manual del usuario; no iniciar F0.2 todavía.
- Sin cambios funcionales nuevos en esta entrega: se documenta el protocolo de validación.

## F0.1: qué validar

Cambios: MudBlazor con tema oscuro, navegación principal, páginas provisionales,
panel izquierdo de playlists y espacio fijo inferior para el futuro reproductor.
Además, `NuGet.Config` limita la restauración a nuget.org sin modificar los feeds
globales del equipo.

### Preparación

Abrir `DjTracks.slnx` en Visual Studio 2026 con .NET 10.
Seleccionar `DjTracks.UI` como proyecto de inicio y el perfil `http`.
Ejecutar y abrir `http://localhost:5050`.
No hacen falta archivos de música, base de datos ni API para estas páginas provisionales.

### Checklist del usuario

- [ ] Restaurar los paquetes de la solución sin indicar una fuente manualmente: no debe consultarse el feed privado ni aparecer su error 401.
- [ ] Abrir Inicio: debe aparecer el tema oscuro, el nombre DJ Tracks y el texto provisional del dashboard.
- [ ] Navegar a Tracks, Sessions y Remember desde la barra superior: deben cambiar el título y el contenido central sin errores.
- [ ] Pulsar el icono de configuración: debe abrir `/settings` y mostrar el texto provisional de Configuración.
- [ ] Durante la navegación, comprobar que siguen visibles el panel izquierdo con las dos secciones de playlists vacías y la barra inferior del reproductor.
- [ ] Recargar directamente `/tracks`, `/sessions`, `/remember` y `/settings`: cada URL debe abrir su página correspondiente.

### Comprobaciones del agente y límites

- La UI compiló y las cinco rutas respondieron HTTP 200 en la entrega inicial.
- La restauración de los cuatro proyectos con la configuración local de NuGet se completó sin `--source`; la única fuente efectiva listada fue nuget.org.
- No se ha realizado una comprobación visual ni se ha recibido confirmación del usuario.
- Las páginas son provisionales: no reproducen audio, no muestran biblioteca ni permiten editar rutas o playlists.
- El cliente HTTP de la API está registrado, pero estas páginas aún no lo consumen.
- El perfil compartido de inicio múltiple Api + UI previsto en F0.1 sigue pendiente; no dar la fase completa por terminada hasta resolverlo o acordar su aplazamiento.

## Formato para futuras entregas

Registrar la fase y PR, el estado de validación, los cambios y las comprobaciones
que deben repetirse. Para cada comprobación indicar una acción y su resultado
esperado, junto con los requisitos de arranque y los límites conocidos.
Mantener las confirmaciones o incidencias del usuario para que otra sesión no
confunda implementación, validación y autorización de merge.
