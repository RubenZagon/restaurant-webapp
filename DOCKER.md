# Docker Guide

Complete guide for running the Restaurant application with Docker.

## Quick Start

```bash
# Start all services (databases + backend + frontend)
docker-compose up --build

# Access:
# - Frontend: http://localhost:3000
# - Backend API: http://localhost:5000
# - Swagger: http://localhost:5000/swagger
```

## Architecture

The Docker setup consists of 4 services:

```
┌─────────────┐     ┌─────────────┐
│  Frontend   │────▶│   Backend   │
│   (Nginx)   │     │   (.NET 8)  │
│  Port 3000  │     │  Port 5000  │
└─────────────┘     └──────┬──────┘
                           │
                  ┌────────┴────────┐
                  ▼                 ▼
            ┌──────────┐     ┌──────────┐
            │PostgreSQL│     │  Redis   │
            │Port 5432 │     │Port 6379 │
            └──────────┘     └──────────┘
```

## Docker Compose Commands

### Start Services

```bash
# Build and start (recommended for first time)
docker-compose up --build

# Start in detached mode (background)
docker-compose up -d

# Start specific services
docker-compose up postgres redis
docker-compose up backend
docker-compose up frontend
```

### Stop Services

```bash
# Stop all services
docker-compose stop

# Stop and remove containers
docker-compose down

# Stop, remove containers, and delete volumes (CAUTION: deletes database)
docker-compose down -v
```

### View Status

```bash
# List running containers
docker-compose ps

# View logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View specific service logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs postgres
```

### Rebuild Services

```bash
# Rebuild all services
docker-compose build

# Rebuild specific service
docker-compose build backend
docker-compose build frontend

# Force rebuild (no cache)
docker-compose build --no-cache
```

## Individual Container Commands

### Backend Container

```bash
# Access backend shell
docker-compose exec backend /bin/sh

# Run tests
docker-compose exec backend dotnet test

# Check version
docker-compose exec backend dotnet --version

# View environment variables
docker-compose exec backend env
```

### Frontend Container

```bash
# Access frontend shell
docker-compose exec frontend /bin/sh

# Check nginx configuration
docker-compose exec frontend nginx -t

# View nginx logs
docker-compose exec frontend cat /var/log/nginx/error.log
```

### Database Operations

```bash
# Access PostgreSQL
docker-compose exec postgres psql -U restaurant_user -d restaurant_db

# Backup database
docker-compose exec postgres pg_dump -U restaurant_user restaurant_db > backup.sql

# Restore database
docker-compose exec -T postgres psql -U restaurant_user -d restaurant_db < backup.sql

# Access Redis CLI
docker-compose exec redis redis-cli

# Monitor Redis
docker-compose exec redis redis-cli MONITOR
```

## Build Individual Images

### Backend Image

```bash
cd src/backend

# Build
docker build -t restaurant-backend:latest .

# Run standalone
docker run -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  restaurant-backend:latest
```

### Frontend Image

```bash
cd src/frontend

# Build
docker build -t restaurant-frontend:latest .

# Run standalone
docker run -p 3000:80 restaurant-frontend:latest
```

## Environment Variables

### Backend

```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:80
ConnectionStrings__PostgreSQL=Host=postgres;Port=5432;...
ConnectionStrings__Redis=redis:6379
```

### Frontend

```bash
VITE_API_URL=http://localhost:5000
```

## Health Checks

All services have health checks configured:

```bash
# Check all services health
docker-compose ps

# Backend health
curl http://localhost:5000/api/tables/health

# Frontend health
curl http://localhost:3000/health

# PostgreSQL health
docker-compose exec postgres pg_isready -U restaurant_user

# Redis health
docker-compose exec redis redis-cli ping
```

## Volumes

The docker-compose creates persistent volumes:

- `postgres_data`: Database files
- `redis_data`: Redis persistence

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect restaurant-webapp_postgres_data

# Remove all volumes (CAUTION: deletes all data)
docker volume prune
```

## Network

Services communicate through a custom bridge network:

```bash
# List networks
docker network ls

# Inspect network
docker network inspect restaurant-webapp_restaurant-network

# Services can resolve each other by name:
# - backend → postgres:5432
# - backend → redis:6379
```

## Troubleshooting

### Port Already in Use

```bash
# Check what's using port 5000
lsof -i :5000
netstat -tuln | grep 5000

# Kill process
kill -9 <PID>

# Or change port in docker-compose.yml
```

### Build Fails

```bash
# Clear Docker cache
docker system prune -a

# Remove specific image
docker rmi restaurant-backend:latest

# Rebuild without cache
docker-compose build --no-cache backend
```

### Container Crashes

```bash
# View logs
docker-compose logs backend

# Check exit code
docker-compose ps

# Restart service
docker-compose restart backend

# Force recreate
docker-compose up -d --force-recreate backend
```

### Database Connection Issues

```bash
# Verify PostgreSQL is healthy
docker-compose ps postgres

# Check logs
docker-compose logs postgres

# Test connection
docker-compose exec postgres psql -U restaurant_user -d restaurant_db -c "SELECT 1"

# Recreate database
docker-compose down -v
docker-compose up postgres -d
```

### Frontend Can't Reach Backend

1. Check backend is running:
```bash
curl http://localhost:5000/api/tables/health
```

2. Check CORS configuration in `Program.cs`

3. Verify environment variable:
```bash
docker-compose exec frontend env | grep VITE_API_URL
```

4. Check network connectivity:
```bash
docker-compose exec frontend wget -O- http://backend/api/tables/health
```

## Performance Tips

### Optimize Build Time

```bash
# Use BuildKit for faster builds
DOCKER_BUILDKIT=1 docker-compose build

# Build in parallel
docker-compose build --parallel
```

### Monitor Resource Usage

```bash
# View real-time stats
docker stats

# View specific container
docker stats restaurant-backend
```

### Clean Up

```bash
# Remove stopped containers
docker container prune

# Remove unused images
docker image prune

# Remove everything (CAUTION)
docker system prune -a --volumes
```

## Production Deployment

### Multi-stage Build Benefits

Both Dockerfiles use multi-stage builds:

**Backend:**
1. Build stage: SDK image (large) - builds the app
2. Runtime stage: ASP.NET image (small) - runs the app

**Frontend:**
1. Build stage: Node image - builds React app
2. Runtime stage: Nginx Alpine (tiny) - serves static files

Result: Small, secure production images.

### Security Best Practices

✅ Non-root users in both containers
✅ Minimal base images (Alpine)
✅ No development dependencies in production
✅ Security headers in nginx
✅ Health checks configured
✅ Resource limits can be set

### Example Production docker-compose

```yaml
version: '3.8'
services:
  backend:
    image: restaurant-backend:v1.0.0
    deploy:
      replicas: 3
      resources:
        limits:
          cpus: '1'
          memory: 512M
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
```

## CI/CD Integration

### GitHub Actions

The project includes `.github/workflows/ci.yml` that:
1. Runs tests
2. Builds Docker images
3. Can push to registry (configure secrets)

### Build in CI

```bash
# Build without starting
docker-compose build

# Tag for registry
docker tag restaurant-backend:latest registry.example.com/restaurant-backend:v1.0.0

# Push to registry
docker push registry.example.com/restaurant-backend:v1.0.0
```

## Useful Commands Cheat Sheet

```bash
# Quick start
docker-compose up -d

# View all logs
docker-compose logs -f

# Restart everything
docker-compose restart

# Stop everything
docker-compose down

# Nuclear option (delete everything)
docker-compose down -v && docker system prune -a

# Run backend tests
docker-compose exec backend dotnet test

# Access database
docker-compose exec postgres psql -U restaurant_user restaurant_db

# Check health of all services
docker-compose ps
```

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Node Docker Images](https://hub.docker.com/_/node)
- [Nginx Docker Images](https://hub.docker.com/_/nginx)
