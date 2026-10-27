# Validaciones manuales y continuidad

Este archivo guarda el estado de entrega para retomar el proyecto en otra sesión.
Las preferencias permanentes están en `../AGENTS.md` y las decisiones funcionales en `PLAN.md`.
Actualizar esta información en cada entrega, revisión o confirmación del usuario.
Consultar siempre el estado real de GitHub antes de actuar sobre un PR.

## Estado actual

- Planificación: PR #1 mergeado.
- F0.1: validada por el usuario el 26/10/2026, incluido el inicio conjunto; PR [#2](https://github.com/dgomezc/dj-tracks/pull/2) mergeado en `develop` (estado comprobado en GitHub).
- Continuación autorizada el 26/10/2026: "Validado. Continua". La pausa anterior queda levantada.
- F0.2: validada por el usuario el 26/10/2026, incluidas la restauración de campos de texto y las descripciones. Confirmación: "Todo Ok, mezcla el PR y continúa como dijimos en el proyecto principal". PR [#3](https://github.com/dgomezc/dj-tracks/pull/3) mergeado en `develop` el 26/10/2026 a las 11:30:56 UTC, commit `88571b9` (comprobado en GitHub).
- F1.1: **validada por el usuario el 26/10/2026** con "Funciona todo. Continua". Aceptación global de la entrega y autorización de continuación; no amplía las comprobaciones individuales del agente. Merge de su PR autorizado en el flujo de continuación.
- Rama: `fase/f1-1-explorador-tracks`, desde `develop` actualizado por fast-forward.
- Entrega F1.1: PR [#4](https://github.com/dgomezc/dj-tracks/pull/4) hacia `develop`; comprobar su estado real en GitHub antes de actuar.
- Continuidad confirmada: trabajar directamente en el checkout original, sin nuevos worktrees. Se han preservado los cambios locales preexistentes en los dos `Class1.cs`, sin incluirlos en los commits.
- Novedad: `/tracks` navega Año → Estilo → canciones reales, con nombre de fichero, título, artista, álbum y duración. `/settings` conserva los cinco inputs de texto con sus ayudas, sin selector.
- Revisión del 26/10/2026: el usuario rechazó el selector de carpetas y pidió volver al formulario anterior. Retirados los botones Examinar, el diálogo y el endpoint de navegación; se mantienen los cinco campos de texto, la validación y el guardado originales. La elección previa del selector queda revocada.
- **Refactor transversal Vertical Slices (27/10/2026): pendiente de validación del usuario.** Pedido por el usuario antes de F1.2. Diseño validado en `arquitectura/flujo-get-tracks.html` ("Cambia todas las llamadas"). Rama `dgomezc-refactor-vertical-slices` (worktree), PR hacia `develop`. F1.2 no empieza hasta mergear este PR.

## Refactor Vertical Slices + Result + FluentValidation (pendiente de validación)

Cambios: las tres llamadas existentes (`GET /api/library/tracks`, `GET /api/settings`,
`PUT /api/settings`) siguen el flujo UI → HTTP → endpoint → validador → handler →
Infrastructure → `Result` → HTTP. Se elimina `weatherforecast`. Rutas, JSON y
mensajes al usuario se mantienen; diferencias conocidas:

- Fichero de ajustes inaccesible: ahora 503 (antes 500). Mismo mensaje.
- Error de JSON de ajustes en `/tracks`: ahora muestra el mensaje común de ajustes
  («El JSON de ajustes es inválido. Corrígelo o restaura una copia…»).
- API caída en `/settings`: el mensaje ya no incluye «para confirmar los ajustes».
- Al guardar ajustes, si hay rutas vacías o relativas solo se muestran esos errores;
  las comprobaciones de disco (existe/legible/escribible) aparecen al corregirlos.

### Comprobaciones del agente (27/10/2026)

- `dotnet build DjTracks.slnx`: 0 errores, 0 advertencias.
- API con datos temporales: 200, 400 (validador y disco), 404, 409 y 500 con los
  mensajes esperados.
- UI en navegador: errores por campo en `/settings`, guardado correcto, navegación
  Año → Estilo → canciones, carpeta eliminada (404) y API detenida.

### Checklist del usuario

Preparación: abrir `DjTracks.slnx` de esta rama en Visual Studio 2026, perfil
**DJ Tracks (Api + UI)**, F5. UI `http://localhost:5050`.

- [ ] `/settings` carga las rutas guardadas.
- [ ] Vaciar un campo y guardar → alerta «Revisa las rutas indicadas.» y «Introduce una ruta.» bajo el campo.
- [ ] Ruta relativa (p. ej. `musica`) → «Introduce una ruta absoluta del servidor (local o de red).».
- [ ] Ruta absoluta inexistente → «La carpeta no existe o el proceso de la API no puede leerla.».
- [ ] Rutas correctas → «Rutas validadas y guardadas en el servidor.».
- [ ] `/tracks` lista años → estilos → canciones como antes, con tags y duración.
- [ ] Renombrar una carpeta de estilo y pulsar «Volver a cargar» dentro de ella → «La carpeta seleccionada ya no existe. Vuelve al nivel anterior.».
- [ ] Detener solo la API y recargar `/tracks` → «No se pudo contactar con la API…».
- [ ] Opcional: `http://localhost:5247/api/library/tracks?style=Tribal` → 400 «Selecciona un año antes del estilo.».

## F1.1: explorador de Tracks (validada por el usuario)

Aceptación global el 26/10/2026: "Funciona todo. Continua".
La checklist siguiente conserva los pasos entregados; no consta un informe individual
por comprobación. F1.2 será la siguiente fase y requerirá su propia validación.

Cambios: GET `/api/library/tracks`, con parámetros opcionales `year` y `style`
para recorrer los tres niveles. Acceso a ficheros en Infrastructure y lectura
con TagLibSharp 2.3.0, sin escribir tags. Se listan MP3 (también `.MP3`) del estilo
seleccionado; las carpetas se muestran con sus nombres reales, incluido
`Night Intrumental`. Años en orden descendente; estilos y canciones por nombre.
La UI incluye navegación a niveles anteriores, recarga, estados vacíos y errores.
No hay filtros F1.2 ni reproducción F1.3.

### Preparación

Abrir `DjTracks.slnx` del checkout original en Visual Studio 2026 (.NET 10).
Seleccionar **DJ Tracks (Api + UI)** y F5: API `http://localhost:5247`,
UI `http://localhost:5050/tracks`. No hace falta BD.
En `/settings`, comprobar que Tracks apunta a una carpeta accesible por el
proceso de la API, organizada como `Año/Estilo/Artista - Título.mp3`.
Los ajustes locales de F0.2 se han trasladado al checkout original y están
ignorados por Git; no es necesario volver a introducirlos si siguen vigentes.
Para las pruebas opcionales de tags ausentes o corrupción, usar copias desechables,
nunca modificar la biblioteca original. Para detener solo API, iniciar ambos
proyectos por separado con sus perfiles `http`.

### Checklist del usuario

- [ ] Abrir `/settings`: siguen las cinco rutas guardadas, sus ayudas y los inputs de texto, sin Examinar. No guardar si las rutas ya son correctas.
- [ ] Abrir `/tracks`: aparecen los años de la carpeta Tracks. Abrir un año y un estilo: solo se muestran sus MP3, con nombre, título, artista, álbum y duración coincidentes con los ficheros. No se reproducen al pulsarlos.
- [ ] Volver mediante el año y «Años», elegir otro estilo y pulsar «Volver a cargar»: cada nivel muestra su contenido actual. Recargar la página completa vuelve al listado de años.
- [ ] Comprobar un año sin estilos y un estilo sin MP3: aparece el estado vacío correspondiente. Si existe `Night Intrumental`, debe poder abrirse igual que cualquier estilo.
- [ ] Con copias de prueba: un MP3 válido sin tags muestra «Sin datos» donde falten; un MP3 dañado sigue listado con aviso sin impedir ver las demás canciones. Los ficheros no MP3 no aparecen.
- [ ] Detener solo la API y pulsar «Volver a cargar»: aparece el aviso de conexión sin listado anterior engañoso. Reiniciar API y volver a cargar: se recupera el nivel seleccionado. Comprobar que los paneles laterales y el reproductor provisional siguen visibles.

### Comprobaciones del agente

- Compilación completa con `dotnet build DjTracks.slnx`: 0 errores y 0 advertencias.
- API y UI en puertos temporales y JSON aislado; los ajustes privados migrados no se usaron ni modificaron durante las pruebas.
- MP3 sintéticos locales: lectura de título, artista, álbum y duración; MP3 sin tags y MP3 corrupto con aviso. Extensión mayúscula incluida, TXT excluido, año/estilo vacíos y alias de carpeta navegables. Las altas se reflejan en la siguiente petición.
- HTTP 400 para rutas absolutas, traversal y estilo sin año; 404 para selecciones inexistentes; 409 sin configuración; 500 con JSON corrupto; 503 con raíz inexistente. Respuestas sin rutas absolutas del servidor.
- Enlaces/junctions internos excluidos y no navegables. OpenAPI contiene el nuevo endpoint.
- Navegador interactivo: recorrido año → estilo → canciones, DOM con tags y aviso; captura visual con los datos sintéticos. Error de almacenamiento visible; API detenida muestra aviso de conexión. La aceptación visual y los datos reales corresponden al usuario.
- Servicios y datos sintéticos temporales retirados al acabar; iniciar los proyectos desde Visual Studio para la validación del usuario.

### Límites

- Lectura bajo demanda, sin índice, caché, watcher, paginación, filtros ni reproducción. «Volver a cargar» vuelve a leer disco; no hay actualización automática.
- Solo la estructura de dos carpetas y los MP3 directamente dentro del estilo; no se recorren subcarpetas ni enlaces internos. No se restringen los nombres de año/estilo para no ocultar carpetas existentes.
- Si un fichero no puede leerse, se conserva su nombre con aviso y se registra una advertencia en API. Un error al enumerar carpetas impide dar por válido un listado parcial y se muestra como error.
- No se ha comprobado NAS/UNC real ni se han creado pruebas unitarias nuevas.
- La documentación de TagLibSharp no estaba disponible en context7 (solo ofrecía otro proyecto, TagLibSharp2); se consultó el README oficial de `mono/taglib-sharp`. MudBlazor 9.11.0 y ASP.NET Core se consultaron en context7.

## Migración y limpieza del 26/10/2026

- Trabajo de F1.1 en el checkout original, sin crear worktree.
- Se comprobó el JSON privado preservado de F0.2 y se copió a
  `src/DjTracks.Api/App_Data/settings.json` del checkout original: el destino
  no existía, no se sobrescribieron datos distintos y Git confirma que está ignorado.
  Los valores y las ubicaciones privadas no se versionan.
- La sesión principal archivó F0.2 mediante la app tras confirmar la copia.
  `git worktree list` confirma retirado su worktree; F1.1 continuó activa.
- La sesión F0.1 sigue activa en su worktree. No puede autoarchivarse ni esta
  sesión es su creadora: queda pendiente que el usuario la archive desde la app
  tras el traspaso. No se ha borrado por shell.
- El worktree detached `dgomezc-ubiquitous-bassoon` no aparece entre las
  sesiones descubiertas. Su propiedad y sus datos no están verificados;
  no se ha inspeccionado ni eliminado, conforme al límite de la coordinación.
- No se han borrado otros elementos por suposición. La limpieza anterior
  no se considera completada mientras persistan estos pendientes.

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
