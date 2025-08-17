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

## Frontend

Requirements: Node 18+, Angular CLI 17

- Install: `npm ci` in `frontend/`
- Run: `npm start` in `frontend/` (defaults to http://localhost:4200)
- Configure API base URL in `frontend/src/environments/environment.ts`

## Notes

- DXF parsed via netDxf, DWG via ACadSharp (lines only in MVP)
- GLTF generated via SharpGLTF (lines)
- Background processing decouples upload from conversion