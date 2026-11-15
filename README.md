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
- Playlists manuales y automáticas guardadas en archivos M3U. Las automáticas
  se actualizan contra un índice de la biblioteca al abrirlas o recargarlas.
- Historial y dashboard de escuchas: una reproducción cuenta tras 30 segundos
  efectivos. La música puede seguir sonando si PostgreSQL no está disponible.
- Temas seleccionables y modos claro y oscuro.

## Biblioteca

Configura cuatro carpetas existentes: Tracks, Sessions, Remember y Playlists.
La API necesita leer las tres primeras y también escribir en Playlists.

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
Playlists/
└── Mi playlist.m3u
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
2. Para estadísticas, configura `ConnectionStrings:Statistics` mediante
   user-secrets del proyecto API. Por ejemplo, en una terminal PowerShell:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:Statistics" "Host=<servidor>;Database=djtracks;Username=<usuario>;Password=<contraseña>" --project src\DjTracks.Api
   ```

   Sustituye los valores por los de tu base de datos. No publiques credenciales.
   La API intenta aplicar las migraciones al arrancar.
3. Inicia con F5 y abre [Configuración](http://localhost:5050/settings).
   Guarda las cuatro rutas absolutas accesibles desde la API y, si usas
   PostgreSQL, pulsa **Probar conexión**.

La UI usa `http://localhost:5050` y la API `http://localhost:5247`. Para
compilar desde la raíz: `dotnet build DjTracks.slnx`.

## Despliegue con Docker en Ubuntu

Requisitos: Docker Engine y el plugin Docker Compose. Si la música está en
un NAS, monta y verifica las carpetas en Ubuntu antes de arrancar: Docker no
configura el montaje ni sus credenciales. Las bibliotecas necesitan permisos de
lectura y recorrido para UID 1654; la carpeta de playlists necesita escritura.

1. En la raíz del proyecto, prepara la configuración:

   ```bash
   cp .env.example .env
   chmod 600 .env
   openssl rand -hex 32
   ```

   Edita `.env`: indica las cuatro rutas absolutas **del host**, pega los
   64 caracteres generados en `POSTGRES_PASSWORD` y ajusta `UI_PORT` si es
   necesario. No compartas ni subas `.env`.
2. Construye y arranca los servicios:

   ```bash
   docker compose config --quiet
   docker compose up -d --build
   docker compose ps
   ```

3. Abre `http://IP_DEL_SERVIDOR:5050/settings` desde la LAN/VPN. Guarda las
   rutas **del contenedor** `/music/Tracks`, `/music/Sessions`,
   `/music/Remember` y `/music/Playlists`. Pulsa **Probar conexión**: debe
   mostrar la migración inicial aplicada y ninguna pendiente.

Compose crea una base PostgreSQL nueva, independiente de la de desarrollo,
y publica solo la UI. Los volúmenes conservan estadísticas, ajustes, ondas y
claves de Blazor; los M3U se guardan en la carpeta de playlists del host.
Haz copias de seguridad y no uses `docker compose down -v` si deseas conservar
los datos. Cambiar la contraseña de `.env` no la cambia en una base ya creada.

La sección F6.1 de la [guía de despliegue y validación](docs/VALIDACIONES.md)
incluye permisos, backups, compatibilidad de M3U y comprobaciones manuales.

## Proyecto y documentación

La solución usa Blazor Server, ASP.NET Core Minimal API, EF Core y PostgreSQL.
Se divide en `DjTracks.UI`, `DjTracks.Api`, `DjTracks.Domain` y
`DjTracks.Infrastructure`. Más información:

- [Arquitectura](docs/ARQUITECTURA.md)
- [Plan y fases](docs/PLAN.md)
- [Validaciones y continuidad](docs/VALIDACIONES.md)
