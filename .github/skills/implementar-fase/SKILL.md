---
name: implementar-fase
description: Implementa una fase concreta del plan de DJ Tracks (docs/PLAN.md), por ejemplo "F1.2". Úsala cuando se pida desarrollar o continuar una fase.
---

# Implementar una fase de DJ Tracks

0. Lee `AGENTS.md`, `docs/PLAN.md` y `docs/VALIDACIONES.md`; comprueba el estado de la rama y del PR. No avances si la fase anterior sigue pendiente de validación o merge, salvo petición expresa.
1. Usa una rama por fase desde `develop` (nombre orientativo `fase/<id>-<nombre>`). Respeta las herramientas de gestión de ramas del entorno; si ya estás en la rama de la fase, continúa en ella. Haz commits pequeños e implementa solo esa fase.
2. Consulta context7 para las librerías implicadas antes de escribir código.
3. Backend: un slice por funcionalidad en `src/DjTracks.Api/Features/<Feature>`; Minimal API.
4. Frontend: componentes MudBlazor en `src/DjTracks.UI`; sin lógica de ficheros en la UI.
5. No generes tests salvo petición explícita. Usa las validaciones existentes y compila con `dotnet build DjTracks.slnx`. Docker queda para F6.1.
6. Actualiza `docs/VALIDACIONES.md`: cambios, fase, estado, requisitos de arranque, pasos con resultados esperados y limitaciones conocidas. Separa lo comprobado por el agente de la validación manual pendiente.
7. Sube los commits y abre o actualiza el PR a `develop`. Incluye la checklist de validación en el PR y notifícala en el mensaje de entrega; indica qué hay que repetir si es una revisión.
8. Espera la confirmación del usuario antes de marcar la fase como validada. No hagas merge sin autorización.
