# DJ Tracks — Plan de desarrollo

Reproductor autoalojado de música electrónica (Tracks, Sessions, Remember). Se ejecuta y depura desde Visual Studio 2026. Docker se aborda al final.

## Decisiones tomadas

| Tema | Decisión |
|---|---|
| Frontend | Blazor Server interactivo (.NET 10, ya existente en `src/DjTracks.UI`) |
| UI | Blazor Blueprint (componentes `Bb*`), temas de tweakcn seleccionables en Configuración ("Light Green" por defecto) con modo claro y oscuro y selector; por defecto sigue la preferencia del sistema (sustituye a MudBlazor desde la migración posterior a F1.6) |
| Backend | ASP.NET Minimal API + Vertical Slices (`src/DjTracks.Api/Features/<Feature>`) |
| Proyectos | `Api`, `UI`, `Domain`, `Infrastructure` (ya creados en `DjTracks.slnx`) |
| Biblioteca musical | Sin BD: se lee del sistema de ficheros (carpetas = año/estilo) |
| Playlists | Ficheros JSON en disco (carpeta configurable). Simple, portable y exportable a m3u |
| PostgreSQL | Solo estadísticas (reproducciones). EF Core + Npgsql |
| Tags/carátula | TagLibSharp (lectura de tags y carátula incrustada) |
| Onda | FFMpegCore decodifica PCM por pipe, transmite los picos en bloques para pintar la onda progresivamente y cachea el JSON final bajo `App_Data/Waveforms`; requiere FFmpeg disponible en la API. NAudio se comparó y se descartó por menor rendimiento |
| Autenticación | Ninguna (red local/VPN) |
| Playlists automáticos "vivos" | Se guardan los filtros; se resuelven al abrirlos contra un índice de la biblioteca. El índice se refresca con `FileSystemWatcher` + reescaneo horario (`BackgroundService`) por si el watcher falla en redes compartidas |
| Audio | Endpoint con soporte de `Range` (seek) |
| Tests | No se generan; validación manual por fase |

## Convenciones de la biblioteca

- **Tracks**: `Año/Estilo/Artista - Título.mp3`. Estilos: `Day Instrumental`, `Day Vocal`, `Night Instrumental`, `Night Vocal`, `Technohouse`, `Tribal`. Estilo compuesto: grupo `Day`/`Night` + subestilo `Instrumental`/`Vocal`. (Se aceptará la errata `Night Intrumental` como alias.)
- **Sessions**: `Año\Nombre de sesión\Nombre.mp3`; la carpeta puede incluir fecha
  (`yyyy-MM-dd - Nombre`), pero no es obligatoria. Se ordenan por nombre descendente,
  sin inferir fechas para las carpetas sin fecha. Se admiten varios MP3 por carpeta.
  Tracklist junto al audio, con líneas `N. Artista - Track`: `Nombre.txt`,
  `Tracklist.txt`, `Tracklist - Nombre.txt` o `Nombre - Tracklist.txt`.
  `Nombre` puede ser el nombre base del MP3 o el nombre completo de la carpeta.
  La detección no distingue mayúsculas/minúsculas, tampoco en `.TXT`, y no busca
  en subcarpetas. Si coinciden varios TXT, se lee solo el primero del listado
  ordenado por nombre (ordinal sin distinguir mayúsculas; desempate ordinal).
  No se concatenan ni se solicita elegir: decisión del usuario del 28/10/2026
  («lo mejor es leer el primero que encuentre»). Otros TXT se ignoran.
- **Remember**: MP3 directamente bajo `Cortas\` y `Largas\` dentro de la raíz
  configurada de Remember, con esos nombres exactos. No hay año/estilo ni
  exploración recursiva. Se ignoran otros archivos, subcarpetas y enlaces.
- Filtros y playlists automáticos se basan en nombres de carpeta, no en tags.

Ejemplo válido de Sessions (los nombres de TXT son alternativas, no obligatorios):

```text
Sessions\
├── 2026\
│   ├── Nombre de la sesión\
│   │   ├── Nombre de la sesión.mp3
│   │   └── Nombre de la sesión.txt
│   └── Otra sesión\
│       ├── Otra sesión.mp3
│       └── Tracklist.txt
└── 2025\
    └── 2025-12-31 - Fin de año\
        ├── Fin de año.mp3
        └── Tracklist - Fin de año.txt
```

## Estructura de la API (vertical slices)

```
Features/
  Library/      (tracks, sessions, remember: listado y navegación)
  Streaming/    (audio, carátula, onda)
  Playlists/    (manuales, automáticos, export m3u)
  Stats/
  Settings/
```
Cada slice agrupa endpoint, request/response y lógica. `Domain` contiene entidades/enums (Estilo, Track, Session, Playlist); `Infrastructure` contiene acceso a ficheros, TagLib, ffmpeg/NAudio, EF Core.

Flujo de cada llamada (guía completa y obligatoria en `ARQUITECTURA.md`; diseño detallado en `arquitectura/flujo-get-tracks.html`):

- **UI**: el code-behind de la página inyecta `HttpClient` y llama con `GetForResultAsync`/`PutForResultAsync` (`UI/Shared/HttpClientResultExtensions.cs`), que devuelven `Result<T>` con `AppError`. Sin clientes intermedios.
- **API**: `Program.cs` registra cada feature con `Add<Feature>Feature()` y la mapea con `Map<Feature>Feature()` dentro del grupo `/api`, que aplica `ValidationFilter` (FluentValidation) a todo parámetro `*Request`; si falta su validador, la API no arranca ese endpoint.
- **Slice**: `Features/<Feature>/<Caso>/<Caso>.Endpoint.cs` (request + mapeo; solo delega) y `<Caso>.Handler.cs` (lógica; devuelve `Result<T>` de FluentResults). Validadores en `<Feature>Validators.cs`.
- **Errores**: `AppError` + `ErrorKind` (Domain) creados donde ocurren; `ToHttpResult()` los traduce a HTTP. Los imprevistos los atiende el `UseExceptionHandler()` integrado (500 ProblemDetails).

## Fases

Cada fase termina en algo visible y probable en el navegador. Validación manual al final de cada una.
El estado de entrega y las checklists se mantienen en `VALIDACIONES.md`. Una implementación no se considera validada hasta la confirmación del usuario.

### Bloque 0 — Base
- **F0.1 Layout**: MudBlazor, tema oscuro (migrado después a Blazor Blueprint con modo claro/oscuro), layout con panel izquierdo (playlists, vacío), zona central, barra inferior de reproductor (vacía), pestañas Tracks / Sessions / Remember, Inicio y Configuración. UI llama a la API vía `HttpClient`. Perfiles de lanzamiento múltiples en VS (Api + UI).
  *Prueba*: navegar entre páginas, layout correcto.
- **F0.2 Configuración de rutas**: página Configuración que edita rutas (tracks, sessions, remember, playlists, ruta raíz m3u) guardadas en un JSON de ajustes; validación de que existen.
  *Prueba*: guardar rutas y ver mensaje de validación.

### Bloque 1 — Tracks
- **F1.1 Explorador de Tracks**: árbol/listado Año → Estilo → canciones (nombre de fichero, tags básicos).
  *Prueba*: navegar y ver el listado real.
- **F1.2 Filtros**: filtro compacto (desplegable) con multiselección de años, estilos/grupos y subestilos, con Day/Night y subestilos separables.
  *Prueba*: combinaciones de filtros devuelven lo esperado.
- **F1.3 Reproductor básico**: barra inferior con play/pausa, volumen, tiempo; reproducir al pulsar una canción (streaming con Range).
  *Prueba*: sonido y seek con barra simple.
- **F1.4 Onda**: FFMpegCore decodifica PCM por pipe y transmite progresivamente bloques de picos mientras genera la onda; el resultado final se cachea en JSON. La forma de onda interactiva permite buscar con clic o teclado y sigue el tiempo de reproducción. FFmpeg debe estar instalado o configurado para la API. Comparativa con NAudio completada: el generador por pipe fue más rápido en las muestras medidas.
  *Prueba*: onda visible, clic salta; probar con track y con mix de 1h+.
- **F1.5 Cola de reproducción**: añadir/quitar, anterior/siguiente, shuffle, bucle, panel de cola.
  Cola en memoria por sesión de la UI, sin archivos repetidos; avance
  automático al terminar, bucle de canción o de cola y recorrido aleatorio.
  *Prueba*: cada control por separado.
- **F1.6 Ficha de "ahora suena"**: carátula e información en la zona principal.
  Ficha sincronizada con la cola: título, artista, álbum, duración, fichero y
  año/estilo de las carpetas. Portada incrustada leída con TagLibSharp; ausencia
  de portada o tags indicada explícitamente, sin descargas de imágenes externas.
  *Prueba*: carátula y datos correctos.

### Bloque 2 — Sessions
- **F2.1 Listado y navegación de sesiones** (por año), añadir a cola.
  Navegación Año → carpeta de sesión → MP3 con tags básicos. Cola compartida con
  Tracks, sin duplicados; añadir no interrumpe el audio. Incluye streaming básico
  con Range para que las entradas se puedan reproducir desde la cola. Por petición
  expresa del usuario del 28/10/2026, incluye Play en el listado, onda progresiva
  con la misma caché de Tracks y carátula incrustada del MP3. No busca imágenes
  externas. El listado identifica los TXT compatibles, pero la lectura del
  contenido del tracklist sigue pendiente de F2.2.
- **F2.2 Tracklist** leído del `.txt` junto a la sesión e integrado con la reproducción.
  La reproducción con onda prevista inicialmente aquí se adelantó a F2.1 por
  petición expresa. «Ahora suena» muestra el nombre y texto del primer TXT compatible
  de la carpeta de la sesión activa, también al avanzar desde la cola y navegar.
  Conserva líneas, numeración y texto original, sin interpretar HTML ni exigir
  un formato de línea; no calcula tiempos ni permite saltar por canción (F2.3).
  Sin TXT, vacío o error se indican explícitamente; recargar tracklist no
  interrumpe el audio. Lectura UTF-8 (con/sin BOM) o Unicode con BOM, sin caché ni
  escritura de archivos; un TXT con codificación inválida pide guardarlo como UTF-8.
- **F2.3 (último, opcional) Marcas de inicio por canción**: marcar posición actual en el tracklist, guardar en JSON junto a la sesión, clic para saltar. Reproductor grande sincronizado en la página principal.

### Bloque 3 — Remember
- **F3.1 Remember**: selector Cortas/Largas, listado, reproducción y ficha del track.
  Cortas se abre inicialmente; ambas categorías muestran MP3 por nombre ascendente,
  tags y avisos individuales. Play y añadir a la cola compartida sin duplicados;
  streaming Range, onda progresiva/cacheada y portada incrustada como Tracks.
  Ficha y cola identifican Remember y su categoría, sin año/estilo ni tracklist.
  Vacíos y errores explícitos; sin playlists todavía.

### Bloque 4 — Playlists
- **F4.1 Manuales**: crear/borrar/renombrar, añadir y quitar canciones (botón en listados de Tracks y Remember), cargar en cola. Panel izquierdo.
  Gestión también en `/playlists` para pantallas sin panel lateral. Un JSON por
  playlist (`<id N minúsculo>.json`) en la ruta configurada, con nombre visible y
  referencias relativas a Tracks (año/estilo/fichero) o Remember (categoría/fichero),
  nunca Sessions, rutas absolutas ni URLs. Orden de incorporación y sin repetidos.
  Nombres de 1–120 caracteres, recortados y únicos sin distinguir mayúsculas;
  renombrar conserva ID y entradas. Borrar pide confirmación y solo elimina el JSON.
  Revisión del usuario (28/10/2026): crear y renombrar en ventana modal con
  Aceptar/Cancelar, y tooltip en todos los botones que solo muestran icono.
  Segunda revisión (28/10/2026): «Nueva playlist» (+) en la cabecera de la sección
  del panel izquierdo y lápiz para renombrar junto a cada nombre; en pantallas sin
  lateral se mantienen en `/playlists`. Cada canción de la playlist tiene
  Reproducir y Añadir a la cola, además de Quitar.
  Decisión del usuario del 28/10/2026: cargar **añade al final** de la cola, sin
  duplicados ni iniciar/interrumpir audio; no sustituye la cola. Resuelve tags
  actuales y reutiliza onda/portada de cada biblioteca. Si falta un MP3, muestra
  error sin modificar la cola; se puede quitar la referencia desde la playlist.
  Persisten tras recargar/reiniciar, a diferencia de la cola. Sin reordenación,
  exportación M3U ni playlists automáticos todavía.
- **F4.2 Export m3u** con la ruta raíz configurada.
- **F4.3 Automáticos**: modal "Generar playlist automático" con filtros Año (multi), Estilo (multi), Incluir Remember; guardado de filtros y generación; export m3u.
- **F4.4 Playlists vivos**: índice de biblioteca con watcher + reescaneo horario; los automáticos reflejan altas/bajas.
  *Prueba*: añadir/borrar un mp3 en carpeta y ver el cambio en el playlist.

### Bloque 5 — Estadísticas
- **F5.1 PostgreSQL y registro de reproducciones** (migración EF Core, conexión de desarrollo en el NAS por user-secrets).
- **F5.2 Dashboard**: más reproducidas, últimas reproducidas, resumen de biblioteca y playlists.

### Bloque 6 — Cierre
- **F6.1 Docker**: Dockerfiles (con ffmpeg si se usa), `docker-compose` (db, api, ui), despliegue en Ubuntu Server.
- **F6.2 (fase 2 del proyecto) Subsonic**: análisis de exponer API compatible para clientes Android.

## Aclaraciones confirmadas

1. Biblioteca de ~1000 mp3, crecimiento de 3-4 al mes: el índice en memoria es suficiente; no hace falta paginación ni BD para la biblioteca.
2. Los m3u usan rutas **relativas** (relativas a la ruta raíz configurada).
3. Los playlists manuales pueden mezclar Tracks y Remember.
4. Las sesiones no se pueden añadir a playlists manuales.
5. Las rutas se introducen mediante campos de texto, sin botón Examinar ni selector de carpetas (decisión del usuario del 26/10/2026, que revoca la propuesta del selector).
6. Los filtros de Tracks son multiselección en los tres campos: por ejemplo, 2025 y 2026 simultáneamente. Se aplica OR dentro de cada campo y AND entre campos; sin selección en un campo se incluyen todos (aclaración del usuario del 26/10/2026).
7. Cargar una playlist manual añade sus canciones al final de la cola sin duplicados
   ni interrumpir el audio (decisión del usuario del 28/10/2026).
