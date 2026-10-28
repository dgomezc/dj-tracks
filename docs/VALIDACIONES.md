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
- F1.6: **validada globalmente por el usuario el 28/10/2026** con «Todo ok. Mezcla el pr y de momento para.». PR [#10](https://github.com/dgomezc/dj-tracks/pull/10) mergeado en `develop` a las 06:39:27 UTC, commit `0d48c5c`. Esta aceptación no amplía las comprobaciones individuales del agente. La pausa posterior se levantó solo para F2.1.
- **Migración transversal MudBlazor → Blazor Blueprint (28/10/2026): validada por el usuario el 28/10/2026**, incluido el selector de temas («Funciona todo Ok. Mezcla el PR»). PR [#11](https://github.com/dgomezc/dj-tracks/pull/11) mergeado en `develop` (ver sección "Migración a Blazor Blueprint"). Las fases siguientes deben respetar `AGENTS.md` y `docs/ARQUITECTURA.md` (sección 8).
- PR #11 comprobado mergeado en `develop` (`948ad2a`), checkout original actualizado por fast-forward e instrucciones de Blueprint releídas.
- F2.1 **validada globalmente por el usuario el 28/10/2026** con «todo ok, continua»,
  incluidas las revisiones de estructura, Play, onda y carátula. Esta aceptación
  no amplía las comprobaciones individuales del agente. Autorizada la integración
  del PR [#12](https://github.com/dgomezc/dj-tracks/pull/12) y continuación con F2.2.
- PR #12 mergeado en `develop` el 28/10/2026 a las 09:59:32 UTC, commit `8bb2fd1`
  (comprobado en GitHub); checkout original actualizado por fast-forward.
- F2.2 en `fase/f2-2-tracklist`: lectura y presentación del tracklist de la sesión
  activa. **Validada globalmente por el usuario el 28/10/2026** con «Ok, sigue a
  la siguiente fase», incluida la revisión responsive. Esta aceptación no amplía
  las comprobaciones individuales del agente. Autorizada la integración del
  PR [#13](https://github.com/dgomezc/dj-tracks/pull/13). Continuación confirmada:
  F3.1 Remember; F2.3 opcional queda para el final.
- PR #13 mergeado en `develop` el 28/10/2026 a las 12:29:27 UTC, commit `afdc690`
  (comprobado en GitHub); checkout original actualizado por fast-forward.
- F3.1 Remember en `fase/f3-1-remember`, PR [#14](https://github.com/dgomezc/dj-tracks/pull/14)
  validada globalmente por el usuario el 28/10/2026 con «ok, continua».
  Esta aceptación no amplía las comprobaciones individuales del agente.
  Autorizadas su integración y la continuación con F4.1; F2.3 opcional
  sigue aplazada para el final.
- Eliminados ambos `Class1.cs` vacíos por petición expresa del usuario del 28/10/2026, en commit separado de F2.1. No quedan esos cambios locales pendientes.
- Checkout original actualizado por fast-forward a `develop` después de integrar F1.2; el trabajo de F1.3 se desarrolla en su rama dedicada. La rama del port se conserva como historial; no se han borrado ni reescrito ramas.
- Entrega F1.2: PR [#5](https://github.com/dgomezc/dj-tracks/pull/5) **mergeado** en `develop` el 27/10/2026 a las 07:13:41 UTC, commit `8fc21f1` (comprobado en GitHub). Incluye el port y ambos historiales sin force-push. PR #6 también mergeado; no queda PR de F1.2 pendiente.
- Continuidad confirmada: trabajar directamente en el checkout original, sin nuevos worktrees. Se han preservado los cambios locales preexistentes en los dos `Class1.cs`, sin incluirlos en los commits.
- Novedad: `/tracks` conserva la navegación Año → Estilo y añade filtros plegables por año, estilo/grupo y subestilo. `/settings` conserva los cinco inputs de texto con sus ayudas, sin selector.
- Revisión del 26/10/2026: el usuario rechazó el selector de carpetas y pidió volver al formulario anterior. Retirados los botones Examinar, el diálogo y el endpoint de navegación; se mantienen los cinco campos de texto, la validación y el guardado originales. La elección previa del selector queda revocada.
- **Refactor transversal Vertical Slices: validado por el usuario el 27/10/2026** con "Perfecto, documenta toda la arquitectura actual… Después mezcla el PR y sigue con el siguiente paso". Arquitectura documentada en `ARQUITECTURA.md`. PR [#6](https://github.com/dgomezc/dj-tracks/pull/6) mergeado en `develop` (`c7c1508`). Diseño validado en `arquitectura/flujo-get-tracks.html` ("Cambia todas las llamadas").
- Port F1.2 recibido de `dgomezc-fase-f1-2-filtros` (`1354305` código, `c38cddb` documentación). Se integra mediante merge en la rama original: se conservan ambos historiales y se resuelven los conflictos de código con la versión adaptada a Result/FluentValidation. Sin PR duplicado ni reescritura de ramas. Cambios preexistentes y JSON privado conservados; no se borran worktrees.
- Comprobación de integración en checkout original (27/10/2026): `src` coincide con el port recibido; compilación completa con 0 errores y advertencias. API/UI aisladas con MP3 sintéticos: multiaño/multigrupo/multisubestilo, alias, ausencia de duplicados, resultados vacíos, explorador, errores 400/404, opciones, carga de ajustes y página Tracks correctos. No equivale a validación visual de biblioteca real. Servicios y fixtures temporales retirados después de comprobar.

## F3.1: Remember (validada por el usuario)

`/remember` abre Cortas y permite cambiar a Largas. MP3 por nombre ascendente,
tags básicos, duración, avisos de tags ilegibles por archivo, vacío y errores
explícitos. Play inicia la entrada; añadir no interrumpe ni duplica. Cola
compartida con Tracks/Sessions; streaming Range, onda progresiva/cacheada y
portada incrustada del MP3. Ficha/cola identifican Remember y categoría sin
atribuir año/estilo de carpetas ni mantener tracklist anterior.

La raíz configurada contiene `Cortas\Audio.mp3` y `Largas\Audio.mp3` (nombres
exactos); MP3 directamente dentro de cada categoría, sin niveles adicionales.
Extensión `.MP3` admitida; otros archivos, subcarpetas y enlaces excluidos.
Una categoría inexistente muestra error y permite seleccionar la otra.
No se añaden playlists, filtros de Tracks ni base de datos.

### Arranque y checklist del usuario

Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx` en `fase/f3-1-remember`,
perfil **DJ Tracks (Api + UI)** y F5. UI `http://localhost:5050/remember`, API
`http://localhost:5247`. Mantener rutas Tracks/Sessions/Remember configuradas y
accesibles a la API; Remember apunta a la raíz que contiene Cortas y Largas.
Usar MP3 válidos; FFmpeg disponible para la API (PATH o `Waveform__BinaryFolder`)
para la onda, sin BD. Reiniciar Api/UI y recargar completamente la página
para renovar la cola en memoria.

- [ ] Abrir Remember: Cortas inicial; cambiar a Largas y volver; lista correcta,
  nombre ascendente, tags/duración o «Sin datos». Volver a cargar mantiene
  categoría. Archivos corruptos avisan por fila sin impedir ver los demás;
  vacíos muestran mensaje. Subcarpetas y TXT no se listan.
- [ ] Play inicia el MP3; audio/pausa/volumen/seek y onda interactiva operativos.
  Primera onda progresiva, después cacheada. Portada incrustada propia; sin
  imagen muestra «Sin carátula». Ficha muestra Remember · Cortas/Largas,
  sin año/estilo ni tracklist de Sessions.
- [ ] Añadir sin iniciar/interrumpir audio; repetir y Play después de añadir
  no duplican. Mismo nombre en Cortas/Largas/Tracks/Sessions puede coexistir.
- [ ] Cola mixta: anterior/siguiente y fin automático con panel abierto actualizan
  indicador, ficha, onda y portada/tracklist correspondientes. Shuffle/bucle,
  quitar actual y vaciar conservan comportamiento previo.
- [ ] Navegar Remember/Tracks/Sessions/Inicio conserva cola/audio. Explorar otra
  categoría no cambia lo que suena. Recarga completa vacía la cola.
- [ ] Claro/oscuro con Light Green y otro tema, ventana estrecha/lista larga:
  selector, tabla, controles, ficha y cola legibles sin tapar controles.
- [ ] Con datos de prueba, categoría o almacenamiento ausente: error visible,
  recargar y elegir otra categoría permiten recuperar; Configuración accesible.
  No modificar/borrar datos reales para provocar errores.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: correcto, 0 errores y advertencias; sin tests nuevos.
- API con MP3 sintéticos válidos: Cortas/Largas, orden ascendente, tags y aviso
  por corrupto, extensión `.MP3`, Year/Style nulos, TXT/subcarpetas excluidos.
  Range bytes=4-9: 206 y seis bytes. Portada PNG idéntica a la incrustada y null
  sin imagen. Primera onda 65 bloques NDJSON/1024 picos; caché un bloque completo.
- Categoría inválida, ruta/traversal y archivo no MP3:
  400; fichero ausente 404; portada corrupta 503. Categoría ausente y categoría
  junction devuelven 404 en los cuatro endpoints. Vacío 200, sin ajustes 409,
  JSON inválido 500, almacenamiento ausente 503. Datos/ajustes de prueba aislados.
- Regresión API de Tracks/Sessions: carátulas correctas y tracklist de Sessions
  conservado tras extraer helpers internos para reutilizar lectura/resolución.
- Navegador: Cortas inicial, cambiar categoría y recargar mantienen selección;
  vacío y error por carpeta desaparecida visibles, recuperación seleccionando
  Cortas. Añadir no inicia audio; repetir y Play tras añadir no duplican.
  Homónimos Cortas/Largas y otras bibliotecas coexisten. Explorar/añadir conserva
  elemento audio activo. MP3 con áñ, `&`, `+` reproduce correctamente.
- Play: audio real avanza, onda dibujada, portada cargada y ficha sin año/estilo.
  Largas sin imagen retira portada anterior. Clic en onda a 50 % busca a unos
  22,6 s de 45 s. Fin real del audio avanza con Cola abierta y procedencia
  Remember · Cortas/Largas correcta. Shuffle, bucle cola, quitar actual y vaciar
  probados; vaciar retira audio y panel de tracklist.
- Cola mixta: Sessions recupera su tracklist/portada/onda; Remember los sustituye
  sin mantener tracklist; Tracks conserva ficha de año/estilo y portada/onda.
  Navegar de vuelta a Remember conserva el mismo audio.
- Capturas inspeccionadas a 642 px en Light Green y Cyberpunk, claro/oscuro,
  con tags, corrupto y portada/ausencia: datos y controles legibles, sin
  desbordamiento de página. Temas aplicados temporalmente en DOM, modo con botón.
  Consultados MCP Blueprint DataTable y documentación .NET.
- No se ha comprobado sonido por hardware, biblioteca real ni todos los
  tamaños/temas. Servicios y biblioteca temporal retirados después de comprobar;
  ajustes privados y datos reales sin tocar.

Validación global del usuario recibida el 28/10/2026 con «ok, continua».

## F2.2: tracklist de la sesión activa (validada por el usuario)

La ficha «Ahora suena» carga el TXT compatible de la carpeta de la sesión que se
está reproduciendo, desde Play o Cola. Admite las cuatro variantes de nombre
documentadas en PLAN, carpetas con/sin fecha y mayúsculas/minúsculas.
Decisión confirmada en esta fase: si hay varios, leer el primero que se encuentre;
se usa el orden por nombre del listado, sin concatenar ni selector. El fichero
elegido se identifica en la ficha. La selección es por carpeta, compartida si hay
varios MP3 dentro de ella.

Se conservan líneas, números, títulos y texto original, incluidas líneas sin el
formato `N. Artista - Track`, sin interpretar HTML. Se lee UTF-8 con/sin BOM o
Unicode con BOM; codificaciones no válidas muestran aviso para guardar como UTF-8.
No se añaden marcas, edición, seek por canción, escrituras de TXT/JSON ni BD.
No hay caché: «Volver a cargar tracklist» obtiene cambios sin interrumpir audio.
Las checklists de F2.1 de abajo son históricas, previas a esta lectura.

### Arranque y checklist del usuario

Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx` en `fase/f2-2-tracklist`,
perfil **DJ Tracks (Api + UI)** y F5. UI `http://localhost:5050/sessions`, API
`http://localhost:5247`. Mantener ruta Sessions configurada y accesible, MP3
válidos bajo `Año\Nombre de sesión\Audio.mp3`, TXT compatibles junto al audio.
FFmpeg disponible para la API mediante PATH o `Waveform__BinaryFolder`; sin BD.
Reiniciar Api/UI y recargar completamente la página para renovar la cola en memoria.

- [ ] Reproducir una sesión con cada variante de TXT: `Audio.txt`, `Tracklist.txt`,
  `Tracklist - Audio.txt`, `Audio - Tracklist.txt` (también `.TXT` y nombres de
  carpeta). En «Ahora suena» aparecen nombre del TXT y texto con acentos,
  numeración y saltos de línea correctos; la onda/carátula siguen operativas.
- [ ] Si hay varios TXT compatibles, solo aparece el primero por nombre; no
  concatena contenido ni pide elegir. `Notas.txt` y TXT en subcarpetas no se leen.
- [ ] Sesión sin TXT muestra aviso de ausencia; TXT vacío muestra «El tracklist
  está vacío». Un TXT con codificación inválida muestra error y permite recargar;
  audio y onda siguen funcionando.
- [ ] Editar un TXT de prueba mientras se reproduce y «Volver a cargar tracklist»:
  muestra la modificación sin reiniciar el audio ni cambiar la cola/posición.
- [ ] Navegar Sessions/Tracks/Inicio conserva el tracklist de la sesión activa,
  no el de la carpeta explorada. Cambiar desde Cola, anterior/siguiente y fin
  automático actualiza el texto; cambios rápidos no dejan texto anterior.
  Reproducir un Track o vaciar cola retira el panel.
- [ ] Repetir claro/oscuro con Light Green y otro tema, ventana estrecha y texto
  largo: ficha legible, líneas ajustadas y scroll del tracklist sin tapar controles.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: 0 errores y advertencias. No se generaron tests.
- API con biblioteca temporal aislada y MP3 sintéticos válidos: cuatro variantes
  TXT, `.TXT`, UTF-8 con/sin BOM, acentos y saltos de línea exactos, UTF-16 con BOM,
  ausencia (null/null), vacío y primer candidato entre varios sin concatenar.
  Edición y recarga sin caché; TXT bloqueado devuelve 503 sin probar otro candidato.
  Codificación inválida y rutas inválidas/MP3 ausente en request devuelven 400;
  audio/carpeta ausentes 404; ajustes no creados 409 y JSON de ajustes inválido 500.
- Navegador: Play inicia audio y muestra contenido/nombre del TXT con onda.
  HTML incluido en el TXT se muestra como texto, sin crear elementos script.
  Ausencia, vacío y error de codificación visibles sin parar audio ni quitar onda.
  Tras corregir el TXT, recargar mantiene el mismo elemento audio y el tiempo
  avanza de 5 a 5,7 s; navegar a Inicio conserva audio y tracklist.
- Anterior/siguiente, cinco cambios rápidos de ida/vuelta y fin real del audio
  con Cola abierta actualizan texto para la entrada activa. Reproducir un Track
  oculta el panel; volver a sesión lo recupera. Vaciar cola retira audio y panel.
- Capturas inspeccionadas a 642 px en Light Green y Cyberpunk, claro/oscuro,
  con 80 líneas largas: texto legible, ajuste de línea, scroll interno y sin
  desbordamiento horizontal de página. Temas aplicados temporalmente en DOM;
  modo alternado con el control de cabecera. MCP Blueprint Card y documentación
  .NET de lectura/codificación consultados.
- No se ha comprobado biblioteca real ni sonido por hardware ni todos los
  temas/tamaños/codificaciones. No hay soporte específico de ANSI/Windows-1252:
  ante bytes no válidos se pide guardar como UTF-8. API/UI y datos temporales
  retirados; ajustes privados sin tocar.

Aceptación global del usuario registrada el 28/10/2026, incluida la revisión
responsive. Las checklists se conservan como detalle histórico de la entrega,
sin atribuir comprobaciones individuales adicionales.

### Revisión responsive del tracklist (28/10/2026)

Por petición del usuario, el tracklist aparece a la derecha de carátula/datos
cuando hay ancho suficiente dentro de «Ahora suena», y debajo en espacios
estrechos. Mantiene scroll interno y ajuste de líneas; Tracks sin tracklist
conserva todo el ancho para sus datos.

- [ ] Repetir con una sesión y TXT largo en ventana grande: tracklist a la derecha,
  datos a la izquierda, sin desbordamientos. Estrechar ventana: tracklist debajo,
  legible y con scroll interno. Probar claro/oscuro y otro tema además de Light Green.
- [ ] Reproducir un Track: sin columna vacía; volver a sesión restaura el tracklist.

Comprobado por el agente: compilación aislada con `--artifacts-path` sin errores
ni advertencias (la compilación normal estaba bloqueada por la depuración activa,
que no se detuvo). Edge headless con el CSS real y marcado sintético de la ficha:
columnas a 1200/820 px disponibles, apilado a 780/642/393/320 px, sin desbordamiento
horizontal y con scroll interno en todos los tamaños. Sin tracklist, datos ocupan
todo el ancho. El navegador integrado agotó el tiempo de respuesta; no se atribuye
una nueva inspección visual de biblioteca real ni de temas a esta revisión.

## F2.1: navegación de Sessions y cola (validada por el usuario)

`/sessions` permite explorar Año → carpeta de sesión → archivos MP3. Años y
sesiones ordenados por nombre descendente (con fecha `yyyy-MM-dd`, las más recientes
primero; sin fecha, por nombre, sin inferir cronología); MP3 por nombre ascendente.
La fecha en la carpeta es opcional. Admite varios MP3 en la misma carpeta,
extensión sin distinguir mayúsculas y nombres con espacios, acentos, `&` y `+`.
El `.txt` no se muestra ni interpreta todavía; la API identifica sus nombres
compatibles en `tracklists`. Las carpetas enlazadas y archivos
enlazados se excluyen igual que en Tracks; no hay exploración recursiva.

El botón añadir por archivo lo añade a la misma cola de Tracks sin iniciar/interrumpir
el audio; una URL ya presente no se duplica. Archivos homónimos de distinta
carpeta o biblioteca son entradas distintas. Desde Cola se puede reproducir,
saltar y quitar; la ficha muestra carpeta de sesión y datos básicos. Se incorpora
el streaming con Range, Play directo, onda progresiva y carátula incrustada
(ampliación expresa del 28/10/2026). La lectura del tracklist se deja para F2.2.
No busca carátulas externas ni es una playlist
persistente ni se guarda en BD. Añadir una carpeta completa no forma parte de F2.1.

### Preparación y checklist del usuario

Visual Studio 2026 (.NET 10), abrir `DjTracks.slnx` en `fase/f2-1-sesiones`,
perfil **DJ Tracks (Api + UI)** y F5. UI `http://localhost:5050/sessions`, API
`http://localhost:5247`. Mantener las rutas de Tracks y Sessions configuradas y
accesibles a la API; sesiones bajo `Año\Nombre de sesión\Nombre.mp3`
(fecha opcional en la carpeta).
No hace falta base de datos. FFmpeg debe estar disponible para la API también
para la onda de Sessions (PATH o `Waveform__BinaryFolder`); no es necesario
para explorar ni para el audio básico. Si cambia la variable de usuario,
reiniciar Visual Studio para heredarla.

- [ ] Abrir Sessions: años recientes primero; entrar en un año, una carpeta de
  sesión y ver solo sus MP3. Volver mediante las migas Años/año y «Volver a cargar»
  mantiene el nivel. Tracks y sus filtros siguen funcionando.
- [ ] Tags y duración correctos; tags ausentes muestran «Sin datos». Un MP3
  ilegible muestra aviso en su fila sin impedir ver/añadir los demás.
  Carpetas vacías muestran mensaje, sin error.
- [ ] Añadir dos MP3: aumenta contador sin arrancar ni interrumpir audio.
  Repetir el mismo desde Sessions no lo duplica. Mismo nombre en carpetas distintas
  o en Tracks y Sessions debe poder coexistir.
- [ ] Abrir Cola y reproducir una sesión: audio básico, pausa, volumen y seek
  operativos; título/datos y carpeta de sesión correctos. Onda y portada propias
  de la sesión, sin conservar las de Tracks; el tracklist aún no se muestra.
- [ ] Cola mixta Tracks/Sessions: anterior/siguiente y fin automático cambian
  correctamente, indicador activo actualizado con panel abierto. Al volver a un
  Track se recupera su onda/carátula. Probar shuffle/bucle, quitar actual y vaciar.
- [ ] Navegar entre Sessions, Tracks e Inicio conserva cola y audio. Recarga
  completa vacía la cola, como hasta ahora.
- [ ] Probar claro y oscuro en Light Green y al menos un tema distinto (por
  ejemplo Cyberpunk). Listado, migas, alertas, cola y ficha legibles; desplegar
  una lista larga, scroll y ventana estrecha sin tapar controles.
- [ ] Si la carpeta seleccionada desaparece o falla el almacenamiento, «Volver
  a cargar» muestra error y permite volver a Años/año; Configuración accesible.
  No cambiar ni borrar datos reales para provocar el error.

### Comprobaciones del agente en la entrega inicial (antes de ampliar onda/portada)

- `dotnet build DjTracks.slnx`: correcto, 0 errores y advertencias, incluida la
  versión final del panel. No se generaron tests unitarios.
- API con biblioteca sintética: años `2026,2025` y carpetas de sesión en orden
  descendente, MP3 en orden ascendente, `.MP3` admitido, `.txt` excluido, tags y
  aviso de archivo corrupto. Vacíos devuelven 200; sesión sin año/identificador
  con ruta y extensión no MP3 devuelven 400; carpeta/fichero ausentes 404;
  ajustes aún no creados 409, JSON de ajustes inválido 500, almacenamiento ausente
  503. Una junction se excluye y no permite abrir audio (404). Range bytes=4-9
  devuelve 206, Content-Range correcto y seis bytes.
- Navegador con MP3 sintéticos: navegación/migas/recarga, vacíos, tags y aviso
  de corrupto; añadir dos archivos sin iniciar audio y repetidos sin duplicar.
  URL con acento, `&` y `+` reproduce y permite seek. Fin real del audio avanza
  con la cola abierta y ficha actualizada. Mismo fichero en Tracks y Sessions
  coexiste; transición mixta anterior/siguiente/shuffle/bucle de cola retira y
  recupera la onda de Tracks sin errores de circuito. Añadir repetido conserva
  el mismo elemento audio y posición de 3 segundos.
- Carpeta sintética renombrada mientras se muestra: recargar presenta el error
  y Años permite recuperarse. Cierre del panel en español y procedencia
  `Sessions · año · carpeta` comprobados en la versión final.
- Capturas inspeccionadas a 642 px de ancho en Light Green y Cyberpunk, claro y
  oscuro: tabla, migas, textos y reproductor legibles sin desbordamiento de página.
  Cyberpunk se eligió desde Configuración; para comparar Light Green se cambió
  temporalmente el atributo de tema en el DOM de la prueba. No se han comprobado
  todos los temas ni tamaños, sonido por hardware ni biblioteca real.
- Se usó el MCP de Blueprint para DataTable y Sheet. No se tocaron los ajustes
  privados ni la biblioteca real; la eliminación de las clases vacías solicitada
  por el usuario se guarda en commit separado.
  Servicios y fixtures de comprobación retirados antes de la entrega.

Aceptación global del usuario registrada el 28/10/2026 con «todo ok, continua».
Las listas siguientes conservan el detalle de las entregas previas, no acreditan
comprobaciones individuales adicionales.

### Revisión: carpetas sin fecha y variantes de tracklist (28/10/2026)

Petición del usuario: admitir carpetas con y sin fecha, y tracklists llamados
`Nombre.txt`, `Tracklist.txt`, `Tracklist - Nombre.txt` o `Nombre - Tracklist.txt`.
`Nombre` puede coincidir con el MP3 sin extensión o con la carpeta completa.
Se admiten mayúsculas/minúsculas y varios candidatos simultáneos; no se selecciona
ni concatena ninguno automáticamente. Otros TXT y enlaces se excluyen. La detección
es por carpeta, no una asociación exclusiva por audio cuando hay varios MP3.
No se cambia el orden por nombre ni se implementa todavía la lectura de F2.2.

Comprobaciones nuevas (pendientes del usuario):

- [ ] En el arranque indicado arriba, abrir un año con carpetas `Nombre de la sesión`,
  `Otra sesión` y `2025-12-31 - Fin de año`: todas se pueden abrir, listar sus MP3,
  añadir a cola y reproducir. Repetir añadir no duplica.
- [ ] Con cada variante de TXT junto a un MP3, abrir
  `http://localhost:5247/api/library/sessions?year=2026&session=Nombre%20de%20la%20sesión`
  (adaptar año/nombre). `tracklists` incluye el nombre real de cada TXT compatible,
  también `.TXT`; varios simultáneos se conservan. `Notas.txt` y TXT en subcarpetas
  no aparecen. En la UI solo siguen apareciendo los MP3, sin tracklist visible.

Repetir navegación y reproducción con los nuevos nombres. La checklist anterior
de cola mixta y temas sigue pendiente; esta revisión no modifica componentes ni estilos.

Comprobado por el agente en esta revisión:

- `dotnet build DjTracks.slnx`: 0 errores y advertencias.
- API con biblioteca temporal aislada: carpetas con y sin fecha, orden por nombre,
  las cuatro variantes de TXT individualmente y coexistiendo, extensión `.TXT`,
  coincidencia por nombre completo de carpeta y por nombre de MP3 distinto al
  de la carpeta. `Notas.txt` y TXT en subcarpetas excluidos; `tracklists` vacío
  en los niveles de años/carpetas. Streaming Range 206 con seis bytes en las
  tres carpetas comprobadas. Los MP3 sintéticos de esta revisión son bytes de
  prueba: no se atribuye reproducción sonora ni validación visual nueva.
- Consultada documentación .NET mediante context7 para enumeración y comparación
  ordinal de nombres. Sin ajustes privados modificados; API temporal detenida
  y biblioteca temporal retirada después de comprobar.

### Revisión: Play, onda y carátula de Sessions (28/10/2026)

El usuario reporta ausencia de Play y onda y carátulas incorrectas. Se amplía
expresamente F2.1: botón Play directo junto a añadir, endpoints de onda progresiva
y carátula incrustada del MP3, conectados tanto al reproducir como al añadir.
Se reutilizan generador/caché y lectura de carátula de Tracks. Ausencia de imagen
muestra «Sin carátula», carga/error y reintento funcionan también para sesiones.
No se buscan JPG/PNG externos ni se modifica el audio. F2.2 conserva el tracklist.

Comprobaciones del agente:

- Compilación completa: 0 errores/advertencias. Sin tests nuevos.
- Biblioteca aislada con MP3 sintéticos de dos minutos: portada PNG extraída
  idéntica a la incrustada, ausencia de portada devuelve null, corrupto 503,
  rutas inválidas 400 y ausentes 404 en audio/onda/portada.
  Primera onda: 65 bloques NDJSON, 1024 picos y finalización; caché posterior:
  un bloque completo. La portada de Tracks sigue devolviendo la misma imagen.
- Navegador: Play inicia audio real (`paused=false`, tiempo avanza), imagen
  cargada y canvas con onda dibujada. Añadir repetido conserva elemento audio y
  contador; añadir otra sesión no interrumpe. Anterior/siguiente actualizan onda
  y portada; sesión sin imagen elimina portada anterior y muestra «Sin carátula».
  Seek desde onda a 25 % sitúa audio en unos 30 s de 120 s. Transición a Tracks y
  vuelta a Sessions conserva onda propia y portada/ausencia correspondiente.
- Capturas inspeccionadas de Cyberpunk en oscuro y claro: Play/añadir, portada,
  ficha y onda legibles. Tema cambiado temporalmente en DOM y modo con botón.
  No se valida sonido por hardware, biblioteca real ni imágenes externas.
  Servicios y biblioteca temporal retirados después de comprobar, sin tocar ajustes privados.

Checklist nueva y a repetir (pendiente del usuario):

- [ ] Detener y arrancar de nuevo **DJ Tracks (Api + UI)** en Visual Studio 2026,
  F5; recargar completamente `http://localhost:5050/sessions` para renovar también
  las URLs de la cola en memoria (se vacía). API `http://localhost:5247`.
  Ruta Sessions accesible, MP3 válidos y FFmpeg disponible para la API.
- [ ] Abrir año/carpeta y pulsar Play: inicia esa sesión sin abrir Cola; añadir
  sigue sin interrumpir. Play tras añadir no crea duplicados.
- [ ] Con una sesión larga sin caché, la onda se dibuja progresivamente y permite
  seek con clic/flechas; al volver a reproducir carga desde caché.
- [ ] Sesión con portada incrustada muestra su imagen; otra sin portada muestra
  «Sin carátula» y no la anterior. Cambiar rápidamente entre sesiones y Tracks
  no intercambia imágenes/ondas. Si falla la imagen, aparece aviso/reintento sin
  parar el audio. Las imágenes externas no se leen.
- [ ] Repetir cola mixta, anterior/siguiente y fin automático con el panel abierto:
  entrada activa, onda y carátula se actualizan juntas.
- [ ] Repetir claro/oscuro en Light Green y otro tema: ambos botones, onda y ficha
  visibles/legibles, incluida ventana estrecha.

Revisión aceptada globalmente por el usuario el 28/10/2026 con «todo ok, continua».

## Migración a Blazor Blueprint (transversal, tras F1.6)

Sustituye MudBlazor por los componentes estilados de Blazor Blueprint 4.1 (`Bb*`) en
`DjTracks.UI`, con el tema "Light Green" de tweakcn en `wwwroot/theme.css` (claro y oscuro)
y un selector de modo en la cabecera. Sin preferencia guardada se sigue la del sistema.
No cambia la API ni la lógica: se mantienen textos, `aria-label` y comportamiento de F0.1–F1.6.
Añade `.mcp.json` con el servidor MCP de Blazor Blueprint (ejecutado en WSL).

Equivalencias principales: barra superior y panel izquierdo con HTML y CSS propios;
cola → `BbSheet`; Ahora suena → `BbCard`; filtros → `BbCollapsible` + `BbMultiSelect`;
tabla de Tracks → `BbDataTable` sin buscador ni paginación; Configuración → `BbField`/`BbInput`;
progreso de onda → `BbProgress`; carga indeterminada → `BbSpinner`; iconos Lucide.
La forma de onda usa `--primary` y `--muted-foreground` y se redibuja al cambiar de modo.

### Comprobaciones del agente

- `dotnet build DjTracks.slnx`: correcto, 0 errores y 0 advertencias.
- API y UI arrancadas con los perfiles `http`: `/`, `/tracks`, `/settings`, `theme.css`,
  `blazorblueprint.css` y `theme-init.js` devuelven 200.
- Navegador integrado (sin biblioteca configurada en el worktree): navegación, botón de cola,
  selector de modo con etiqueta en español, filtros plegados por defecto, mensaje de error y
  enlace a Configuración en `/tracks`. En `/settings`, guardar vacío muestra la alerta y
  "Introduce una ruta." bajo los cinco campos (`aria-invalid`).
- Modo: sin `bb-theme` guardado se aplica el del sistema (oscuro en la prueba); al pulsar el selector
  cambia la clase `dark` de `<html>` y se guarda en `localStorage`. Radio del tema: 1rem.
- Servidor MCP probado desde WSL (Node 24 con nvm): responde y devuelve documentación de componentes.
- **No** se han podido tomar capturas: el aspecto visual, la reproducción real, la onda, la cola
  con canciones y los multiselect abiertos quedan pendientes del usuario.

### Preparación y checklist del usuario

En Visual Studio 2026, abre `DjTracks.slnx`, selecciona el perfil **DJ Tracks (Api + UI)** y
pulsa F5. Abre `http://localhost:5050`. Usa la biblioteca y los ajustes reales ya configurados.

1. **Tema por defecto**: en una ventana privada, el modo coincide con el del sistema operativo.
2. **Selector**: el botón sol/luna de la cabecera alterna claro/oscuro sin recargar; al recargar
   se conserva la elección. Colores verde lima de "Light Green" en botones primarios y bordes redondeados.
3. **Cabecera y layout** (F0.1): enlaces Inicio, Tracks, Sessions y Remember con el activo resaltado;
   panel izquierdo de playlists (se oculta por debajo de 960 px); barra inferior fija.
4. **Configuración** (F0.2): carga las rutas guardadas; un error de ruta aparece bajo su campo;
   "Guardar rutas" muestra el aviso verde; escribir de nuevo lo oculta; "Volver a cargar" funciona.
5. **Tracks** (F1.1/F1.2): navegación Años → Estilo → tabla con Fichero, Título, Artista, Álbum,
   Duración y acciones; avisos de tags en la columna Fichero. Despliega "Filtros": se cargan las
   opciones y los tres multiselect permiten elegir varios valores; "Aplicar filtros" añade las
   columnas Año y Estilo; "Limpiar filtros y volver a años" restablece todo.
6. **Reproductor y onda** (F1.3/F1.4): reproduce una canción; se ve la barra de progreso mientras
   se genera la onda; la onda se pinta en verde (reproducido) y gris (pendiente), se puede buscar
   con clic y flechas, y cambia de color al alternar el modo.
7. **Cola** (F1.5): "Cola (n)" abre un panel lateral derecho; añadir, reproducir, quitar, vaciar,
   aleatorio y bucle funcionan; el panel cubre la barra inferior y se cierra con X, Escape o fuera.
8. **Ahora suena** (F1.6): tarjeta con carátula o icono de disco, datos y aviso en ambos modos.
9. **Contraste**: textos legibles en ambos modos (cabecera, tabla, barra inferior, alertas).

### Limitaciones conocidas

- El servidor MCP necesita WSL con Ubuntu y Node instalado con nvm; con Node en Windows basta
  cambiar el comando de `.mcp.json` (ver `ARQUITECTURA.md`, sección 8).
- Algunos componentes de Blueprint traen textos internos en inglés; se han pasado en español los
  visibles y los `aria-label` usados.
- La fuente Inter se carga desde Google Fonts; sin conexión se usa la fuente del sistema.
### Revisión: selector de temas (28/10/2026)

Petición del usuario tras validar la migración: elegir el tema en Configuración, conservando
"Light Green" y añadiendo Radioactive, Cyberpunk, Agentica Node, Bugchud!, Cyberpook y ESLinks.

- Nueva tarjeta "Tema de la interfaz" en Configuración (`BbSelect`). El cambio es inmediato.
- Se guarda en la cookie `djt-theme` (por navegador). `App.razor` la lee y renderiza
  `data-app-theme` en `<html>`, por lo que se mantiene al recargar y al navegar sin parpadeo.
- El modo claro/oscuro sigue siendo independiente (botón de la cabecera) y funciona con todos los temas.
- Cada tema aplica sus colores, radio y fuente (Geist, Afacad, Space Grotesk, JetBrains Mono… desde Google Fonts).
- La onda se redibuja también al cambiar de tema.
- Se elimina el ajuste `DefaultRadius` de `Program.cs`: el radio de cada tema lleva `!important` en `theme.css`.

Comprobado por el agente: `dotnet build DjTracks.slnx` con 0 errores y 0 advertencias. En el
navegador integrado, el selector muestra los 7 temas. Al elegir Cyberpunk cambian `data-app-theme`,
la cookie, `--primary`, `--radius` (0.5rem) y la fuente (Geist). Tras recargar y navegar a Tracks se
mantiene, con modo oscuro y claro correctos (captura de Cyberpunk oscuro revisada).

Checklist del usuario (perfil **DJ Tracks (Api + UI)**, F5, `http://localhost:5050/settings`):

1. La tarjeta "Tema de la interfaz" muestra "Light Green" la primera vez.
2. Elige cada tema: la interfaz cambia al momento (colores, bordes y tipografía) sin recargar.
3. Con cada tema, alterna claro/oscuro con el botón de la cabecera: ambos modos se ven bien y son legibles.
4. Recarga (F5) y navega a Tracks e Inicio: se conserva el tema elegido sin parpadeo del anterior.
5. Con una canción sonando, cambia de tema: la onda adopta los colores nuevos.
6. Abre la cola y un desplegable de filtros: paneles y listas usan el tema elegido.
7. Vuelve a "Light Green": queda como antes del cambio.

No hace falta repetir el resto de comprobaciones de la migración.
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
