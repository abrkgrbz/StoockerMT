# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["StoockerMT.API/StoockerMT.API.csproj", "StoockerMT.API/"]
COPY ["StoockerMT.Application/StoockerMT.Application.csproj", "StoockerMT.Application/"]
COPY ["StoockerMT.Domain/StoockerMT.Domain.csproj", "StoockerMT.Domain/"]
COPY ["StoockerMT.Infrastructure/StoockerMT.Infrastructure.csproj", "StoockerMT.Infrastructure/"]
COPY ["StoockerMT.Persistence/StoockerMT.Persistence.csproj", "StoockerMT.Persistence/"] 
COPY ["StoockerMT.Identity/StoockerMT.Identity.csproj", "StoockerMT.Identity/"]

RUN dotnet restore "StoockerMT.API/StoockerMT.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/StoockerMT.API"
RUN dotnet build "StoockerMT.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StoockerMT.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy from publish stage - ÖNEMLÝ: publish stage'den kopyala
COPY --from=publish /app/publish .

# Copy Docker-specific settings if exists
COPY --from=build /src/StoockerMT.API/appsettings.Docker.json ./


# Debug: List files to verify
RUN ls -la /app/

EXPOSE 5000

ENTRYPOINT ["dotnet", "StoockerMT.API.dll"]