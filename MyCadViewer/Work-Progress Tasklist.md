# Work-Progress Tasklist

- [x] Repository scaffolding (backend Clean Architecture + frontend Angular 17)
- [x] Backend solution and projects (.NET 9): Domain, Application, Infrastructure, API
- [x] Domain model: `CadModel`, `LayerInfo`, `LineSegment3D`, `ModelJob`, `ModelId`
- [x] Core interfaces: `ICadParser`, `IModelConverter`, `IModelStorage`, `IBackgroundTaskQueue`, `IJobStore`
- [x] Application service: `ModelProcessingService`
- [x] Infrastructure storage: File system storage for source, GLTF, SVG, layers metadata
- [x] Parsers: DXF (netDxf), DWG (ACadSharp) – MVP maps layers and lines
- [x] Converters: GLTF (SharpGLTF, grouped by layer), SVG (2D lines)
- [x] Async pipeline: In-memory queue + `BackgroundService` worker
- [x] API endpoints: upload, status, gltf, svg, delete, layers
- [x] Response compression (GZip)
- [x] Swagger/OpenAPI
- [x] Logging (Serilog)
- [x] CORS (dev-friendly)
- [x] Config: `appsettings.json` and `.Development.json`
- [x] Backend Dockerfile
- [x] Frontend Angular workspace (standalone)
- [x] Three.js SceneService lifecycle and cleanup
- [x] ModelService (GLTF loading)
- [x] Upload + polling (ApiService)
- [x] Viewer UI: Toolbar, Layer panel
- [x] Layer visibility via node names
- [x] Measurement tool (raycasting)
- [x] Section plane (global clipping)
- [x] Wireframe toggle
- [ ] Orthographic camera switch (full swap) – partial (toggle planned)
- [ ] DRACO compression (pipeline, loader wiring)
- [ ] SVG export UI endpoint usage in frontend
- [x] CI: GitHub Actions (backend, frontend)
- [ ] Azure deployment config (App Service/containers) – samples pending
- [ ] VM deployment scripts (systemd/Windows Service) – pending

Overall status: In progress