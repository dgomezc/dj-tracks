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
- Estado: implementada, **pendiente de validación manual del usuario**. No hacer merge ni iniciar F1.1 sin su confirmación y autorización.
- Novedad inicial: `/settings` consume la API y permite guardar las cinco rutas en JSON del servidor.
- Revisión solicitada el 26/10/2026: añadir botones para elegir carpetas. El usuario eligió explícitamente un selector de carpetas del servidor dentro de la página, en lugar de un diálogo nativo de Windows. Implementado en el mismo PR, pendiente de validación.

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

### Checklist del usuario (pendiente)

- [ ] Abrir Configuración: aparecen las cinco rutas y los paneles existentes; en la primera ejecución los campos están vacíos y no se crea JSON hasta guardar.
- [ ] Guardar cinco carpetas válidas: aparece «Rutas validadas y guardadas en el servidor». Recargar, navegar fuera y volver, y reiniciar Api + UI: se conservan las cinco rutas.
- [ ] Probar un campo vacío, una ruta relativa, una inexistente y una ruta a un fichero: aparece el error bajo el campo; no se modifica el JSON. «Volver a cargar» recupera lo guardado.
- [ ] Editar tras guardar: desaparece el aviso de éxito. «Volver a cargar» descarta los cambios; durante las operaciones los controles quedan bloqueados.
- [ ] Detener solo la API y pulsar «Volver a cargar»: aparece el aviso de conexión y se bloquea guardar. Reiniciar la API y volver a cargar: desaparece el error y regresan las rutas.
- [ ] Hacer copia del JSON; dejar temporalmente JSON mal formado y volver a cargar: aparece el aviso de JSON inválido, guardar queda bloqueado y el fichero no se sobrescribe. Restaurar la copia y volver a cargar: se recupera el formulario.
- [ ] Con copia del JSON, marcarlo de solo lectura e intentar guardar otra carpeta válida: aparece error de almacenamiento, sin éxito ni cambio del fichero. Quitar solo lectura y volver a cargar: se recuperan las rutas originales.
- [ ] Si se usan rutas NAS/UNC, repetir guardado con las rutas reales: deben ser accesibles con la identidad que ejecuta la API; las carpetas inaccesibles o playlists sin escritura deben dar error por campo.

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

### Revisión: selector de carpetas del servidor (26/10/2026)

Cada campo incluye un botón **Examinar**. Abre un diálogo MudBlazor que consulta
`GET /api/settings/folders?path=...`: unidades en Windows o raíz en otros sistemas,
subcarpetas ordenadas, navegación al padre y entrada directa de ruta absoluta.
Solo lista carpetas, no archivos ni su contenido; no carga música al navegador.
La ruta actual del campo se usa como punto de partida. Si está vacía muestra
unidades/raíz; si no es válida muestra el error y permite volver a unidades/raíz.
Una carpeta vacía se puede seleccionar. Una ruta con problemas o la pantalla
de unidades/raíz no se puede seleccionar. Elegir una carpeta solo cambia el
campo correspondiente; cancelar conserva todos los cambios anteriores del formulario.
El guardado sigue siendo explícito y mantiene las validaciones de existencia y permisos.

La preparación es la misma: Visual Studio 2026, perfil **DJ Tracks (Api + UI)**,
F5 y `http://localhost:5050/settings`; API `http://localhost:5247`.
Para comprobar la navegación, preparar una carpeta con dos subcarpetas,
una de ellas vacía, y un archivo cualquiera. Para probar una carpeta de red
en Windows, pegar la ruta UNC completa de un recurso compartido y pulsar **Ir**.
No se descubren equipos ni recursos compartidos automáticamente.

**Nuevas comprobaciones del usuario:**

- [ ] Pulsar Examinar en cada uno de los cinco campos: se abre el selector en la ruta del campo, o en unidades/raíz si está vacío; se identifican claramente las carpetas del servidor.
- [ ] Abrir una subcarpeta, pulsar Subir y Unidades / raíz; pegar una ruta absoluta y pulsar Ir: navega correctamente, solo muestra carpetas y permite elegir una carpeta vacía.
- [ ] Pulsar Seleccionar esta carpeta: solo se rellena el campo que abrió el selector; no guarda el JSON hasta pulsar Guardar rutas. Cancelar, después de navegar, no modifica ningún campo.
- [ ] Introducir en el selector una ruta relativa, inexistente o sin lectura y pulsar Ir: aparece un error visible, no permite seleccionar una carpeta anterior por error y permite recuperarse con Ir o Unidades / raíz. Al editar la ruta, Seleccionar queda bloqueado hasta abrirla con Ir.
- [ ] Con el selector abierto, detener solo la API y pulsar Ir: aparece un error y no permite seleccionar. Reiniciar la API y pulsar Ir o Unidades / raíz: se recupera sin cerrar la página.
- [ ] Si se usa NAS/UNC, pegar en el selector la ruta real del recurso compartido y pulsar Ir: debe listar las subcarpetas accesibles con la identidad de la API. La ruta sigue admitiendo edición manual.

**Comprobaciones que repetir tras esta revisión:**

- [ ] Elegir las cinco carpetas con Examinar, guardar y recargar/reiniciar Api + UI: mensaje de éxito y rutas conservadas.
- [ ] Tras guardar, elegir otra carpeta: desaparece el aviso de éxito; Volver a cargar descarta la selección no guardada. Una ruta inválida escrita manualmente sigue produciendo error por campo sin modificar el JSON.
- [ ] Con API caída o JSON de ajustes corrupto al cargar el formulario, tanto Guardar como Examinar quedan bloqueados hasta recuperar y volver a cargar. Hacer copia antes de modificar el JSON.

La checklist inicial sigue pendiente; esta revisión no implica validación de los
puntos anteriores ni autorización de merge.

**Comprobaciones adicionales del agente:**

- Compilación completa sin errores ni advertencias, sin generar pruebas unitarias.
- API: unidades y raíz, padre y subcarpetas, directorio vacío, carpeta oculta,
  rutas con espacios, Unicode, `#` y `&`; no aparecen archivos.
- Rutas relativas/inexistentes/fichero y ACL real denegando lectura: errores HTTP explícitos;
  permisos originales restaurados después de comprobarlos.
- Navegador interactivo: apertura y selección en los cinco campos, cancelación sin
  cambios, navegación a subcarpetas/padre/unidades, error de ruta relativa,
  selección deshabilitada en unidades o error, guardado explícito de los cinco valores,
  error con API detenida y recuperación al reiniciarla.
- Edición de la ruta del selector: no permite seleccionar el directorio anterior
  mientras no se abra la ruta escrita con Ir; navegación y elección de ruta con
  espacios, Unicode, `#` y `&` verificadas en el navegador.
- JSON corrupto al cargar el formulario: aviso visible y los cinco botones
  Examinar y Guardar bloqueados. Copia del JSON restaurada después.
- Seleccionar sin guardar no cambia el JSON; el guardado posterior conserva las
  cinco carpetas elegidas. Comprobación DOM sin desbordamiento horizontal en el
  tamaño de ventana utilizado; no sustituye la revisión visual del usuario.
- Context7 no expuso herramientas en esta revisión; se consultó la documentación XML
  incluida en el paquete MudBlazor 9.11.0 instalado para las firmas de diálogos.

### Límites y continuidad

- Falta confirmar la checklist del usuario y el arranque conjunto de esta fase en Visual Studio. F0.1 conserva su validación previa.
- Sin pruebas unitarias nuevas ni validación real de NAS/UNC.
- Un servidor de API: acceso al JSON serializado dentro del proceso; el último guardado correcto prevalece. No hay coordinación entre varias instancias ni detección de cambios de otros formularios.
- JSON escrito mediante temporal en el mismo directorio, flush a disco y renombrado; los errores se registran en el servidor y se notifican en la UI. Un JSON corrupto no se sobrescribe: hay que corregirlo o restaurarlo y volver a cargar.
- Las rutas guardadas se pueden cargar aunque sus carpetas hayan desaparecido, para poder corregirlas; se revalidan al guardar.
- No se impone todavía relación de contención entre las carpetas y la raíz M3U.
- El selector enumera carpetas accesibles por la identidad de la API, sin restringirlas a una biblioteca ya configurada. Como el resto de ajustes sin autenticación, usar solo en la red local/VPN de confianza.
- Comprobado en Windows; ejecución en Linux y NAS/UNC real pendientes. Las carpetas de red no se descubren: hay que introducir su ruta de acceso.
- Sin explorador musical, reproducción, exportación M3U, edición de playlists, BD ni Docker en esta fase.
- Servicios de comprobación y datos temporales retirados al acabar; el usuario debe iniciar sus proyectos desde Visual Studio.
- La revisión del selector y sus comprobaciones nuevas/repetidas están descritas arriba. F0.2 continúa pendiente de confirmación del usuario.

## Formato para futuras entregas

Registrar la fase y PR, el estado de validación, los cambios y las comprobaciones
que deben repetirse. Para cada comprobación indicar una acción y su resultado
esperado, junto con los requisitos de arranque y los límites conocidos.
Mantener las confirmaciones o incidencias del usuario para que otra sesión no
confunda implementación, validación y autorización de merge.
