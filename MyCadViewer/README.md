# MyCadViewer

Single-repo 3D CAD Viewer using ASP.NET Core 9 (Clean Architecture) and Angular 17 + Three.js.

## Structure

- backend/
  - CadViewer.Api (Web API)
  - CadViewer.Application (Application layer)
  - CadViewer.Domain (Domain entities & contracts)
  - CadViewer.Infrastructure (Parsers, Converters, Storage, Worker)
- frontend/
  - Angular 17 SPA with a Three.js viewer

## Backend

Requirements: .NET 9 SDK

- Run: `dotnet run --project backend/CadViewer.Api/CadViewer.Api.csproj`
- Docker: `docker build -t mycad-backend backend/ && docker run -p 5000:80 mycad-backend`

Endpoints:
- POST /api/models (multipart `file`)
- GET /api/models/{id}
- GET /api/models/{id}/gltf
- GET /api/models/{id}/svg
- DELETE /api/models/{id}
- GET /api/layers/{id}

## Frontend

Requirements: Node 18+, Angular CLI 17

- Install: `npm ci` in `frontend/`
- Run: `npm start` in `frontend/` (defaults to http://localhost:4200)
- Configure API base URL in `frontend/src/environments/environment.ts`

## Deployment

### Azure App Service (Linux container)
1. Build and push image:
   - `docker build -t <ACR_NAME>.azurecr.io/mycad-backend:latest backend/`
   - `docker push <ACR_NAME>.azurecr.io/mycad-backend:latest`
2. Create App Service (Linux) with container and assign ACR permissions.
3. Configure settings:
   - `WEBSITES_PORT=80`
   - `ASPNETCORE_ENVIRONMENT=Production`
4. Optional: mount Azure File Share for persistent `data` if desired.

### Linux VM (systemd)
- Copy published backend to `/opt/mycad/backend`
- Example unit file `/etc/systemd/system/mycadviewer.service`:
```
[Unit]
Description=MyCadViewer Backend
After=network.target

[Service]
WorkingDirectory=/opt/mycad/backend
ExecStart=/usr/bin/dotnet CadViewer.Api.dll
Restart=always
RestartSec=5
Environment=ASPNETCORE_ENVIRONMENT=Production
User=www-data

[Install]
WantedBy=multi-user.target
```
- Enable and start:
```
sudo systemctl daemon-reload
sudo systemctl enable mycadviewer
sudo systemctl start mycadviewer
```

## Notes

- DXF parsed via netDxf, DWG via ACadSharp (lines only in MVP)
- GLTF generated via SharpGLTF (lines grouped by layer)
- Background processing decouples upload from conversion
- DRACO decoding supported on frontend if decoders are added to `src/assets/draco/`