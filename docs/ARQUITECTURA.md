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
| `DjTracks.UI` | Blazor Server + MudBlazor; llama a la API por HTTP | Domain | MudBlazor |

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
