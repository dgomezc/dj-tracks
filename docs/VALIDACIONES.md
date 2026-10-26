# Validaciones manuales y continuidad

Este archivo guarda el estado de entrega para retomar el proyecto en otra sesión.
Las preferencias permanentes están en `../AGENTS.md` y las decisiones funcionales en `PLAN.md`.
Actualizar esta información en cada entrega, revisión o confirmación del usuario.
Consultar siempre el estado real de GitHub antes de actuar sobre un PR.

## Estado actual

- Planificación: PR #1 mergeado.
- F0.1: validada por el usuario el 26/10/2026, incluido el inicio conjunto; PR [#2](https://github.com/dgomezc/dj-tracks/pull/2) mergeado en `develop` (estado comprobado en GitHub).
- Continuación autorizada el 26/10/2026: "Validado. Continua". La pausa anterior queda levantada.
- Fase activa: F0.2, configuración de rutas.
- PR: [#3](https://github.com/dgomezc/dj-tracks/pull/3), dirigido a `develop`.
- Rama del entorno: `dgomezc-f0-2-configuracion-de-rutas`.
- Estado: **F0.2 validada por el usuario el 26/10/2026**, incluidas la restauración de campos de texto y las descripciones. Confirmación: "Todo Ok, mezcla el PR y continúa como dijimos en el proyecto principal". Merge autorizado; comprobar GitHub para conocer su estado definitivo.
- Continuidad solicitada: "continúa como dijimos en el proyecto principal", y limpieza de worktrees pasados. El destino exacto no está confirmado en esta sesión; se coordina con la sesión principal antes de iniciar F1.1. Preservar cambios locales y ajustes no versionados antes de archivar; no borrar el worktree activo.
- Novedad: `/settings` consume la API y permite guardar las cinco rutas en JSON del servidor.
- Revisión del 26/10/2026: el usuario rechazó el selector de carpetas y pidió volver al formulario anterior. Retirados los botones Examinar, el diálogo y el endpoint de navegación; se mantienen los cinco campos de texto, la validación y el guardado originales. La elección previa del selector queda revocada.

## F0.1: validación completada (histórico)

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
- En F0.1 las páginas eran provisionales: sin reproducción, biblioteca, edición de rutas ni playlists.
- En F0.1 el cliente HTTP de la API estaba registrado, pero las páginas aún no lo consumían.
- El perfil compartido de inicio múltiple está añadido y validado por el usuario desde Visual Studio.

## F0.2: qué validar

Cambios: formulario MudBlazor para Tracks, Sessions, Remember, Playlists y raíz M3U;
GET/PUT `/api/settings`; errores por campo y avisos visibles de conexión,
JSON inválido y permisos. Se comprueban rutas absolutas de carpetas existentes,
lectura en todas y escritura en playlists. Se recortan espacios exteriores.
No se crean carpetas de biblioteca; sí el directorio del JSON al guardar por primera vez.
Las futuras exportaciones M3U usarán rutas relativas a la raíz configurada.

### Preparación

Visual Studio 2026 y .NET 10: abrir `DjTracks.slnx`, seleccionar
**DJ Tracks (Api + UI)** y F5.
UI: `http://localhost:5050/settings`; API: `http://localhost:5247/api/settings`.
Preparar cinco carpetas existentes (pueden estar vacías), accesibles por el
usuario de la API; playlists requiere escritura. No hacen falta música ni BD.
Las rutas son del servidor de la API, no del equipo del navegador.
Para la prueba de caída, iniciar UI y API por separado con sus perfiles `http`,
para poder detener solo la API.

El JSON local se guarda por defecto en `src/DjTracks.Api/App_Data/settings.json`;
`App_Data` está ignorado por Git. No existe hasta el primer guardado correcto.
Sus propiedades son `tracks`, `sessions`, `remember`, `playlists` y `m3uRoot`.
No editar `appsettings.json` para guardar rutas privadas.
Para cambiar la ubicación, usar `Settings:FilePath` mediante user-secrets o
`Settings__FilePath` en el entorno de la API (las ubicaciones relativas se
resuelven respecto a su content root). La identidad de la API necesita
lectura y permisos de creación/renombrado/borrado en el directorio del JSON.
Si la ubicación personalizada está dentro del repositorio pero fuera de
`App_Data`, hay que excluirla localmente de Git antes de usarla.
Para pruebas de corrupción o permisos, hacer primero una copia del JSON y
restaurarla al terminar; no usar datos irreemplazables.

### Checklist del usuario (confirmada el 26/10/2026)

- [x] Abrir Configuración: aparecen las cinco rutas y los paneles existentes; en la primera ejecución los campos están vacíos y no se crea JSON hasta guardar.
- [x] Guardar cinco carpetas válidas: aparece «Rutas validadas y guardadas en el servidor». Recargar, navegar fuera y volver, y reiniciar Api + UI: se conservan las cinco rutas.
- [x] Probar un campo vacío, una ruta relativa, una inexistente y una ruta a un fichero: aparece el error bajo el campo; no se modifica el JSON. «Volver a cargar» recupera lo guardado.
- [x] Editar tras guardar: desaparece el aviso de éxito. «Volver a cargar» descarta los cambios; durante las operaciones los controles quedan bloqueados.
- [x] Detener solo la API y pulsar «Volver a cargar»: aparece el aviso de conexión y se bloquea guardar. Reiniciar la API y volver a cargar: desaparece el error y regresan las rutas.
- [x] Hacer copia del JSON; dejar temporalmente JSON mal formado y volver a cargar: aparece el aviso de JSON inválido, guardar queda bloqueado y el fichero no se sobrescribe. Restaurar la copia y volver a cargar: se recupera el formulario.
- [x] Con copia del JSON, marcarlo de solo lectura e intentar guardar otra carpeta válida: aparece error de almacenamiento, sin éxito ni cambio del fichero. Quitar solo lectura y volver a cargar: se recuperan las rutas originales.
- Condicional NAS/UNC: incluida en la aceptación global del usuario cuando aplique; no hay detalle de ejecución sobre una ruta de red concreta. El agente no ha comprobado NAS/UNC real.

### Comprobaciones reales del agente

- `dotnet build DjTracks.slnx`: 0 errores y 0 advertencias.
- API y UI iniciadas con puertos y almacenamiento temporales aislados de los datos del usuario.
- GET inicial sin fichero: rutas vacías; no crea JSON.
- PUT con rutas vacías, relativas, inexistentes y ficheros: 400 con errores por campo, sin guardar.
- Guardado y recorte de espacios exteriores; GET conserva valores después de reiniciar la API.
- Veinte PUT concurrentes correctos: JSON íntegro y sin temporales restantes. Se corrigió un fallo reproducido de `File.Replace` en Windows usando renombrado con sobrescritura en el mismo directorio.
- ACL temporal denegando lectura en tracks y creación de ficheros en playlists: errores por campo. Permisos restaurados y guardado correcto.
- JSON de solo lectura: 500 y fichero anterior intacto.
- JSON mal formado, incompleto, `null` y con esquema inválido: GET/PUT devuelven 500 sin sobrescribir. Petición JSON mal formada: 400 sin cambios.
- Navegador interactivo: guardado con éxito, validación de ruta relativa, error JSON con campos/guardar bloqueados, API detenida y recuperación con los cinco valores al reiniciarla.
- La captura visual del navegador agotó el tiempo de espera. Las interacciones y el DOM sí se comprobaron; no se equiparan a la validación visual del usuario.

### Límites y continuidad

- El usuario confirmó globalmente F0.2 y sus revisiones con "Todo Ok" el 26/10/2026 y autorizó el merge. No se dispone de un informe individual de cada prueba; esta aceptación no amplía las comprobaciones realizadas por el agente. F0.1 conserva su validación previa.
- Sin pruebas unitarias nuevas ni validación real de NAS/UNC.
- Un servidor de API: acceso al JSON serializado dentro del proceso; el último guardado correcto prevalece. No hay coordinación entre varias instancias ni detección de cambios de otros formularios.
- JSON escrito mediante temporal en el mismo directorio, flush a disco y renombrado; los errores se registran en el servidor y se notifican en la UI. Un JSON corrupto no se sobrescribe: hay que corregirlo o restaurarlo y volver a cargar.
- Las rutas guardadas se pueden cargar aunque sus carpetas hayan desaparecido, para poder corregirlas; se revalidan al guardar.
- No se impone todavía relación de contención entre las carpetas y la raíz M3U.
- Sin explorador, reproducción, exportación M3U, edición de playlists, BD ni Docker en esta fase.
- Servicios de comprobación y datos temporales retirados al acabar; el usuario debe iniciar sus proyectos desde Visual Studio.
- Las revisiones posteriores a la entrega y sus comprobaciones están registradas abajo; quedaron incluidas en la confirmación global del usuario.

## Revisión: restaurar campos de texto (26/10/2026)

Petición del usuario: "No me gusta lo que has hecho con el botón examinar,
revierte los cambios y deja el input de texto plano anterior".
Se revirtieron exclusivamente los tres commits del selector. En esa revisión, el código de `src`
quedó igual a la entrega inicial de F0.2 (`2ff8296`); no se modificaron los ajustes
JSON locales ni se pierde la validación o persistencia anterior.
Las checklists del selector quedan anuladas; la checklist inicial de F0.2 fue confirmada posteriormente.

Preparación: Visual Studio 2026 con .NET 10, abrir `DjTracks.slnx`, perfil
**DJ Tracks (Api + UI)**, F5; UI `http://localhost:5050/settings`,
API `http://localhost:5247/api/settings`. Usar cinco carpetas existentes y
accesibles por la API, con escritura en playlists; no hace falta música ni BD.

Comprobado por el agente: coincidencia exacta del código con la entrega anterior
al selector y compilación completa sin errores ni advertencias.
La comprobación visual de esta reversión corresponde al usuario.

### Comprobaciones que repetir

- [x] Abrir Configuración: solo aparecen los cinco campos de texto y los botones Guardar rutas y Volver a cargar, sin Examinar ni diálogo.
- [x] Escribir rutas válidas, guardar y recargar: aparece el mensaje de éxito y se conservan las rutas.
- [x] Escribir una ruta inválida y guardar: error bajo el campo sin modificar el JSON. Volver a cargar recupera las rutas guardadas.

No hay funciones nuevas. Revisión incluida en la aceptación y autorización de merge posterior.

## Revisión: descripciones de las rutas (26/10/2026)

A petición del usuario se añaden textos de ayuda bajo Tracks (canciones por año
y estilo), Sessions (audio y tracklist por año), Remember (Cortas y Largas)
y Playlists (archivos JSON, requiere escritura), igual que el texto existente
de raíz M3U (rutas relativas para futuras exportaciones). Se conservan los
campos de texto sin selector, sus valores y el comportamiento de guardado.

Preparación: abrir `DjTracks.slnx` en Visual Studio 2026 con .NET 10,
perfil **DJ Tracks (Api + UI)** y F5; `http://localhost:5050/settings`
(API `http://localhost:5247`). No hacen falta música ni BD para revisar los textos.

- [x] Nueva: abrir Configuración y comprobar una descripción breve del uso bajo cada uno de los cinco campos, sin botones Examinar.
- [x] Repetir: escribir una ruta inválida y guardar; debe aparecer su error bajo el campo. Corregir y guardar rutas válidas: se conservan al recargar. Para esto se necesitan cinco carpetas existentes, con escritura en playlists.

Comprobación del agente: compilación completa sin errores ni advertencias;
reutilización del mismo `HelperText` que ya utilizaba raíz M3U.
Context7 no expone herramientas en esta sesión. Revisión incluida en la aceptación del usuario.
No cambia la API ni el JSON. Merge autorizado el 26/10/2026.

## Formato para futuras entregas

Registrar la fase y PR, el estado de validación, los cambios y las comprobaciones
que deben repetirse. Para cada comprobación indicar una acción y su resultado
esperado, junto con los requisitos de arranque y los límites conocidos.
Mantener las confirmaciones o incidencias del usuario para que otra sesión no
confunda implementación, validación y autorización de merge.
