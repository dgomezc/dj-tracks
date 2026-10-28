# Arquitectura de DJ Tracks

Guía obligatoria para todo desarrollo nuevo. Describe la estructura, la nomenclatura y el
flujo que ya siguen las llamadas existentes (`GET /api/library/tracks`, `GET /api/settings`,
`PUT /api/settings`). El diseño detallado de una llamada completa está en
[`arquitectura/flujo-get-tracks.html`](arquitectura/flujo-get-tracks.html).

Principios: KISS y YAGNI. Vertical Slices simplificado: sin MediatR, sin repositorios
genéricos, sin clientes HTTP intermedios ni clases de errores por feature.

## 1. Proyectos y dependencias

| Proyecto | Responsabilidad | Depende de | Paquetes clave |
|---|---|---|---|
| `DjTracks.Domain` | Contratos compartidos API ↔ UI (records), `AppError`/`ErrorKind` | — | FluentResults |
| `DjTracks.Infrastructure` | Ficheros, tags, ajustes, onda y EF Core | Domain | TagLibSharp, FFMpegCore, Logging.Abstractions |
| `DjTracks.Api` | Minimal API en vertical slices | Domain, Infrastructure | FluentValidation (+ DependencyInjectionExtensions) |
| `DjTracks.UI` | Blazor Server + Blazor Blueprint; llama a la API por HTTP | Domain | BlazorBlueprint.Components, BlazorBlueprint.Icons.Lucide |

La UI **nunca** referencia Infrastructure ni la API: solo se comunica por HTTP.

## 2. Flujo de una llamada

```
Página .razor ─► code-behind .razor.cs ─► Http.GetForResultAsync / PutForResultAsync (UI/Shared)
      ─── HTTP ───►
Program.cs ─► grupo /api + ValidationFilter ─► <Caso>RequestValidator (FluentValidation)
      ─► <Caso>Endpoint.Handle ─► <Caso>Handler.HandleAsync ─► Infrastructure
      ◄─ Result<T> ◄─ ToHttpResult() ─► 200 JSON | ProblemDetails
      ◄─── HTTP ───
Result<T> (AppError) ─► estado de la página (_listing / _error / _errors)
```

Qué hace cada capa:

| Capa | Responsabilidad | No hace |
|---|---|---|
| Página (`.razor`) | Marcado y binding | Lógica ni HTTP |
| Code-behind (`.razor.cs`) | Construye la URL, llama con las extensiones, pinta `Result` | `try/catch` HTTP, leer `ProblemDetails` |
| `HttpClientResultExtensions` | HTTP → `Result<T>`; errores de red/JSON → `AppError`; log | Traducir HTTP → `ErrorKind` |
| `ValidationFilter` | Ejecuta el validador de cada parámetro `*Request` → 400 | Reglas de negocio |
| `<Caso>RequestValidator` | **Formato** del request (obligatorio, absoluto, nombre de carpeta…) | Disco, ajustes |
| `<Caso>Endpoint` | Declara ruta y request; delega en el handler; `ToHttpResult()` | Lógica, `try/catch` |
| `<Caso>Handler` | **Negocio**: ajustes, disco, orquestar Infrastructure, logs; devuelve `Result<T>` | Conocer HTTP |
| Infrastructure | Acceso a ficheros/tags/ajustes; `Result` para fallos esperables | Mensajes HTTP |
| `UseExceptionHandler()` integrado | Excepciones **imprevistas** → log + 500 ProblemDetails | — |

## 3. Estructura de carpetas y nomenclatura

```
src/DjTracks.Api/
├─ Program.cs                         registro explícito + grupo /api
├─ Shared/
│  ├─ ValidationFilter.cs             filtro único del grupo /api
│  └─ ResultExtensions.cs             Result<T> → IResult
└─ Features/<Feature>/                p. ej. Library, Settings, Streaming, Playlists, Stats
   ├─ <Feature>Feature.cs             Add<Feature>Feature() · Map<Feature>Feature()
   ├─ <Feature>Validators.cs          todos los validadores de la feature
   └─ <Caso>/                         p. ej. GetTracks, SaveSettings
      ├─ <Caso>.Endpoint.cs           record <Caso>Request + static class <Caso>Endpoint
      └─ <Caso>.Handler.cs            sealed class <Caso>Handler

src/DjTracks.Domain/                  records de contrato (TrackListing, PathSettings…) y AppError.cs
src/DjTracks.Infrastructure/<Área>/   p. ej. Library/TrackLibrary.cs, Settings/JsonSettingsStore.cs
src/DjTracks.UI/
├─ Shared/HttpClientResultExtensions.cs
└─ Components/Pages/<Página>.razor + <Página>.razor.cs
```

La forma de onda de Tracks se genera en Infrastructure mediante `IWaveformGenerator`
y FFMpegCore. El slice `Streaming/GetTrackWaveform` envía los picos en bloques
NDJSON durante la primera generación y la UI los dibuja al recibirlos; después
entrega la caché JSON de `Api/App_Data/Waveforms` en un solo bloque. La UI controla
el seek en `WaveformPlayer`; no accede al sistema de ficheros ni invoca FFmpeg.

La cola de reproducción se mantiene únicamente en la UI, en el servicio scoped
`AudioPlayerState`, mediante entradas `QueuedTrack` con identificador independiente
(sin repetir la URL de audio de un archivo). `PlaybackQueue` muestra y modifica esa
cola y se suscribe a sus cambios para actualizar la pista activa; `MainLayout`
enlaza controles y eventos de fin del audio. No accede a ficheros ni crea casos de
uso HTTP: reutiliza las URLs de streaming y onda resueltas desde Tracks. La cola
no es una playlist persistente; su estado se pierde al recargar la sesión.

La ficha `NowPlaying` se muestra en la zona principal del layout y se suscribe
al mismo estado para seguir la pista seleccionada, incluso al avanzar desde la
cola. Cada entrada conserva los tags básicos del listado (sin volver a leerlos
en la UI) y la URL de carátula. El code-behind obtiene `TrackArtwork` mediante
`GetForResultAsync`: el slice `Streaming/GetTrackArtwork`, con validación de los
mismos identificadores que el audio, resuelve el archivo mediante `TrackLibrary`
y lee la imagen incrustada con TagLibSharp. Prioriza `FrontCover`, o la primera
imagen si no hay portada frontal. Devuelve JSON con una URL de datos de imagen
(JPEG, PNG, GIF o WebP), o `dataUrl: null` si no existe ninguna. No carga imágenes
externas ni añade caché/persistencia. Los errores se presentan en la ficha sin
detener el audio; las respuestas atrasadas no sustituyen la portada de la pista
actual. Año/estilo son las carpetas de la biblioteca, no los tags del MP3.

Sessions usa `Library/GetSessions` para Año → carpeta de sesión → MP3 y
`Streaming/StreamSession` para el audio con Range. Ambos reciben identificadores
de carpetas/fichero, nunca rutas del cliente, y validan formato con FluentValidation.
`SessionLibrary` reutiliza la navegación, lectura de tags y resolución segura de
`TrackLibrary`: ambas estructuras tienen dos niveles de carpetas. El contrato
`SessionListing` distingue carpeta de sesión de estilo (el `Style` de sus archivos
es nulo). Las sesiones se ordenan por nombre descendente; con `yyyy-MM-dd - Nombre`
las recientes aparecen primero. No se interpreta ni exige una fecha válida.
Las carpetas sin fecha se admiten al mismo nivel y siguen el mismo orden por nombre.
`SessionListing.Tracklists` devuelve nombres de TXT candidatos de la carpeta actual
(vacío en los niveles de años/carpetas). `SessionLibrary` reconoce `Tracklist.txt`,
el nombre base de un MP3 o el nombre completo de la carpeta con `.txt`, y estos
dos últimos precedidos por `Tracklist - ` o seguidos por ` - Tracklist`.
La comparación es ordinal sin distinguir mayúsculas, se excluyen enlaces y no
se recorren subcarpetas. Se conservan todos los candidatos ordenados por nombre:
no se elige uno ni se interpreta su contenido en F2.1.

La página Sessions llama por HTTP desde su code-behind, sin acceso a ficheros.
Añade un MP3 a la cola mediante su URL de streaming, con `SessionFolder` en
`QueuedTrack` para identificar su procedencia en la ficha. URLs de carátula/onda
son opcionales: en F2.1 solo Tracks las proporciona. Al cambiar a una sesión se
retira la onda anterior y la ficha muestra la carpeta de sesión, no un estilo.
El streaming básico permite usar la cola mixta sin entradas inoperantes; la onda,
portada y lectura del contenido del `.txt` quedan fuera de F2.1. No se añaden playlists ni BD.

| Elemento | Nombre | Namespace |
|---|---|---|
| Caso de uso | Verbo + sustantivo: `GetTracks`, `SaveSettings`, `GetSessions`, `CreatePlaylist` | — |
| Request | `<Caso>Request` (record, propiedades anulables) | `DjTracks.Api.Features.<Feature>.<Caso>` |
| Endpoint | `static class <Caso>Endpoint` con `Map<Caso>(this RouteGroupBuilder api)` y `private static Handle(...)` | ídem |
| Handler | `sealed class <Caso>Handler` (constructor primario) con `HandleAsync(...)` | ídem |
| Validador | `<Caso>RequestValidator : AbstractValidator<<Caso>Request>` | `DjTracks.Api.Features.<Feature>` |
| Feature | `static class <Feature>Feature` | `DjTracks.Api.Features.<Feature>` |
| Respuesta | Record de Domain si la UI lo consume (`TrackListing`, `PathSettings`) | `DjTracks.Domain` |

## 4. Receta: añadir un caso de uso

1. **Domain**: crea el record de respuesta si la UI lo necesita.
2. **Infrastructure**: añade o amplía la clase del área. Devuelve `Result<T>` con
   `new AppError(...)` para fallos esperables (carpeta inexistente…); deja propagar el resto.
3. **Endpoint** (`<Caso>.Endpoint.cs`):
   ```csharp
   public sealed record GetSessionsRequest(string? Year);

   public static class GetSessionsEndpoint
   {
       public static void MapGetSessions(this RouteGroupBuilder api) =>
           api.MapGet("library/sessions", Handle).WithTags("Library");

       private static async Task<IResult> Handle([AsParameters] GetSessionsRequest request,
           GetSessionsHandler handler, CancellationToken ct) =>
           (await handler.HandleAsync(request, ct)).ToHttpResult();
   }
   ```
   Query string → `[AsParameters]`; cuerpo JSON → parámetro sin atributo (`SaveSettingsRequest request`).
   La ruta es relativa al grupo `/api` (sin barra inicial).
4. **Handler** (`<Caso>.Handler.cs`): `Task<Result<T>> HandleAsync(<Caso>Request request, CancellationToken ct)`.
   Carga ajustes con `JsonSettingsStore.LoadAsync` (si falla, `return settings.ToResult();`),
   aplica reglas de negocio y captura solo las excepciones de E/S esperables con log y mensaje propio.
5. **Validador** en `<Feature>Validators.cs`: solo formato. Las reglas auxiliares son métodos
   `private static bool Be…` dentro del validador que las usa.
6. **Registro** en `<Feature>Feature.cs`: `.AddScoped<<Caso>Handler>()` en `Add<Feature>Feature()` y
   `api.Map<Caso>();` en `Map<Feature>Feature()`. Una feature nueva añade una línea a cada bloque de `Program.cs`.
7. **UI**: en el code-behind, `[Inject] HttpClient Http` + `Http.GetForResultAsync<T>(url, Logger)`;
   si `result.IsFailed`, `_error = result.Errors[0].Message`; los errores por campo están en
   `((AppError)result.Errors[0]).FieldErrors`.
8. Añade la petición a `DjTracks.Api.http` y compila con `dotnet build DjTracks.slnx`.

Endpoints sin request (p. ej. `GET /api/settings`) no necesitan validador. Todo parámetro cuyo tipo
termine en `Request` **debe** tener validador: si falta, la API falla al construir el endpoint.

## 5. Errores

`AppError(ErrorKind kind, string title, string detail, IDictionary<string, string[]>? fieldErrors = null)`
(Domain) es el único tipo de error. Se crea in situ, donde ocurre el fallo; los textos son en español
y orientados al usuario (`detail` es lo que se muestra).

| `ErrorKind` | HTTP | Uso |
|---|---|---|
| `Validation` | 400 | Regla de negocio por campo (con `fieldErrors`), p. ej. carpeta inexistente al guardar ajustes |
| `NotFound` | 404 | Recurso que ya no existe (carpeta seleccionada) |
| `Conflict` | 409 | Estado previo incorrecto (ruta no configurada) |
| `Unavailable` | 503 | Almacenamiento o ficheros inaccesibles |
| `Unexpected` | 500 | Datos corruptos (JSON de ajustes inválido) |

Los errores de formato no usan `AppError`: los devuelve `ValidationFilter` como 400
`ValidationProblem` (título «Revisa los datos enviados.», `detail` = primer mensaje, `errors` por campo).

Criterio de dónde va cada comprobación: **formato** → validador · **negocio, disco, ajustes** →
handler/Infrastructure con `Result` · **imprevisto** → manejador integrado.

## 6. Logging

- Infrastructure registra los errores que traduce (p. ej. `JsonSettingsStore`).
- El handler registra sus `catch` y avisos de negocio (`LogWarning` por tags ilegibles).
- La UI registra en `HttpClientResultExtensions` (respuesta no 2xx, red, JSON).
- Usa plantillas estructuradas (`{Year}`), nunca interpolación.

## 7. Registro de servicios

- Ciclo de vida: Infrastructure con estado compartido → `Singleton` (`TrackLibrary`, `JsonSettingsStore`);
  handlers → `Scoped`; validadores → `Singleton` (escaneo con `AddValidatorsFromAssemblyContaining<Program>`).
- UI: `AddHttpClient("Api")` + `AddScoped(sp => factory.CreateClient("Api"))` para inyectar `HttpClient`.
- Sin escaneo por reflexión para handlers ni endpoints: todo explícito y navegable.

## 8. Interfaz: Blazor Blueprint y tema

- Componentes estilados de Blazor Blueprint (`Bb*`, paquete `BlazorBlueprint.Components`) directamente
  en `DjTracks.UI`; sin proyecto de componentes propio ni primitivas salvo necesidad real.
  Iconos con `<LucideIcon Name="..." />` (`BlazorBlueprint.Icons.Lucide`).
- Registro en `Program.cs` con `AddBlazorBlueprintComponents()`.
  `MainLayout` contiene `<BbPortalHost />`, necesario para popovers, selects y paneles (`BbSheet`).
- Temas: `wwwroot/theme.css` contiene las variables shadcn de varios temas de tweakcn. "Light Green"
  es el predeterminado (`:root` claro y `.dark` oscuro); el resto usa `:root[data-app-theme="<id>"]`
  y `:root[data-app-theme="<id>"].dark`. `--radius` lleva `!important` porque el `ThemeService` de
  Blueprint lo escribe en línea en `<html>`. Orden en `App.razor`: `theme.css` → `blazorblueprint.css` →
  `app.css`, y el script `theme-init.js` de Blueprint en `<head>` para aplicar el modo antes de pintar.
  Las fuentes de los temas se cargan desde Google Fonts en `App.razor`.
- Selector de tema en Configuración: `AppThemes` (`Shared/AppThemes.cs`) lista los temas. La elección
  se guarda en la cookie `djt-theme` y `App.razor` la renderiza en el servidor como `data-app-theme`
  (así no hay parpadeo y la navegación mejorada de Blazor no borra el atributo). `Settings.razor.js`
  cambia el atributo y la cookie al momento.
- Modo claro/oscuro: `BbDarkModeToggle` en la cabecera. Se guarda en `localStorage` (`bb-theme`);
  sin preferencia guardada se sigue la del sistema.
- Añadir un tema: genera sus bloques en `theme.css` desde `https://tweakcn.com/r/themes/<id>`
  (variables `theme` + `light` en el bloque base, `dark` en el `.dark`), añádelo a `AppThemes.All`
  y, si usa una fuente nueva, al enlace de Google Fonts. No hay que tocar componentes.
- Estilos propios en `wwwroot/app.css` usando solo variables del tema (`--primary`, `--muted-foreground`,
  `--border`, `--card`…). Las utilidades de Blueprint llevan prefijo `bb:` y no hay Tailwind en el
  proyecto: no uses clases de Tailwind en el marcado propio; crea clases en `app.css`.
- Textos y `aria-label` en español: pasa las etiquetas de los componentes (`AriaLabel`, `Placeholder`,
  `ClearLabel`, `LightLabel`…) cuando su valor por defecto esté en inglés.
- Documentación: servidor MCP de Blazor Blueprint configurado en `.mcp.json` (Visual Studio y Copilot).
  Se ejecuta con `npx` dentro de WSL (Ubuntu con Node mediante nvm). Con Node instalado en Windows
  se puede usar `"command": "npx", "args": ["-y", "@blazorblueprint/mcp@latest"]`.
