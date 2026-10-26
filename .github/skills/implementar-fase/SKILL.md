---
name: implementar-fase
description: Implementa una fase concreta del plan de DJ Tracks (docs/PLAN.md), por ejemplo "F1.2". Úsala cuando se pida desarrollar o continuar una fase.
---

# Implementar una fase de DJ Tracks

0. Crea la rama `fase/<id>-<nombre>` desde `develop` y haz commits pequeños durante el trabajo; al acabar, abre un PR a `develop`.
1. Lee `AGENTS.md` y la fase indicada en `docs/PLAN.md`. Implementa solo esa fase.
2. Consulta context7 para las librerías implicadas antes de escribir código.
3. Backend: un slice por funcionalidad en `src/DjTracks.Api/Features/<Feature>`; Minimal API.
4. Frontend: componentes MudBlazor en `src/DjTracks.UI`; sin lógica de ficheros en la UI.
5. Sin tests ni Docker. Compila con `dotnet build DjTracks.slnx`.
6. Termina indicando los pasos manuales para probar la fase en el navegador (perfil de VS a usar) y marca la fase como hecha en `docs/PLAN.md`.
