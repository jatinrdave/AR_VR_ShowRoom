# Technical Document

This document enumerates the implementation tasks derived from the project objectives. Each task includes a brief description and acceptance criteria.

## Architecture & Foundation
- Clean Architecture layering (Domain, Application, Infrastructure, API)
  - Criteria: No infrastructure dependencies in Domain/Application; DI configured in API
- Solution scaffolding and project references
  - Criteria: `CadViewer.sln` with four projects and correct references
- Configuration & environments
  - Criteria: `appsettings.json`, `appsettings.Development.json`; CORS; request limits
- Logging & diagnostics
  - Criteria: Serilog wired; request logging enabled
- Response compression & caching
  - Criteria: GZip enabled; cache headers on static-like endpoints

## Backend Features
- File upload endpoint (async enqueue)
  - Criteria: POST `/api/models`, returns 202 with job id
- Job status endpoint
  - Criteria: GET `/api/models/{id}` returns state and error
- CAD parsing: DXF (netDxf) and DWG (ACadSharp)
  - Criteria: Layers + line entities mapped into domain model
- Conversion: GLTF (SharpGLTF) + SVG 2D projection
  - Criteria: GLB with lines grouped by layer; SVG containing lines
- Storage abstraction + file system storage
  - Criteria: Save source, GLB, SVG, layers metadata by model id
- Layers endpoint
  - Criteria: GET `/api/layers/{id}` returns layer list
- Background processing
  - Criteria: In-memory queue and hosted worker convert jobs
- Delete endpoint
  - Criteria: DELETE `/api/models/{id}` cleans up
- Dockerfile
  - Criteria: Multi-stage build on dotnet 9 images

## Frontend (Angular 17 + Three.js)
- Angular workspace with routing and standalone components
  - Criteria: Lazy route `/viewer`, `AppComponent` shells router-outlet
- Three.js scene service
  - Criteria: Scene, renderer, camera setup; resize; cleanup
- Model loading service
  - Criteria: Fetch GLB via HttpClient and add to scene (GLTFLoader)
- Upload and status polling
  - Criteria: UI upload, store model id, poll until Completed
- Layer panel and toggling
  - Criteria: Toggle visibility by layer node names
- Measurement tool
  - Criteria: Raycast pick two points and draw a line
- Section plane (clipping)
  - Criteria: Global clipping plane toggle
- Display modes (wireframe)
  - Criteria: Wireframe toggle over mesh materials
- Orthographic camera
  - Criteria: Switch between perspective and orthographic
- SVG export (UI)
  - Criteria: Button to fetch and download SVG

## DevOps
- CI (GitHub Actions)
  - Criteria: Backend build/publish and Docker build; Frontend build
- Azure deployment configuration (App Service, containers)
  - Criteria: Documentation with steps and settings
- VM deployment scripts
  - Criteria: systemd unit or Windows Service sample
- DRACO compression pipeline (optional)
  - Criteria: DRACO loader configured; docs for server-side compression