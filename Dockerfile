# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for caching layer
COPY ["LAP.Shared/LAP.Shared.csproj", "LAP.Shared/"]
COPY ["LAP.Domain/LAP.Domain.csproj", "LAP.Domain/"]
COPY ["LAP.Application/LAP.Application.csproj", "LAP.Application/"]
COPY ["LAP.Infrastructure/LAP.Infrastructure.csproj", "LAP.Infrastructure/"]
COPY ["LAP.API/LAP.API.csproj", "LAP.API/"]

# Restore dependencies
RUN dotnet restore "LAP.API/LAP.API.csproj"

# Copy full source and publish
COPY . .
WORKDIR "/src/LAP.API"
RUN dotnet publish "LAP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "LAP.API.dll"]
