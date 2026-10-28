# Validaciones manuales y continuidad

Este archivo guarda el estado de entrega para retomar el proyecto en otra sesión.
Las preferencias permanentes están en `../AGENTS.md` y las decisiones funcionales en `PLAN.md`.
Actualizar esta información en cada entrega, revisión o confirmación del usuario.
Consultar siempre el estado real de GitHub antes de actuar sobre un PR.

## Estado actual

- Planificación: PR #1 mergeado.
- F0.1: validada por el usuario el 26/10/2026, incluido el inicio conjunto; PR [#2](https://github.com/dgomezc/dj-tracks/pull/2) mergeado en `develop` (estado comprobado en GitHub).
- Continuación anterior autorizada el 26/10/2026: "Validado. Continua". Sustituida por la pausa explícita de F1.2 indicada abajo.
- F0.2: validada por el usuario el 26/10/2026, incluidas la restauración de campos de texto y las descripciones. Confirmación: "Todo Ok, mezcla el PR y continúa como dijimos en el proyecto principal". PR [#3](https://github.com/dgomezc/dj-tracks/pull/3) mergeado en `develop` el 26/10/2026 a las 11:30:56 UTC, commit `88571b9` (comprobado en GitHub).
- F1.1: **validada por el usuario el 26/10/2026** con "Funciona todo. Continua". Aceptación global de la entrega y autorización de continuación; no amplía las comprobaciones individuales del agente. PR [#4](https://github.com/dgomezc/dj-tracks/pull/4) mergeado en `develop` a las 14:57:01 UTC, commit `e70aafc` (comprobado en GitHub).
- F1.2: **validada globalmente por el usuario el 26/10/2026**, incluida la multiselección, con "Perfecto, haz el PR pero de momento no sigas." No consta informe individual de cada comprobación; esta aceptación no amplía las pruebas del agente.
- El 27/10/2026 el usuario autorizó integrar el port en el checkout original y mezclar los PRs: "trae los cambios y deja los pr mezclados y las ramas estables". F1.3 quedó pausada en esa entrega y se reanudó tras la instrucción posterior "continúa".
- F1.3: **validada por el usuario el 27/10/2026** con «Todo ok, continúa». PR [#7](https://github.com/dgomezc/dj-tracks/pull/7) mergeado en `develop` el 27/10/2026 a las 10:34:24 UTC; merge commit `d86e1c4` (comprobado en GitHub).
- F1.4: **validada globalmente por el usuario el 27/10/2026** con «todo ok, continua», incluida la revisión de carga progresiva. Esta confirmación no amplía las comprobaciones individuales del agente. PR [#8](https://github.com/dgomezc/dj-tracks/pull/8) mergeado en `develop` a las 14:26:25 UTC; merge commit `1d43cc8`.
- F1.5: **validada globalmente por el usuario el 28/10/2026** con «Todo ok, continúa», incluidas las revisiones sin duplicados, indicador activo y panel fijo al hacer scroll. PR [#9](https://github.com/dgomezc/dj-tracks/pull/9) mergeado en `develop` a las 06:28:03 UTC, commit `b90f570` (comprobado en GitHub). Esta confirmación no amplía las comprobaciones individuales del agente.
- F1.6: **validada globalmente por el usuario el 28/10/2026** con «Todo ok. Mezcla el pr y de momento para.». Autorizado el merge del PR [#10](https://github.com/dgomezc/dj-tracks/pull/10) hacia `develop`. Esta aceptación no amplía las comprobaciones individuales del agente. **Trabajo pausado por petición expresa; F2.1 no iniciada.**
- Checkout original actualizado por fast-forward a `develop` después de integrar F1.2; el trabajo de F1.3 se desarrolla en su rama dedicada. La rama del port se conserva como historial; no se han borrado ni reescrito ramas.
- Entrega F1.2: PR [#5](https://github.com/dgomezc/dj-tracks/pull/5) **mergeado** en `develop` el 27/10/2026 a las 07:13:41 UTC, commit `8fc21f1` (comprobado en GitHub). Incluye el port y ambos historiales sin force-push. PR #6 también mergeado; no queda PR de F1.2 pendiente.
- Continuidad confirmada: trabajar directamente en el checkout original, sin nuevos worktrees. Se han preservado los cambios locales preexistentes en los dos `Class1.cs`, sin incluirlos en los commits.
- Novedad: `/tracks` conserva la navegación Año → Estilo y añade filtros plegables por año, estilo/grupo y subestilo. `/settings` conserva los cinco inputs de texto con sus ayudas, sin selector.
- Revisión del 26/10/2026: el usuario rechazó el selector de carpetas y pidió volver al formulario anterior. Retirados los botones Examinar, el diálogo y el endpoint de navegación; se mantienen los cinco campos de texto, la validación y el guardado originales. La elección previa del selector queda revocada.
- **Refactor transversal Vertical Slices: validado por el usuario el 27/10/2026** con "Perfecto, documenta toda la arquitectura actual… Después mezcla el PR y sigue con el siguiente paso". Arquitectura documentada en `ARQUITECTURA.md`. PR [#6](https://github.com/dgomezc/dj-tracks/pull/6) mergeado en `develop` (`c7c1508`). Diseño validado en `arquitectura/flujo-get-tracks.html` ("Cambia todas las llamadas").
- Port F1.2 recibido de `dgomezc-fase-f1-2-filtros` (`1354305` código, `c38cddb` documentación). Se integra mediante merge en la rama original: se conservan ambos historiales y se resuelven los conflictos de código con la versión adaptada a Result/FluentValidation. Sin PR duplicado ni reescritura de ramas. Cambios preexistentes y JSON privado conservados; no se borran worktrees.
- Comprobación de integración en checkout original (27/10/2026): `src` coincide con el port recibido; compilación completa con 0 errores y advertencias. API/UI aisladas con MP3 sintéticos: multiaño/multigrupo/multisubestilo, alias, ausencia de duplicados, resultados vacíos, explorador, errores 400/404, opciones, carga de ajustes y página Tracks correctos. No equivale a validación visual de biblioteca real. Servicios y fixtures temporales retirados después de comprobar.

## F1.3: reproductor básico

La tabla de Tracks permite iniciar una canción desde su fila. La barra inferior
muestra el título y artista y usa los controles nativos de audio del navegador para
play/pausa, volumen y posición. La API ofrece `GET /api/streaming/tracks` por
año, estilo y nombre de fichero (no acepta rutas locales del cliente), valida los
identificadores y transmite `audio/mpeg` con soporte HTTP Range para el seek.
La reproducción permanece fuera del alcance de la cola, la forma de onda y las
estadísticas.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: correcto, 0 errores y 0 advertencias.
- Biblioteca aislada y temporal: una petición `Range: bytes=4-9` devolvió 206,
  `Content-Range: bytes 4-9/256` y seis bytes; ruta inválida devolvió 400 y fichero
  inexistente 404.
- Navegador con la API y UI aisladas: navegación año → estilo → canción, botón
  Reproducir actualiza título y URL del audio, y los controles nativos aparecen en
  la barra inferior. La fixture era deliberadamente bytes sintéticos no decodificables:
  el agente no verificó sonido ni seek real con un MP3 válido; esa comprobación se
  confirmó en la validación manual del usuario abajo.
- No se accedió a la biblioteca real ni a `App_Data/settings.json`; los datos de
  comprobación fueron temporales y deben retirarse al cerrar la validación.

### Preparación y checklist del usuario

En Visual Studio 2026, abre `DjTracks.slnx`, selecciona el perfil **DJ Tracks
(Api + UI)** y pulsa F5. Abre `http://localhost:5050/tracks`. La ruta de Tracks
configurada debe estar disponible para el proceso de la API y contener archivos
MP3 bajo `Año/Estilo/`; no es necesaria una base de datos. No cambies ajustes ni
la biblioteca para esta comprobación.

- [x] Navega a un año y un estilo (o aplica filtros) y pulsa el icono Reproducir
  de una canción. Debe comenzar el audio y la barra inferior debe mostrar su
  título y artista.
- [x] En la barra inferior, pausa y reanuda: el sonido debe detenerse y continuar.
- [x] Cambia el volumen y usa la barra de tiempo para adelantar/retroceder; el
  sonido debe reflejar el volumen y la posición elegidos.
- [x] Reproduce otra canción desde otra carpeta o desde resultados filtrados: el
  reproductor debe cambiar al nuevo archivo y mantener sus controles operativos.
- [x] Comprueba que la barra no tapa el contenido en una ventana estrecha.
- [ ] Opcional: en las herramientas de red del navegador, una petición de audio
  con `Range` debe recibir 206 Partial Content durante el seek.

Validación manual del usuario: «Todo ok, continúa» (27/10/2026). El usuario
confirma la entrega F1.3 y autoriza mezclar el PR #7 antes de continuar con F1.4.

### Limitaciones conocidas

- Los controles y los mensajes de formato de audio dependen del navegador. La
  fase no añade una cola, onda, reproducción automática del siguiente track,
  estadísticas, ni soporte para Sessions/Remember.

## F1.6: ficha de «ahora suena» (validada por el usuario)

Confirmación global del 28/10/2026: «Todo ok. Mezcla el pr y de momento para.».
Se registra la validación y autorización de merge, sin dar por realizadas
comprobaciones individuales adicionales. No continuar a F2.1 hasta nueva petición.

La zona principal muestra la canción seleccionada sin sustituir el explorador.
La ficha sigue reproducción directa, anterior/siguiente, cambio automático,
shuffle, bucle y eliminación desde la cola. Muestra título (o fichero si falta),
artista, álbum, duración, fichero y año/estilo de las carpetas.

La portada procede únicamente de imágenes incrustadas en el MP3, preferentemente
la frontal. No hay búsquedas externas, imágenes de carpeta ni modificación de
tags. JPEG/PNG/GIF/WebP admitidos. Sin imagen se indica «Sin carátula»; un error
de lectura/formato muestra aviso y permite reintentar sin detener el audio.
Los tags básicos son la instantánea del listado al añadir la canción; si se
editan externamente, vaciar la cola y volver a cargar la biblioteca para renovarlos.
La ficha sigue la selección del reproductor; no indica si el audio está pausado.

### Preparación y checklist del usuario

Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx` en `fase/f1-6-ahora-suena`,
perfil **DJ Tracks (Api + UI)** y F5. UI `http://localhost:5050/tracks`, API
`http://localhost:5247`. Mantener la biblioteca ya configurada y FFmpeg por
`PATH` o `Waveform__BinaryFolder`; no se necesita base de datos. Para comprobar
todos los estados, usar MP3 con carátula incrustada y otros sin carátula/tags.

- [ ] Antes de reproducir, la ficha invita a seleccionar una canción. Añadir a
  la cola sin reproducir no debe seleccionar una ni cargar una portada.
- [ ] Reproducir un MP3 con carátula: imagen correcta, título/artista/álbum,
  duración y nombre de fichero correctos; año/estilo coinciden con las carpetas.
  La imagen no se deforma. No se pierde el listado ni los filtros.
- [ ] Probar un archivo sin portada y otro sin tags: «Sin carátula», «Sin datos»
  en los campos ausentes y nombre del fichero como título. Nunca queda la imagen
  de la canción anterior.
- [ ] Cambiar con anterior/siguiente, desde el panel y por fin automático:
  ficha, audio, onda e indicador de cola corresponden a la misma canción.
  Probar cambios rápidos y shuffle/bucles: no aparece una portada atrasada.
- [ ] Navegar a Inicio y volver: ficha y cola se conservan sin reiniciar audio.
  Quitar actual selecciona la siguiente ficha; vaciar vuelve al estado sin canción.
- [ ] Pausa/reanudación, volumen y seek siguen funcionando. Sin duplicados,
  icono activo actualizado y cola fija al hacer scroll.
- [ ] Ventana estrecha y títulos largos: carátula/datos legibles y sin tapar
  contenido ni últimos controles del listado. El panel izquierdo de playlists
  (todavía vacío) se repliega bajo el breakpoint mediano de MudBlazor y se
  mantiene visible en escritorio.
- [ ] Si ocurre un error de carátula, ver aviso y botón «Reintentar carátula»:
  el audio sigue independiente. Al solucionar el archivo y reintentar, se recupera.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: correcto, 0 errores y advertencias.
- Biblioteca/configuración aisladas con MP3 sintéticos: endpoint de portada PNG
  y JPEG devuelve 200 con URL de datos de imagen; sin portada devuelve 200 con
  `dataUrl: null`. MP3 corrupto devuelve 503, formato de imagen BMP no admitido
  400, identificador con ruta 400 y fichero inexistente 404. Audio Range conserva
  206 con `Content-Range` correcto y la onda sigue sirviendo NDJSON (200).
- Navegador: carátulas PNG/JPEG decodificadas y datos correctos; archivo sin tags
  muestra nombre y «Sin datos», sin conservar la imagen anterior. Añadir a cola
  no selecciona canción y no duplica. Anterior/siguiente, cambio desde panel,
  fin real del audio, bucle de cola, shuffle, cambios rápidos, quitar actual y
  vaciar mantienen la ficha coherente con el audio y el indicador de la cola.
- Error de lectura visible en la ficha; sustituir la fixture corrupta por un
  MP3 válido y pulsar Reintentar recupera su imagen. Los tags básicos siguen
  siendo la instantánea del listado, tal como se describe arriba.
- Navegar a Inicio conserva el mismo elemento audio y la ficha. Inspección visual
  a 393 × 768: ficha de 351 px, portada de 180 × 180 sin deformación y datos
  legibles en columna, con el panel izquierdo replegado. No se ha comprobado
  hardware de sonido ni biblioteca real. Validación de escritorio pendiente.
- No se generaron tests unitarios ni se modificaron ajustes/biblioteca privados.
  Los cambios ajenos en ambos `Class1.cs` se mantienen fuera de los commits.
  Los servicios y las fixtures temporales de esta comprobación se retiraron.

Validación global del usuario recibida. Merge autorizado; continuación a F2.1
expresamente pausada.

## F1.5: cola de reproducción (validada por el usuario)

Confirmación global del 28/10/2026: «Todo ok, continúa». Acepta la entrega y sus
revisiones y autoriza mezclar el PR #9 y continuar con F1.6.

La cola reside en `AudioPlayerState`, con alcance de sesión interactiva de la UI:
se conserva al navegar dentro de la aplicación, no se guarda en disco ni se
comparte con otras pestañas y una recarga completa inicia otra sesión.
No necesita un endpoint ni base de datos.

- «Añadir a la cola» agrega una entrada sin interrumpir el audio; si el archivo
  ya está en la cola, no lo añade ni reinicia la reproducción. «Reproducir» selecciona su entrada existente o
  la añade si no estaba; pulsarla de nuevo reinicia la canción.
- «Cola (N)» abre el panel derecho. Permite iniciar cualquier entrada, quitarla
  y vaciar la cola. Quitar la entrada actual reproduce la siguiente en el orden
  de reproducción; si era la última, reproduce la anterior disponible. Vaciar
  detiene el audio y retira la onda.
- Anterior/siguiente recorren la cola. Sin pista activa, siguiente inicia la
  primera. Sin bucle, los controles se deshabilitan en los extremos y al acabar
  la última canción se detiene la reproducción.
- Shuffle baraja el recorrido una vez, manteniendo la pista actual al principio.
  Cada entrada se recorre una vez antes de llegar al final. Las entradas nuevas
  se incorporan al final del recorrido. Desactivarlo recupera el orden de adición.
  El panel conserva siempre el orden de adición.
- Bucle alterna **no → cola → canción → no**. «Cola» vuelve al principio al
  terminar; «canción» repite la actual al terminar. Anterior/siguiente permiten
  cambiar de canción también con bucle de canción.

### Preparación y checklist del usuario

En Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx` en `fase/f1-5-cola`, elegir
**DJ Tracks (Api + UI)** y pulsar F5. Abrir `http://localhost:5050/tracks`
(API `http://localhost:5247`). Se necesitan al menos tres MP3 válidos bajo
`Año/Estilo/` en la biblioteca ya configurada y FFmpeg disponible para la API
mediante `PATH` o `Waveform__BinaryFolder`. No se necesita base de datos.

- [ ] Añadir tres canciones con el nuevo botón de cola: aumenta el contador,
  sin iniciar ni interrumpir el audio. Abrir «Cola (3)» y verificar las entradas.
- [ ] Reproducir una entrada del panel y probar anterior/siguiente: cambian
  audio, título, entrada resaltada y onda. Pausa, volumen y seek siguen funcionando.
- [ ] Con bucle desactivado, dejar acabar una canción: pasa a la siguiente; al
  acabar la última se detiene. Los controles se deshabilitan en los extremos.
- [ ] Activar shuffle y recorrer las tres entradas: no se repiten antes de llegar
  al final. Desactivarlo: vuelve el orden de adición.
- [ ] Bucle de cola: al acabar la última vuelve a la primera. Bucle de canción:
  al terminar se reinicia la misma; anterior/siguiente aún cambian de canción.
- [ ] Quitar una entrada no activa: no interrumpe el audio. Quitar la actual:
  reproduce otra disponible. Vaciar: desaparecen audio y onda; contador cero.
- [ ] Añadir varias veces el mismo archivo, desde explorador y filtros: solo hay
  una entrada y el contador no aumenta; no cambia la pista ni su posición.
  Archivos distintos con el mismo título sí pueden coexistir. Quitar el archivo
  permite volver a añadirlo. Navegar a Inicio y volver: se conserva la cola.
- [ ] Mantener la cola abierta al cambiar con anterior/siguiente y al acabar
  automáticamente una canción: el icono activo pasa inmediatamente a la nueva
  entrada sin cerrar ni reabrir el panel. Probar también shuffle y bucle de cola.
- [ ] En ventana estrecha, comprobar panel, cierre, controles y onda sin que el
  reproductor tape las últimas filas. Una recarga completa vacía la cola.
- [ ] Nueva revisión de scroll: con una lista larga y la cola abierta, desplazar
  la página verticalmente. El panel permanece fijo a la derecha; si la cola
  supera la altura disponible, se desplaza su contenido para acceder al final.
  Cerrar y volver a abrir debe seguir funcionando, también en ventana estrecha.

### Comprobaciones del agente

Revisión del 27/10/2026 solicitada por el usuario: no admitir archivos repetidos
y corregir el icono activo que quedaba desactualizado con el panel abierto.
La comprobación anterior de duplicados queda sustituida por la checklist nueva.
Repetir anterior/siguiente, avance automático, shuffle, bucle y quitar/vaciar;
son nuevas las comprobaciones de rechazo de duplicados y del indicador en vivo.

Segunda revisión del 27/10/2026: el usuario informa de que el panel sube con la
página. `PlaybackQueue` pasa al interior de `MudLayout` para que MudBlazor aplique
su posicionamiento fijo y desplazamiento interno. Incluida en la aceptación global.

- Segunda revisión comprobada: compilación en salida aislada, 0 errores y
  advertencias (la salida normal estaba bloqueada por la depuración activa,
  que no se detuvo). Navegador de 642 × 1020: con altura de página ampliada
  temporalmente desde las herramientas del navegador y scroll vertical de
  800 px, el panel conserva `position: fixed`, borde superior 0 y borde derecho
  alineado con el viewport. Simulando contenido largo, el panel admite scroll
  interno de 500 px sin moverse; cerrar y reabrir funciona sin error de circuito.
  No se modificaron datos ni ajustes; pendiente comprobar con una cola real larga.
- Revisión comprobada: compilación completa, 0 errores y advertencias. Navegador
  con dos MP3 sintéticos distintos y el mismo título: se mantienen dos entradas,
  añadir otra vez el mismo archivo no aumenta el contador ni recrea el audio ni
  cambia su posición. Tras quitarlo se puede volver a añadir.
- Con el panel abierto, el icono activo se actualiza al usar anterior/siguiente,
  al finalizar realmente el audio, con shuffle, al volver al inicio por bucle de
  cola y al quitar la pista actual. Vaciar retira las entradas y el audio, sin
  error de circuito. Pendiente repetir con la biblioteca real del usuario.
- Compilación completa correcta con 0 errores y 0 advertencias.
- Navegador con API, ajustes y tres MP3 sintéticos aislados: añadir sin iniciar,
  reproducción, anterior/siguiente, avance mediante evento de fin y fin real del
  audio, shuffle recorriendo tres entradas distintas, reinicio de la canción en
  bucle, vuelta al inicio con bucle de cola, quitar actual y vaciar. La aceptación
  de duplicados de la entrega inicial queda revocada por esta revisión.
- Onda renderizada y seek por clic conservados tras cambiar de pista; el circuito
  Blazor no muestra error. Inspección en ventana estrecha: reproductor de 167 px
  y reserva inferior de 300 px en el contenido.
- No se ha comprobado sonido con hardware ni la biblioteca real. No se generaron
  tests unitarios. Los procesos y datos sintéticos se retiraron tras comprobar.

## F1.4: onda interactiva de Tracks (validada por el usuario; PR #8 mergeado)

La API genera 1024 picos por canción decodificando PCM estéreo a 44,1 kHz con
FFmpegCore por pipe, sin escribir WAV intermedio. Durante la primera generación,
envía bloques NDJSON de 16 picos y la UI los pinta inmediatamente; al terminar
persiste el resultado completo en caché JSON en
`src/DjTracks.Api/App_Data/Waveforms`, que no se versiona. La caché se reutiliza
mientras coincidan longitud y fecha de modificación del MP3; una modificación
del archivo obliga a regenerarla. El endpoint de onda comparte la resolución
segura de ruta con el streaming y valida año, estilo y nombre de archivo.

En la barra inferior, la onda sigue el progreso de audio y permite buscar con
clic o con flechas; Inicio/Fin llevan al comienzo/final. Se conservan los
controles nativos de reproducción, pausa, volumen y tiempo.

### Comparación de generadores

Medición temporal con FFmpeg 9.0.2, MP3 sintéticos estéreo y 1024 picos:

| Duración | NAudio/NLayer | FFMpegCore por pipe | Diferencia máxima de pico |
|---|---:|---:|---:|
| 3 minutos | 2264 ms | 591 ms | 0,0000 |
| 1 hora | 28070 ms | 10422 ms | 0,0107 (media 0,0001) |

Se mantiene FFMpegCore por rendimiento y porque el pipe elimina los WAV
temporales. NAudio se descarta para producción; la diferencia máxima observada
en una de las 1024 celdas del MP3 de una hora fue 0,0107, con diferencia media
0,0001. No se distribuye FFmpeg con la aplicación.

### Requisitos y checklist del usuario

En Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx`, seleccionar **DJ Tracks
(Api + UI)** y pulsar F5. UI: `http://localhost:5050/tracks`; API:
`http://localhost:5247`. La biblioteca ya configurada debe ser accesible por la
API y contener MP3 válidos bajo `Año/Estilo/`. Instalar FFmpeg para Windows en el
equipo donde corre la API y añadir su carpeta a `PATH`; alternativamente, definir
`Waveform:BinaryFolder` (por ejemplo, la carpeta que contiene `ffmpeg.exe`) en
la configuración de la API. No hace falta base de datos. No modificar el JSON de
ajustes ni la biblioteca para esta comprobación.

- [ ] Reproducir una canción sin caché (idealmente un mix largo): aparecen bloques
  de la onda progresivamente mientras se genera, el indicador de avance progresa
  y los controles nativos siguen operativos. La reproducción del audio no espera
  a que termine el cálculo.
- [ ] Hacer clic en varios puntos de la onda: el audio salta a esas posiciones y la parte resaltada sigue el progreso. Con la onda enfocada, probar flechas, Inicio y Fin.
- [ ] Cambiar a otra canción: título, audio y onda corresponden a la nueva pista.
- [ ] Recargar la pista o reiniciar la API y volver a reproducirla: la onda aparece desde la caché; no se pierde el audio ni sus controles.
- [ ] Repetir con un mix de una hora o más: la onda cubre toda la duración, permite saltar a un punto lejano y el audio continúa desde allí.
- [ ] En una ventana estrecha, comprobar que la onda y los controles siguen visibles y no tapan el contenido.
- [ ] Si FFmpeg no está disponible, la reproducción básica debe seguir funcionando y la UI debe mostrar que no se pudo generar la onda; tras instalar/configurar FFmpeg, recargar debe generarla.

### Comprobaciones del agente y límites

- API con MP3 sintético de una hora, sin caché previa: llegaron 65 bloques
  NDJSON (64 bloques de 16 picos y el cierre); el primer bloque se recibió a los
  0,363 s, la generación acabó en 10,395 s y se cubrieron los 1024 picos. La
  siguiente petición usó la caché y llegó en un solo bloque (9 ms).
- UI en navegador con la fixture sintética de una hora: el canvas mostró primero
  una parte de la onda y los píxeles dibujados aumentaron en lecturas sucesivas
  hasta completar la forma; el indicador de avance desapareció al terminar.
- Los códigos HTTP de petición inválida y pista inexistente siguieron siendo 400
  y 404 antes de iniciar la respuesta en streaming.
- Incidencia de validación reportada por el usuario: se muestra el reproductor
  nativo y el mensaje de que no se pudo generar la onda. Se comprobó que FFmpeg
  no estaba disponible en el `PATH` del entorno de desarrollo; se instaló
  FFmpeg 9.0.2 y se configuró `Waveform__BinaryFolder` como variable de entorno
  local del usuario. La decodificación de un MP3 sintético a PCM se verificó
  correctamente. Falta reiniciar Visual Studio/API y repetir la comprobación
  visual con la biblioteca real.
- `dotnet build DjTracks.slnx`: correcto, 0 errores y 0 advertencias.
- Benchmark sintético a 44,1 kHz y 1024 picos: en 3 minutos NAudio tardó 2264 ms
  y FFMpegCore por pipe 591 ms; en 1 hora, 28070 ms y 10422 ms. Para una hora,
  diferencia máxima 0,0107 en una celda y media 0,0001.
- API aislada: MP3 válido devuelve 200 y 1024 picos; primer cálculo de un track de
  20 segundos en 2683 ms, caché repetida en 26 ms. Al sustituir el MP3 por otro
  de 9 segundos se regeneró la caché y devolvió la nueva duración.
- Generador de producto con fixture sintética de una hora (41,2 MB): 1024 picos,
  duración 3600,07 s, sin celdas vacías, cálculo en 13860 ms y caché repetida en
  49 ms. La fixture no representa una mezcla musical real.
- Errores/API: traversal 400, canción inexistente 404. Sin FFmpeg configurado,
  la onda devuelve 503 y el streaming conserva HTTP 206 para una petición Range.
- Navegador con dos MP3 sintéticos válidos: onda renderizada en la barra inferior,
  progreso sincronizado, clic al 25 % desplazó el audio a 1,75 s de 7 s y
  ArrowRight avanzó 5 s; cambiar de pista actualizó título, audio y onda. Los
  controles nativos siguieron visibles. No se verificó sonido con hardware.
- Las comprobaciones usaron API/UI y ajustes aislados en carpeta temporal. No se
  ha usado ni modificado el JSON privado ni la biblioteca real; la validación
  manual visual con la biblioteca real y un mix corresponde al usuario. Las
  fixtures, cachés, harness y binarios FFmpeg temporales se retiraron al terminar.
- No se han generado pruebas unitarias. La caché identifica cambios por tamaño y
  fecha de modificación; no calcula hash del contenido MP3. Requiere FFmpeg
  instalado/configurado en el servidor API; no incluye binarios ni trata Docker
  (F6.1).

## Refactor Vertical Slices + Result + FluentValidation (validado por el usuario)

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

## F1.2: filtros de Tracks (funcionalidad validada en PR #5; port de arquitectura)

La implementación funcional y su checklist fueron validadas globalmente por el
usuario en la rama `fase/f1-2-filtros`. El port sobre PR #6 se incorpora al mismo
PR #5, siguiendo `ARQUITECTURA.md`, por autorización del 27/10/2026.
La revisión con biblioteca real del port no se presenta como realizada. En esta
entrega, F1.3 se implementa en una rama separada, sin alterar la validación pendiente
del usuario sobre la biblioteca real.

El panel «Filtros» empieza plegado y ofrece multiselección de años, estilos/grupos
y subestilos. Los valores se mantienen como borrador hasta «Aplicar filtros»;
las alternativas dentro de un campo se combinan con OR y los campos entre sí con
AND. Un campo vacío significa todos. Day/Night son grupos separados de
Instrumental/Vocal; el alias `Night Intrumental` se clasifica como Night +
Instrumental. Estilos sin subestilo (por ejemplo Technohouse y Tribal) aparecen
como grupos propios; al combinarlos con Instrumental/Vocal no hay coincidencias.
Los resultados incluyen año y estilo reales, no duplican canciones ante valores
repetidos, y se puede recargar la combinación aplicada, limpiar y volver al
explorador Año → Estilo → canciones.

En el port, `GET /api/library/tracks/filter-options` y
`GET /api/library/tracks/search` son slices separados con handlers `Result`,
validación FluentValidation y llamadas desde `HttpClientResultExtensions` en el
code-behind. El endpoint original de exploración se conserva.

### Preparación y checklist para validar el port

En Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx`, seleccionar **DJ Tracks
(Api + UI)** y pulsar F5. UI: `http://localhost:5050/tracks`; API:
`http://localhost:5247`. Se usan las rutas Tracks ya guardadas, accesibles por
el proceso de la API, organizadas como `Año/Estilo/*.mp3`; no hace falta BD.
No modificar la biblioteca ni el JSON privado. Si faltan combinaciones para
probar, usar carpetas y archivos sintéticos desechables en una configuración
aislada. Para detener solo API, iniciar los dos proyectos por separado con sus
perfiles `http`.

- [ ] Abrir `/tracks`: «Filtros» está plegado y sigue funcionando Año → Estilo → canciones. Abrir el panel: años y grupos reales; Day/Night separados de Instrumental/Vocal.
- [ ] Aplicar un año + Day y todos los subestilos: solo canciones Day de ese año. Cambiar a Vocal y aplicar: solo Day Vocal. Repetir con Night e Instrumental; incluir `Night Intrumental` si existe.
- [ ] Elegir varios años, ambos grupos y ambos subestilos: incluir todas las combinaciones coincidentes, mostrando el año y estilo en cada fila, también con nombres de fichero repetidos.
- [ ] Aplicar Technohouse o Tribal y combinar después con Vocal: primero aparece solo el estilo elegido; al combinar con el subestilo incompatible, aparece «No hay canciones que coincidan con los filtros».
- [ ] Cambiar selectores sin aplicar: no cambia el listado ni el resumen efectivo. «Volver a cargar» conserva la combinación aplicada; «Aplicar filtros» toma el borrador. Plegar el panel conserva resultados/resumen. «Limpiar filtros y volver a años» borra selecciones y recupera el explorador.
- [ ] Con filtros aplicados, detener solo la API y recargar: error visible y sin listado anterior. Reiniciar API y recargar: se recupera la misma combinación. Recargar la página completa vuelve a Años con filtros plegados.

### Validación y límites

- Validación global original del usuario: «Perfecto, haz el PR pero de momento no sigas.» Se conserva como aceptación de la funcionalidad F1.2 en PR #5; no sustituye la revisión del port sobre PR #6.
- Comprobaciones del agente del port: `dotnet build DjTracks.slnx` terminó con 0 errores y 0 advertencias. API aislada con dos años y carpetas sintéticas: opciones y explorador 200; búsqueda vacía 10 canciones; años 2025/2026 + Day/Night + ambos subestilos 8 canciones, incluyendo el alias; Tribal 2 y Tribal + Vocal 0; subestilo inválido 400 y año eliminado 404. Las filas conservan año/estilo y un MP3 inválido muestra el aviso esperado.
- Navegador del port con API/UI aisladas: panel plegado inicialmente, filtros visibles al abrir, aplicación de 2025/2026 + Day/Night + Instrumental/Vocal (8 filas), cambio de borrador seguido de recarga conserva los filtros aplicados, aplicar el borrador reduce a 4 filas y limpiar vuelve al explorador. Aplicar campos vacíos devuelve los 10 resultados.
- Los datos y ajustes sintéticos se crearon en una carpeta temporal; no se usó ni modificó la biblioteca o el JSON privado. No se generaron pruebas unitarias.
- Pendiente del usuario: revisar en Visual Studio 2026 la experiencia sobre la biblioteca real siguiendo la checklist anterior. La confirmación global original se conserva, pero no se presenta como validación visual del port.
- Los resultados consultan los ficheros bajo demanda, leen tags en todas las canciones coincidentes y no usan índice, caché ni paginación.
- No incluye reproducción F1.3, watcher, persistencia de filtros ni actualización automática. No genera pruebas unitarias nuevas ni comprueba NAS/UNC real.
- Estado del port: compilación y comprobaciones aisladas completadas por la sesión de origen; validación manual sobre la biblioteca real pendiente. Integración y merge autorizados por el usuario; no avanzar a F1.3.

## F1.2: entrega funcional original (histórico anterior al port)

Cambios: panel «Filtros» plegado inicialmente, con tres desplegables de
multiselección (años, estilos/grupos y subestilos). Sin selección se incluyen todos.
«Aplicar filtros» combina valores mediante OR en cada campo y AND entre campos y muestra todas las
canciones coincidentes, aunque pertenezcan a distintos años o carpetas.
Los resultados añaden año y estilo reales para distinguir ficheros homónimos.
«Limpiar filtros y volver a años» reinicia el formulario y recupera el explorador.

- Day/Night se separan de Instrumental/Vocal. Solo Instrumental o Vocal incluye
  ambos grupos; solo Day o Night incluye sus dos subestilos.
- `Night Intrumental` se considera Night + Instrumental sin renombrar carpetas.
  Las carpetas y grupos se comparan sin distinguir mayúsculas en la clasificación.
- Technohouse, Tribal y otras carpetas se conservan como grupos propios, sin
  subestilo. Combinarlos con Instrumental/Vocal devuelve cero resultados;
  no se ignora ni cambia silenciosamente ninguna selección.
- Los años y grupos disponibles proceden de las carpetas de la biblioteca
  completa, no se reducen al cambiar otro selector. Se refrescan al abrir el
  panel y al recargar/aplicar con él abierto.
- Cambiar un selector no cambia el listado hasta pulsar Aplicar.
  «Volver a cargar» mantiene los filtros ya aplicados, no aplica el borrador.
  El resumen indica los filtros efectivos incluso con el panel cerrado.
  «Años» sale de los resultados y recupera la navegación; «Limpiar» borra también
  el borrador. Recargar la página completa reinicia el estado.
- GET `/api/library/tracks/filter-options` devuelve años y grupos.
  GET `/api/library/tracks/search` admite `year`, `group`, `substyle`;
  cada parámetro puede repetirse (`year=2025&year=2026`); omitirlo significa todos.
  Cada valor se valida y las selecciones repetidas no duplican canciones.
  El endpoint original de exploración se conserva.
  Se comparte la lectura de MP3 y el tratamiento de errores, sin índice ni caché.

### Preparación

Visual Studio 2026 con .NET 10: abrir `DjTracks.slnx` en el checkout original,
perfil **DJ Tracks (Api + UI)**, F5. UI `http://localhost:5050/tracks`;
API `http://localhost:5247`. Se usan las rutas ya guardadas en Configuración,
sin alterar el JSON privado. Tracks debe ser accesible por el proceso API y
tener la estructura `Año/Estilo/*.mp3`; no hace falta BD.
Para comparar combinaciones, disponer de al menos dos años y estilos Day/Night
con Instrumental/Vocal, más Technohouse o Tribal si existen en la biblioteca.
No crear ni modificar ficheros originales para probar: usar copias si hace falta.
Para detener solo API, iniciar ambos proyectos por separado con perfil `http`.

### Checklist del usuario (nueva de F1.2)

- [ ] Abrir `/tracks`: «Filtros» está plegado y sigue funcionando Año → Estilo → canciones. Abrir el panel: aparecen años y estilos reales, con Day/Night separados del subestilo.
- [ ] Aplicar un año + Day + todos los subestilos: solo canciones Day de ese año. Cambiar a Vocal y aplicar: solo Day Vocal. Repetir con Night e Instrumental: incluir `Night Intrumental` si existe.
- [ ] Elegir todos los años + todos los estilos + Instrumental (y después Vocal): incluir Day y Night del subestilo, de todos los años. Comprobar año/estilo en las filas, también si se repite el nombre del fichero.
- [ ] Seleccionar Technohouse o Tribal con todos los subestilos: solo ese estilo. Combinarlo con Vocal o usar un año/estilo sin coincidencias: aparece «No hay canciones que coincidan con los filtros», sin resultados anteriores.
- [ ] Modificar selectores sin aplicar: no cambia el listado ni el resumen efectivo. «Volver a cargar» conserva la combinación aplicada; Aplicar usa la nueva. Plegar el panel conserva resultados y resumen. «Limpiar filtros y volver a años» borra las selecciones y recupera el explorador.
- [ ] Con filtros aplicados, detener solo API y recargar: error visible, sin listado anterior engañoso. Reiniciar y recargar: se recupera la misma combinación. Recargar la página completa vuelve a los años con filtros plegados. Configuración, paneles y barra provisional no cambian.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: 0 errores y 0 advertencias.
- API/UI aisladas, sin usar ni modificar el JSON privado ni la biblioteca del usuario.
  MP3 sintéticos en dos años, año vacío, seis estilos convencionales, alias,
  carpeta no convencional con caracteres especiales, ficheros homónimos y un MP3 corrupto.
- 84 combinaciones de año, grupo y subestilo comparadas con el conjunto exacto
  esperado de año/estilo/fichero, incluidas combinaciones vacías, grupo en minúsculas,
  Instrumental independiente y alias. Orden de años descendente, TXT excluido,
  corrupto visible con aviso y respuestas sin rutas absolutas.
- Regresión del endpoint de exploración, opciones reales de carpetas, exclusión
  de junctions. Nuevos endpoints: 400 para rutas/traversal y subestilo inválido,
  404 para año inexistente, 409 sin ajustes, 500 con JSON corrupto y 503 con raíz inaccesible.
- Navegador interactivo con datos sintéticos: apertura y selección de desplegables,
  combinación año/Night/Instrumental con alias, Instrumental independiente de ambos
  años, cambio explícito al aplicar, recarga sin aplicar el borrador, resultados
  vacíos y limpieza al explorador. Captura visual del panel y resultados.
  Fallos de almacenamiento y API detenida visibles sin resultados anteriores;
  recarga completa vuelve a los años y pliega el panel.
  La aceptación con la biblioteca real corresponde al usuario.
- Servicios y datos sintéticos temporales retirados al acabar; iniciar los
  proyectos desde Visual Studio para validar. Sin cambios en el JSON privado.

### Límites

- Multiselección por campo, sin persistencia de filtros entre recargas completas.
  Los filtros usan nombres de carpetas, nunca tags.
- Sin reproductor F1.3, BD, watcher ni actualización automática. Consultar todos
  los años lee los tags de todos los MP3 coincidentes bajo demanda.
- Sin nuevas pruebas unitarias ni comprobación real de NAS/UNC.
- Las limitaciones de estructura, permisos y enlaces de F1.1 se mantienen.
  Un error de enumeración se comunica como error, no como listado parcial correcto.

### Revisión: multiselección (26/10/2026, validada globalmente)

Confirmación posterior: "Perfecto, haz el PR pero de momento no sigas."
El PR existente se mantiene abierto, sin merge ni comienzo de otra fase.
Las checklists conservan los pasos entregados, sin atribuir ejecución individual.

Petición: "Los filtros tienen que ser multiselección, es decir poder seleccionar
por ejemplo el año 2025 y 2026 a la vez". Sustituye la selección única de la
primera entrega en los tres campos; no amplía la fase a F1.3.
Los desplegables mantienen las opciones marcadas y permiten desmarcar valores
o vaciar el campo con su botón de limpieza. Un campo vacío significa todos.
Se mantienen Aplicar explícito, borrador separado y recarga de la combinación efectiva.

Comprobado por el agente: compilación con 0 errores/advertencias; 80 combinaciones
de selecciones múltiples comparadas con el conjunto exacto esperado, incluidos
años repetidos sin duplicados, grupos múltiples, ambos subestilos y vacíos.
Validación individual de valores inválidos, año inexistente y regresión del explorador.
Navegador interactivo: 2025 y 2026 + Day y Night + Instrumental y Vocal simultáneos,
desmarcar 2025, recargar sin aplicar el borrador, aplicar solo 2026, limpiar y aplicar
campos vacíos (todos). Datos sintéticos aislados; no se modificaron ajustes privados.

Requisitos de arranque: los mismos de F1.2 (Api + UI desde Visual Studio,
`http://localhost:5050/tracks`, API `http://localhost:5247`, biblioteca con dos años
y carpetas de estilos). No hace falta repetir toda F1.1. Repetir la checklist
F1.2 anterior usando la multiselección, y añadir:

- [ ] Marcar 2025 y 2026 a la vez, aplicar: aparecen canciones de ambos años y de ningún otro; el resumen muestra ambos.
- [ ] Marcar Day y Night junto con Instrumental: aparecen ambos grupos de los años elegidos, incluido el alias. Marcar también Vocal: aparecen ambos subestilos sin duplicados.
- [ ] Desmarcar un año sin aplicar y recargar: se conserva la combinación previa. Aplicar: desaparecen las canciones del año desmarcado.
- [ ] Vaciar un campo con su botón de limpieza y aplicar: ese campo deja de restringir. «Limpiar filtros y volver a años» vacía los tres; aplicar vacíos incluye toda la biblioteca.

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
