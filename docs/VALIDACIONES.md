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
- Estado: F0.1 validada por el usuario el 26/10/2026, incluido el inicio conjunto ("Perfecto, mezcla el PR y detente que quiero hacer una pregunta"). Merge autorizado; detener el desarrollo y no iniciar F0.2 hasta nueva indicación.
- Novedad de esta entrega: `DjTracks.slnLaunch` permite iniciar y depurar Api + UI conjuntamente.

## F0.1: qué validar

Cambios: MudBlazor con tema oscuro, navegación principal, páginas provisionales,
panel izquierdo de playlists y espacio fijo inferior para el futuro reproductor.
Además, `NuGet.Config` limita la restauración a nuget.org sin modificar los feeds
globales del equipo.

### Preparación

Abrir `DjTracks.slnx` en Visual Studio 2026 con .NET 10.
Seleccionar el perfil compartido `DJ Tracks (Api + UI)` y pulsar F5.
Se inician ambos proyectos con el destino `http`; la UI abre `http://localhost:5050`
y la API escucha en `http://localhost:5247`.
Si no aparece el perfil, recargar la solución y revisar **Configurar proyectos de inicio**.
No hacen falta archivos de música, base de datos ni API para estas páginas provisionales.

### Checklist del usuario

- [x] Restaurar los paquetes de la solución sin indicar una fuente manualmente: no debe consultarse el feed privado ni aparecer su error 401.
- [x] Abrir Inicio: debe aparecer el tema oscuro, el nombre DJ Tracks y el texto provisional del dashboard.
- [x] Navegar a Tracks, Sessions y Remember desde la barra superior: deben cambiar el título y el contenido central sin errores.
- [x] Pulsar el icono de configuración: debe abrir `/settings` y mostrar el texto provisional de Configuración.
- [x] Durante la navegación, comprobar que siguen visibles el panel izquierdo con las dos secciones de playlists vacías y la barra inferior del reproductor.
- [x] Recargar directamente `/tracks`, `/sessions`, `/remember` y `/settings`: cada URL debe abrir su página correspondiente.

### Nueva comprobación (no repetir las anteriores)

- [x] Seleccionar `DJ Tracks (Api + UI)` y pulsar F5: deben iniciarse ambos proyectos bajo el depurador y abrirse la UI en `http://localhost:5050`.
- [x] Abrir `http://localhost:5247/openapi/v1.json`: debe devolver el documento JSON de la API.
- [x] Detener la depuración en Visual Studio: deben detenerse ambos proyectos.

### Comprobaciones del agente y límites

- La UI compiló y las cinco rutas respondieron HTTP 200 en la entrega inicial.
- La restauración de los cuatro proyectos con la configuración local de NuGet se completó sin `--source`; la única fuente efectiva listada fue nuget.org.
- El usuario confirmó la checklist original y el nuevo perfil el 26/10/2026.
- Las páginas son provisionales: no reproducen audio, no muestran biblioteca ni permiten editar rutas o playlists.
- El cliente HTTP de la API está registrado, pero estas páginas aún no lo consumen.
- El perfil compartido de inicio múltiple está añadido y validado por el usuario desde Visual Studio.

## Formato para futuras entregas

Registrar la fase y PR, el estado de validación, los cambios y las comprobaciones
que deben repetirse. Para cada comprobación indicar una acción y su resultado
esperado, junto con los requisitos de arranque y los límites conocidos.
Mantener las confirmaciones o incidencias del usuario para que otra sesión no
confunda implementación, validación y autorización de merge.
