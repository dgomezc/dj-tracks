# Despliegue de DJ Tracks con Docker y conexión Subsonic

Este tutorial describe una instalación en **Ubuntu Server** con Docker Compose.
Permite abrir DJ Tracks en un navegador y conectar un cliente Subsonic, como
Tempus, desde la misma red local o VPN.

> **Estado:** la integración Subsonic de F6.2 está pendiente de validación con
> Tempus y con un despliegue Docker real. La compatibilidad con otros clientes
> también depende de las funciones que soliciten.

## 1. Qué se pone en marcha

```text
  Navegador                     Tempus u otro cliente Subsonic
      |                                      |
      | http://IP_DEL_SERVIDOR:5050           | misma dirección
      +------------------+-------------------+
                         v
             +------------------------+
             | ui: web y proxy        |  único puerto publicado
             +-----------+------------+
                         | /rest → Subsonic; audio → streaming
                         v
             +------------------------+       +------------------+
             | api: DJ Tracks y       |------>| db: PostgreSQL   |
             | servidor Subsonic      |       | estadísticas     |
             +-----------+------------+       +------------------+
                         |
              Bibliotecas y datos del servidor
```

Compose inicia los tres servicios juntos: `ui`, `api` y `db`. **Subsonic
forma parte de `api`**; no hay otro contenedor ni un servidor que arrancar por
separado. La interfaz web y el cliente usan el puerto publicado por `ui`;
la API y PostgreSQL permanecen en la red interna de Docker.

> **Seguridad:** utilice únicamente LAN o VPN, restrinja el puerto a redes
> confiables y no lo publique directamente en Internet. El despliegue no incluye
> HTTPS; si lo necesita, debe configurar un proxy TLS independiente y seguro.
> Las credenciales Subsonic **no protegen la web ni el resto de la API**. Los
> tokens incluidos en las URL pueden filtrarse en clientes o logs y reutilizarse.

## 2. Antes de empezar

- Ubuntu Server con [Docker Engine y el complemento Compose](https://docs.docker.com/engine/install/ubuntu/)
  instalados y funcionando; `docker compose version` debe mostrar una versión.
- El código de DJ Tracks en el servidor, en una revisión que incluya F6.2.
  Actualmente la implementación está en la rama del
  [PR #24](https://github.com/dgomezc/dj-tracks/pull/24), pendiente de
  validación. Si todavía no tiene el repositorio, puede obtener esa rama:

  ```bash
  git clone --branch fase/f6-2-subsonic https://github.com/dgomezc/dj-tracks.git
  cd dj-tracks
  ```

  Si ya tiene el proyecto, sitúese en su directorio y seleccione la revisión
  adecuada sin sobrescribir cambios locales. Tras una integración futura, use
  la rama donde se haya incorporado.
- Una dirección IP o nombre de host del servidor accesible desde el navegador
  y el teléfono. En el servidor, `hostname -I` muestra sus direcciones; elija
  la correspondiente a la LAN o VPN. En el teléfono, `localhost` señala al
  propio teléfono, no al servidor.
- Las tres carpetas de música y una raíz de datos **existentes**. Si la música
  está en un NAS, monte las carpetas en Ubuntu antes de arrancar y compruebe
  que siguen montadas. Los enlaces no se exploran como archivos de biblioteca.

Un ejemplo de estructura de música compatible:

```text
Tracks/2026/Day Vocal/Canción.mp3
Sessions/2026/Mi sesión/Sesión.mp3
Remember/Cortas/Canción.mp3
Remember/Largas/Canción.mp3
```

`Remember` necesita ambas carpetas con esos nombres exactos. La API lee las
bibliotecas; escribe playlists, marcas y caché de ondas únicamente bajo la raíz
de datos. El usuario del contenedor API es **UID 1654**: necesita permiso para
recorrer y leer la música y para leer y escribir en la raíz de datos. Pida al
administrador que configure permisos acordes con su sistema o NAS. En una
instalación nueva puede crear una raíz vacía, por ejemplo:

```bash
sudo mkdir -p /srv/dj-tracks-data
sudo chown 1654:1654 /srv/dj-tracks-data
```

No aplique ese cambio de propietario a datos existentes sin revisar primero
sus permisos y copias de seguridad.

## 3. Configurar `.env`

En la raíz del proyecto, junto a `compose.yml`:

```bash
cp -n .env.example .env
chmod 600 .env
nano .env
```

La opción `-n` conserva un `.env` existente. Si ya tiene una instalación,
revise sus valores y haga una copia de seguridad antes de modificarlos.
Sustituya los valores de ejemplo por rutas absolutas **del servidor Ubuntu**
y credenciales propias. Las líneas siguientes son ilustrativas: no use los
textos entre ángulos como contraseñas.

```dotenv
TRACKS_PATH=/srv/music/Tracks
SESSIONS_PATH=/srv/music/Sessions
REMEMBER_PATH=/srv/music/Remember
DJ_TRACKS_DATA_PATH=/srv/dj-tracks-data
UI_PORT=5050
POSTGRES_PASSWORD=<contraseña-exclusiva-de-postgresql>
SUBSONIC_USERNAME=<usuario-subsonic>
SUBSONIC_PASSWORD=<contraseña-exclusiva-de-subsonic>
```

| Variable | Qué indicar |
|---|---|
| `TRACKS_PATH` | Carpeta raíz existente de Tracks, legible por UID 1654. |
| `SESSIONS_PATH` | Carpeta raíz existente de Sessions, legible por UID 1654. |
| `REMEMBER_PATH` | Carpeta raíz existente de Remember, con `Cortas` y `Largas`. |
| `DJ_TRACKS_DATA_PATH` | Carpeta raíz existente, legible y escribible por UID 1654. |
| `UI_PORT` | Puerto accesible desde la red; si se omite, `5050`. |
| `POSTGRES_PASSWORD` | Contraseña de la base nueva. Use 64 caracteres hexadecimales. |
| `SUBSONIC_USERNAME` | Usuario exclusivo para los clientes Subsonic. |
| `SUBSONIC_PASSWORD` | Contraseña exclusiva para Subsonic, distinta de la de PostgreSQL. |

Puede ejecutar `openssl rand -hex 32` **dos veces** y copiar cada resultado en
la contraseña correspondiente. Trate la salida y el archivo `.env` como
secretos; no los comparta ni los suba al repositorio. Los valores hexadecimales
evitan problemas de interpretación de caracteres especiales de Compose. Si
deja vacías las credenciales Subsonic, la web puede funcionar, pero las
peticiones Subsonic autenticadas se rechazan.

La API creará `playlists`, `session-marks` y `tracks-waves` dentro de la raíz
de datos. Si está migrando una instalación anterior, **no empiece con carpetas
vacías sin revisar sus datos**: haga una copia y siga «Actualizar una
instalación existente» en el [README](../README.md#actualizar-una-instalación-existente)
para copiar M3U y marcas y adaptar las rutas relativas.

## 4. Construir y arrancar

Desde la raíz del proyecto:

```bash
docker compose config --quiet
docker compose up -d --build
docker compose ps
```

El primer comando valida la configuración sin imprimir secretos. El segundo
construye las imágenes e inicia la web, la API/Subsonic y PostgreSQL; la primera
vez puede tardar varios minutos. El tercero muestra su estado: `db`, `api` y
`ui` deben estar en ejecución y `db` debe aparecer saludable. Compruebe los
errores con `docker compose logs --tail=100 api` o cambiando `api` por `ui`
o `db`. Los logs pueden contener información sensible; revíselos antes de
compartirlos.

Abra en el navegador `http://IP_DEL_SERVIDOR:5050`, sustituyendo IP y puerto
por los reales. Debe aparecer DJ Tracks; entre en Tracks, Sessions y Remember
y reproduzca un MP3. En **Configuración → Probar conexión**, PostgreSQL debe
responder y no mostrar migraciones pendientes.

La API no tiene un puerto público independiente. Su protocolo Subsonic llega
al cliente mediante el proxy de la web, bajo `/rest`; no añada `/rest` a la
URL base salvo que el cliente indique expresamente lo contrario.

## 5. Conectar Tempus u otro cliente

1. Conecte el teléfono o equipo cliente a la misma LAN o VPN que el servidor.
2. Instale Tempus desde una [fuente oficial del proyecto](https://github.com/eddyizm/tempus#releases)
   o utilice otro cliente compatible con Subsonic.
3. En la pantalla para añadir un servidor, introduzca:

   | Campo del cliente | Valor |
   |---|---|
   | Dirección del servidor | `http://IP_DEL_SERVIDOR:5050`, sin `/rest` |
   | Usuario | Valor de `SUBSONIC_USERNAME` |
   | Contraseña | Valor de `SUBSONIC_PASSWORD` |
   | Autenticación | Token Subsonic (`MD5` con salt), si permite elegirla |

   Los nombres de los campos y menús pueden variar entre versiones. En Tempus,
   no active la opción de contraseña de baja seguridad. Si otro cliente solo
   permite contraseña `p` o API key, esta implementación no lo admite.
4. Guarde la conexión y, si el cliente ofrece una prueba, ejecútela. Una
   contraseña incorrecta debe rechazarse. Después seleccione la navegación por
   carpetas/bibliotecas, abra Tracks, Sessions o Remember y reproduzca un MP3.
5. Consulte las playlists existentes: deben ser de **solo lectura**. Las
   automáticas se actualizan al consultarlas; las playlists actuales solo
   incluyen Tracks y Remember, no Sessions.

Si un cliente solicita búsqueda, vistas de artistas/álbumes, edición de
playlists u otras operaciones no implementadas, puede mostrar errores o no ser
compatible. No se registran estadísticas de escucha de Subsonic. `download`
entrega el MP3 original; un cliente también puede guardar audio recibido por
`stream`, algo que el servidor no puede distinguir de la reproducción.

Opcionalmente, `stream` transcodifica a MP3, Opus, Ogg Vorbis o AAC. Para
comenzar, use el formato original y pruebe la transcodificación después: el
desplazamiento puede no funcionar con audio transcodificado porque no se
admiten rangos arbitrarios ni `timeOffset` distinto de cero.

## 6. Operación y problemas frecuentes

```bash
docker compose ps                    # estado
docker compose logs --tail=100 api   # diagnóstico de API/Subsonic
docker compose stop                  # parar sin borrar contenedores ni datos
docker compose start                 # iniciar contenedores existentes
```

Después de cambiar rutas o credenciales Subsonic en `.env`, ejecute
`docker compose up -d api` para recrear la API. Si cambia `UI_PORT`, ejecute
`docker compose up -d ui` para recrear la UI. `docker compose restart` o
`start` no aplican cambios de configuración. Tras actualizar código, use
`docker compose up -d --build`. No cambie `POSTGRES_PASSWORD` en una base ya
creada como si actualizara su contraseña interna: requiere una operación
separada de administración de PostgreSQL.

- **No aparece la web:** compruebe `docker compose ps`, la IP, `UI_PORT`, el
  firewall y que el puerto no esté ocupado. Recuerde usar la IP del servidor,
  no `localhost` desde otro dispositivo.
- **La API no arranca:** compruebe que las cuatro rutas existen, que el NAS
  está montado y que UID 1654 puede acceder a ellas y escribir en datos.
- **Tempus rechaza la conexión:** revise URL base, usuario, contraseña y
  autenticación por token; asegúrese de haber recreado la API después de editar
  `.env`. Una respuesta HTTP por sí sola no demuestra compatibilidad del cliente.
- **Faltan canciones o playlists:** revise estructura, permisos y rutas M3U;
  la música en enlaces se omite. Una biblioteca desconectada debe producir
  error, no tratarse como vacía.

Conserve copias de seguridad de la raíz de datos, `.env`, la base de
estadísticas y las claves de la UI. `docker compose down -v` elimina volúmenes
y puede borrar datos persistentes: no lo utilice para una parada ordinaria.
La checklist completa para validar F6.2 está en
[VALIDACIONES.md](VALIDACIONES.md#f62-subsonic-pr-24).
