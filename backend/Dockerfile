# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Resilio.sln .
COPY backend/Resilio.API/Resilio.API.csproj backend/Resilio.API/
COPY backend/Resilio.Core/Resilio.Core.csproj backend/Resilio.Core/
COPY backend/Resilio.Infrastructure/Resilio.Infrastructure.csproj backend/Resilio.Infrastructure/
COPY backend/Resilio.Tests/Resilio.Tests.csproj backend/Resilio.Tests/

# Restore dependencies
RUN dotnet restore Resilio.sln

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish backend/Resilio.API/Resilio.API.csproj -c Release -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Resilio.API.dll"]