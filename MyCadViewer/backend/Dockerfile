FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ./CadViewer.sln ./
COPY ./CadViewer.*/*.csproj ./
RUN for file in *.csproj; do mkdir -p ${file%.*} && mv "$file" ${file%.*}/; done
RUN dotnet restore CadViewer.sln
COPY . .
RUN dotnet publish CadViewer.Api/CadViewer.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CadViewer.Api.dll"]