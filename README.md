# Resilio — Disaster Relief Resource Management System

![CI - Backend](https://github.com/Sudheera-w/Resilio/actions/workflows/ci-backend.yml/badge.svg)
![CI - Frontend](https://github.com/Sudheera-w/Resilio/actions/workflows/ci-frontend.yml/badge.svg)

A full-stack web application for managing disaster relief resources, volunteers, and relief requests. 

---


## Technology Stack

| Layer | Technology |
|---|---|
| Frontend | React.js + Vite + Axios |
| Backend | ASP.NET Web API (.NET 8) + ADO.NET |
| Database | Azure SQL Database (SQL Server) |
| Version Control | GitHub |
| CI/CD | GitHub Actions |
| Deployment | Azure App Service |
| Testing | xUnit, Coverlet, Selenium, JMeter |
| Project Management | JIRA (Scrum) |

---

## Live URLs

| Environment | URL |
|---|---|
| Backend API (Production) | https://resilio-fuczgdd3dpe5gbe3.eastasia-01.azurewebsites.net |
| Swagger UI | https://resilio-fuczgdd3dpe5gbe3.eastasia-01.azurewebsites.net/swagger |
| Health Check | https://resilio-fuczgdd3dpe5gbe3.eastasia-01.azurewebsites.net/api/health |

---

## Folder Structure

```
Resilio/
├── .github/
│   └── workflows/
│       ├── ci-backend.yml        # ASP.NET build and test pipeline
│       └── ci-frontend.yml       # React build pipeline
├── backend/
│   ├── Resilio.API/              # ASP.NET Web API — controllers, middleware
│   ├── Resilio.Core/             # Models, DTOs, interfaces
│   ├── Resilio.Infrastructure/   # Repositories, data access (ADO.NET)
│   └── Resilio.Tests/            # xUnit unit tests
├── database/
│   └── migrations/               # SQL migration scripts (versioned)
├── docs/
│   ├── api/                      # Swagger JSON and API documentation
│   ├── architecture/             # System and deployment diagrams
│   ├── coverage/                 # Coverlet code coverage reports
│   ├── deployment/               # Deployment and environment setup guides
│   ├── retrospectives/           # Sprint retrospective notes
│   ├── standups/                 # Daily standup logs
│   └── testing/                  # Test plans, test cases, performance reports
├── frontend/
│   ├── public/
│   └── src/
│       ├── components/           # Reusable React components
│       ├── context/              # React context (auth, state)
│       ├── pages/                # Page-level components
│       └── services/             # Axios API service calls
└── tests/
    ├── jmeter/                   # JMeter performance test plans
    └── selenium/                 # Selenium E2E test project
```

---

## Prerequisites

Make sure you have the following installed before setting up the project locally:

- [Git](https://git-scm.com/)
- [Node.js v20+](https://nodejs.org/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Local Development Setup

### Step 1 — Clone the repository

```bash
git clone https://github.com/Sudheera-w/Resilio.git
cd Resilio
```

### Step 2 — Set up environment files

**Backend** — create `backend/Resilio.API/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=resilio-db;User ID=sa;Password=Resilio@Docker123;TrustServerCertificate=True;"
  }
}
```

> ⚠️ This file is in `.gitignore` and will not be present after cloning. You must create it manually.

### Step 3 — Run with Docker Compose

From the project root:

```bash
docker compose up --build
```

This starts three containers:
- SQL Server on `localhost:1433`
- ASP.NET backend on `localhost:5000`
- React frontend on `localhost:3000`

### Step 4 — Verify everything is running

| Check | URL |
|---|---|
| React frontend | http://localhost:3000 |
| Swagger UI | http://localhost:5000/swagger |
| Health check | http://localhost:5000/api/health |
| Database test | http://localhost:5000/api/test-db |

### Step 5 — Run database migrations

Once the containers are running, run the migration scripts against the local SQL Server using Azure Data Studio or any SQL client connecting to `localhost:1433` with user `sa` and password `Resilio@Docker123`.

Migration scripts are located in `/database/migrations/`.

---

## Running Without Docker

### Backend only

```bash
cd backend
dotnet restore
dotnet run --project Resilio.API
```

> Make sure `appsettings.Development.json` is present with a valid connection string pointing to an accessible SQL Server instance.

### Frontend only

```bash
cd frontend
npm install
npm run dev
```

Frontend runs on `http://localhost:5173` by default when using Vite dev server.

---

## Running Tests

### Unit tests

```bash
cd backend
dotnet test Resilio.sln --configuration Release --verbosity normal
```

### Code coverage

```bash
cd backend
dotnet test Resilio.sln --collect:"XPlat Code Coverage"
```

---

## Branching Strategy

| Branch | Purpose |
|---|---|
| `main` | Production-ready code only |
| `develop` | Integration branch — all features merge here first |
| `feature/[name]/[STORY-ID]-description` | New feature development |
| `fix/[name]/[STORY-ID]-description` | Bug fixes |
| `test/[name]/[STORY-ID]-description` | Pipeline or testing validation (never merged) |

### Branch protection rules

- PRs required before merging to `main` or `develop`
- Both CI pipelines (`build-and-test` and `build`) must pass before merge
- Force pushes are blocked on `main` and `develop`



---

## CI/CD Pipeline

Every pull request to `main` or `develop` automatically triggers:

| Pipeline | What it does |
|---|---|
| CI - Backend | Restores NuGet packages, builds solution, runs xUnit tests |
| CI - Frontend | Installs npm packages, runs Vite build |

If either pipeline fails the PR cannot be merged.

Continuous deployment to Azure App Service is configured in Sprint 2.

---

## Environment Variables

The following environment variable must be set in Azure App Service for production:

| Name | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | Azure SQL Database connection string |

For local development this is handled through `appsettings.Development.json` (see setup above).

---

## Documentation Index

| Document | Location |
|---|---|
| Deployment Guide | /docs/deployment/README.md |
| API Documentation | /docs/api/ |
| Test Plan | /docs/testing/ |
| Architecture Diagrams | /docs/architecture/ |
| Sprint Retrospectives | /docs/retrospectives/ |
| Daily Standup Logs | /docs/standups/ |
