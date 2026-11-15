# DJ Tracks

Reproductor autoalojado de música electrónica para explorar y escuchar una
biblioteca propia desde el navegador. La música y las playlists viven en el
sistema de archivos; PostgreSQL se utiliza únicamente para las estadísticas.

> **Uso en red local o VPN:** la aplicación no incluye autenticación ni TLS.
> No expongas sus puertos directamente a Internet.

## Funcionalidades

- Biblioteca de **Tracks**, **Sessions** y **Remember**, con filtros, tags,
  carátulas y tracklists de sesiones.
- Reproducción con búsqueda, onda progresiva, cola, reproducción aleatoria y
  bucle. La cola se mantiene en memoria y se pierde al recargar la sesión.
- Marcas de sesiones guardadas por línea del tracklist, visibles y pulsables
  en la onda del reproductor.
- Playlists manuales y automáticas guardadas en archivos M3U. Las automáticas
  se actualizan contra un índice de la biblioteca al abrirlas o recargarlas.
- Historial y dashboard de escuchas: una reproducción cuenta tras 30 segundos
  efectivos. La música puede seguir sonando si PostgreSQL no está disponible.
- Temas seleccionables y modos claro y oscuro.

## Biblioteca

Configura cuatro rutas absolutas: Tracks, Sessions, Remember y `dj-tracks-data`.
Las tres bibliotecas son independientes y solo necesitan lectura.
La raíz de datos debe existir y permitir lectura/escritura: la API crea dentro
`playlists`, `session-marks` y `tracks-waves`. No se configuran por separado
ni desde la interfaz.

```text
Tracks/
└── 2026/
    └── Day Instrumental/
        └── Artista - Título.mp3
Sessions/
└── 2026/
    └── Sesión/
        ├── Sesión.mp3
        └── Tracklist.txt
Remember/
├── Cortas/
│   └── Tema.mp3
└── Largas/
    └── Tema.mp3
dj-tracks-data/
├── playlists/
│   └── Mi playlist.m3u
├── session-marks/
└── tracks-waves/
```

Remember requiere las carpetas `Cortas` y `Largas`. Las playlists manuales
admiten Tracks y Remember, no Sessions. Los M3U relativos son portables si se
mantiene la relación entre carpetas; las rutas absolutas de Windows y las
diferencias de mayúsculas pueden requerir adaptación al pasar a Linux. Prueba
primero con copias. Consulta los formatos y reglas completos en el
[plan](docs/PLAN.md#convenciones-de-la-biblioteca).

## Desarrollo con Visual Studio

Requisitos: Visual Studio 2026, SDK .NET 10 y FFmpeg disponible para la API
en `PATH` o mediante `Waveform:BinaryFolder`. PostgreSQL es necesario para
guardar estadísticas, pero no para reproducir.

1. Abre `DjTracks.slnx` y selecciona el perfil **DJ Tracks (Api + UI)**.
2. Configura las rutas mediante user-secrets de la API. Crea antes la raíz
   de datos; sustituye estas rutas de ejemplo por las de tu equipo:

   ```powershell
   dotnet user-secrets set "Paths:Tracks" "D:\Music\Tracks" --project src\DjTracks.Api
   dotnet user-secrets set "Paths:Sessions" "D:\Music\Sessions" --project src\DjTracks.Api
   dotnet user-secrets set "Paths:Remember" "D:\Music\Remember" --project src\DjTracks.Api
   dotnet user-secrets set "Paths:Data" "D:\dj-tracks-data" --project src\DjTracks.Api
   ```

   Las opciones se leen una vez al arrancar: reinicia la API tras cambiarlas.
   No guardes rutas privadas en `appsettings.json`. Visual Studio no carga `.env`.
3. Para estadísticas, configura `ConnectionStrings:Statistics` mediante
   user-secrets del proyecto API. Por ejemplo, en una terminal PowerShell:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:Statistics" "Host=<servidor>;Database=djtracks;Username=<usuario>;Password=<contraseña>" --project src\DjTracks.Api
   ```

   Sustituye los valores por los de tu base de datos. No publiques credenciales.
   La API intenta aplicar las migraciones al arrancar.
4. Inicia con F5. Las bibliotecas deben estar disponibles sin guardar ajustes.
   [Configuración](http://localhost:5050/settings) conserva el selector de
   temas y **Probar conexión** de PostgreSQL, sin formulario de rutas.

La UI usa `http://localhost:5050` y la API `http://localhost:5247`. Para
compilar desde la raíz: `dotnet build DjTracks.slnx`.

## Despliegue con Docker en Ubuntu

Requisitos: Docker Engine y el plugin Docker Compose. Si la música está en
un NAS, monta y verifica las carpetas en Ubuntu antes de arrancar: Docker no
configura el montaje ni sus credenciales. Las bibliotecas necesitan permisos de
lectura y recorrido para UID 1654; `dj-tracks-data` necesita lectura/escritura.

1. En la raíz del proyecto, prepara la configuración:

   ```bash
   cp .env.example .env
   chmod 600 .env
   openssl rand -hex 32
   ```

   Edita `.env`: indica `TRACKS_PATH`, `SESSIONS_PATH`, `REMEMBER_PATH` y
   `DJ_TRACKS_DATA_PATH` con las cuatro rutas absolutas **del host**. Pega los
   64 caracteres generados en `POSTGRES_PASSWORD` y ajusta `UI_PORT` si es
   necesario. No compartas ni subas `.env`.
2. Construye y arranca los servicios:

   ```bash
   docker compose config --quiet
   docker compose up -d --build
   docker compose ps
   ```

3. Abre `http://IP_DEL_SERVIDOR:5050` desde la LAN/VPN: las bibliotecas ya
   están configuradas. En `/settings`, pulsa **Probar conexión**: debe
   mostrar la migración inicial aplicada y ninguna pendiente.

Compose crea una base PostgreSQL nueva, independiente de la de desarrollo,
y publica solo la UI. Los volúmenes conservan estadísticas y claves de Blazor;
M3U, marcas JSON y ondas se guardan bajo `DJ_TRACKS_DATA_PATH` del host.
Haz copias de seguridad y no uses `docker compose down -v` si deseas conservar
los datos. Cambiar la contraseña de `.env` no la cambia en una base ya creada.

Compose convierte `.env` en montajes y variables `Paths__*` de la API.
La API ve `/music/Tracks`, `/music/Sessions`, `/music/Remember` y
`/dj-tracks-data`, no las rutas del host. No lee `.env` ni `settings.json`.
Después de editar `.env`, ejecuta `docker compose up -d api` para recrearla
con la nueva configuración; `docker compose restart` no actualiza su entorno.

### Actualizar una instalación existente

1. Detén la API y conserva una copia de playlists, marcas, `.env` y del
   antiguo volumen `api-data`. No borres volúmenes.
2. Crea `dj-tracks-data` y copia los M3U existentes a `playlists` y los JSON
   de marcas a `session-marks`, manteniendo sus nombres. Da lectura/escritura
   al UID 1654 en la raíz y en los archivos copiados.
3. **Ajusta las rutas de canciones en las copias de los M3U manuales**:
   se resuelven desde la nueva carpeta `playlists`. En Docker, por ejemplo,
   `../Tracks/2026/...` pasa a `../../music/Tracks/2026/...`; Remember sigue
   la misma regla. Conserva cabeceras, nombres e IDs. Las rutas absolutas
   `/music/Tracks/...` y `/music/Remember/...` no cambian.
   Las automáticas se reconstruyen al consultarlas conservando sus filtros.
4. Sustituye `PLAYLISTS_PATH` y `SESSION_MARKS_PATH` por
   `DJ_TRACKS_DATA_PATH` en `.env`; reconstruye con
   `docker compose up -d --build`. En Visual Studio, usa las cuatro claves
   `Paths:*` anteriores.
5. Las ondas se regeneran al reproducir; opcionalmente copia los JSON de
   `App_Data/Waveforms` del antiguo volumen a `tracks-waves` para reutilizar
   la caché si no cambian las rutas de audio, tamaño ni fecha de los archivos.

No hay migración automática, lectura alternativa del JSON de ajustes ni
borrado de los datos antiguos. Comprueba playlists y marcas antes de retirar
las copias. Una raíz de datos ausente o sin permisos impide iniciar la API;
una biblioteca temporalmente desconectada devuelve un error al consultarla.

La sección F6.1 de la [guía de despliegue y validación](docs/VALIDACIONES.md)
incluye permisos, backups, compatibilidad de M3U y comprobaciones manuales.

## Proyecto y documentación

La solución usa Blazor Server, ASP.NET Core Minimal API, EF Core y PostgreSQL.
Se divide en `DjTracks.UI`, `DjTracks.Api`, `DjTracks.Domain` y
`DjTracks.Infrastructure`. Más información:

- [Arquitectura](docs/ARQUITECTURA.md)
- [Plan y fases](docs/PLAN.md)
- [Validaciones y continuidad](docs/VALIDACIONES.md)
