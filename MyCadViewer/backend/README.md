# Backend (ASP.NET Core 9)

Run locally:
- `dotnet run --project CadViewer.Api/CadViewer.Api.csproj`

Endpoints:
- POST `/api/models` (multipart form `file`)
- GET `/api/models/{id}`
- GET `/api/models/{id}/gltf`
- GET `/api/models/{id}/svg`
- GET `/api/layers/{id}`
- DELETE `/api/models/{id}`

Notes:
- In-memory queue and job store; filesystem storage under `bin/.../data`
- DXF via netDxf, DWG via ACadSharp (lines + layers)
- GLTF via SharpGLTF (lines grouped by layer)