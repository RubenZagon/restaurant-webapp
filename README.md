# Restaurant Digital Ordering System

Progressive Web App (PWA) for restaurant order and payment management, built with Hexagonal Architecture and Clean Code principles.

## Tech Stack

### Backend
- **.NET 8** - Main framework
- **PostgreSQL 16** - Relational database
- **Redis 7** - In-memory cache
- **Serilog** - Structured logging
- **xUnit + FluentAssertions** - Testing framework

### Frontend
- **React 18** with TypeScript
- **Vite** - Build tool and dev server
- **Vitest** - Testing framework
- **Axios** - HTTP client

### DevOps
- **Docker + Docker Compose** - Containerization
- **GitHub Actions** - CI/CD pipeline
- **Kubernetes** - Orchestration (production)

## Architecture

The project follows **Hexagonal Architecture (Ports & Adapters)** and **DDD** principles:

```
src/
├── backend/
│   ├── RestaurantApp.Domain/        # Pure business logic
│   ├── RestaurantApp.Application/   # Use cases
│   ├── RestaurantApp.Infrastructure/# External implementations
│   ├── RestaurantApp.API/           # REST controllers
│   └── RestaurantApp.Tests.Unit/    # Unit tests
│
└── frontend/
    ├── domain/                       # Domain models
    ├── application/                  # Use cases
    ├── infrastructure/               # Adapters (API, storage)
    └── presentation/                 # React components
```

## Development Principles

### Sustainable Code (Carlos Blé)
- Intention-revealing names
- Small functions (max 20 lines)
- No unnecessary comments
- Tests as documentation

### Outside-in TDD
1. E2E test that fails (Red)
2. Integration test that fails (Red)
3. Unit tests that fail (Red)
4. Minimum code to pass tests (Green)
5. Refactoring (Refactor)

### SOLID Principles
- **S**ingle Responsibility
- **O**pen/Closed
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

## Quick Start with Docker

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop) 20.10+
- [Docker Compose](https://docs.docker.com/compose/install/) 2.0+

### 1. Clone the repository

```bash
git clone <repository-url>
cd restaurant-webapp
```

### 2. Start all services with Docker Compose

```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up --build -d
```

This will start:
- **PostgreSQL** on port 5432
- **Redis** on port 6379
- **Backend API** on port 5000
- **Frontend** on port 3000

### 3. Access the application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

### 4. Stop all services

```bash
# Stop and remove containers
docker-compose down

# Stop and remove containers + volumes (database data)
docker-compose down -v
```

## Local Development (without Docker)

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20 LTS](https://nodejs.org/)
- [PostgreSQL 16](https://www.postgresql.org/download/)
- [Redis 7](https://redis.io/download)

### 1. Configure environment variables

```bash
cp .env.example .env
# Edit .env with your values
```

### 2. Start PostgreSQL and Redis

```bash
# Using Docker for databases only
docker-compose up postgres redis -d
```

### 3. Backend (.NET)

```bash
cd src/backend

# Restore dependencies
dotnet restore

# Run tests
dotnet test

# Start API in development mode
cd RestaurantApp.API
dotnet run
```

API will be available at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### 4. Frontend (React)

```bash
cd src/frontend

# Install dependencies
npm install

# Run tests
npm run test

# Start in development mode
npm run dev
```

Application will be available at: `http://localhost:5173`

## Available Scripts

### Backend

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run API
dotnet run --project RestaurantApp.API

# Create migration
dotnet ef migrations add <MigrationName> -p RestaurantApp.Infrastructure
```

### Frontend

```bash
# Development
npm run dev

# Build for production
npm run build

# Preview build
npm run preview

# Tests
npm run test
npm run test:ui
npm run test:coverage

# Linting
npm run lint
```

## Docker Commands

### Build images separately

```bash
# Build backend image
docker build -t restaurant-backend:latest ./src/backend

# Build frontend image
docker build -t restaurant-frontend:latest ./src/frontend
```

### Run individual containers

```bash
# Run backend
docker run -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__PostgreSQL="Host=host.docker.internal;Port=5432;Database=restaurant_db;Username=restaurant_user;Password=restaurant_pass_dev" \
  restaurant-backend:latest

# Run frontend
docker run -p 3000:80 \
  -e VITE_API_URL=http://localhost:5000 \
  restaurant-frontend:latest
```

### View logs

```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs backend
docker-compose logs frontend

# Follow logs
docker-compose logs -f backend
```

### Execute commands inside containers

```bash
# Access backend container
docker-compose exec backend /bin/sh

# Access frontend container
docker-compose exec frontend /bin/sh

# Run tests inside backend container
docker-compose exec backend dotnet test
```

## Project Status - PHASE 1 COMPLETED ✅

### Implemented

- ✅ Backend folder structure (Hexagonal Architecture)
- ✅ Frontend folder structure (React + TypeScript)
- ✅ Docker Compose with PostgreSQL and Redis
- ✅ .NET 8 projects configuration
- ✅ Value Objects: `TableId`, `SessionId`
- ✅ Entities: `Table`, `TableSession`
- ✅ Use Case: `StartTableSessionUseCase` (with tests)
- ✅ Port: `ITableRepository`
- ✅ Adapter: `InMemoryTableRepository`
- ✅ API Controller: `TablesController`
- ✅ React client with welcome and menu pages
- ✅ GitHub Actions for CI/CD
- ✅ Full Docker containerization
- ✅ Code in English

### Next Steps (PHASE 2)

- Implement product catalog
- Add categories and allergens
- Search and filter system
- PostgreSQL persistence
- Integration tests

## Testing

### Backend (xUnit)

```bash
cd src/backend
dotnet test --verbosity normal

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend (Vitest)

```bash
cd src/frontend
npm run test

# With coverage
npm run test:coverage
```

### E2E Tests (inside Docker)

```bash
# Run all tests in containers
docker-compose exec backend dotnet test
docker-compose exec frontend npm run test
```

## Commit Conventions

```
feat: New feature
fix: Bug fix
refactor: Refactoring without functional changes
test: Add or modify tests
docs: Documentation changes
chore: Maintenance tasks
build: Build system or dependencies changes
ci: CI/CD changes
```

### Example

```bash
git commit -m "feat(domain): add Table and TableSession entities with TDD

Implement domain entities following DDD principles:
- Table entity with session management
- TableSession value object
- Unit tests with 100% coverage"
```

## Health Checks

All services have health check endpoints:

- **Backend**: http://localhost:5000/api/tables/health
- **Frontend**: http://localhost:3000/health
- **PostgreSQL**: `docker-compose ps postgres` (should be "healthy")
- **Redis**: `docker-compose ps redis` (should be "healthy")

## Troubleshooting

### Port already in use

```bash
# Find process using port 5000
lsof -i :5000

# Kill process
kill -9 <PID>

# Or change port in docker-compose.yml
```

### Database connection issues

```bash
# Check if PostgreSQL is running
docker-compose ps postgres

# View PostgreSQL logs
docker-compose logs postgres

# Recreate database
docker-compose down -v
docker-compose up postgres -d
```

### Frontend can't connect to backend

Check that:
1. Backend is running: http://localhost:5000/api/tables/health
2. CORS is configured correctly in Program.cs
3. Environment variable `VITE_API_URL` is set correctly

## Security

- ✅ Non-root users in Docker containers
- ✅ Input validation with Value Objects
- ✅ HTTPS ready (certificates needed)
- ✅ Security headers in nginx
- ✅ Structured logging (no sensitive data)
- ✅ Health checks without authentication

## Contributing

1. Fork the project
2. Create feature branch (`git checkout -b feature/new-feature`)
3. Commit changes following TDD
4. Push to branch (`git push origin feature/new-feature`)
5. Create Pull Request

## License

This project is under MIT License.
