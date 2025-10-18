# Restaurant App - E2E Tests

End-to-end tests for the Restaurant application using Playwright and Testcontainers.

## Overview

These tests validate the complete order flow from a customer's perspective:
- Starting a table session
- Browsing menu categories
- Adding products to cart
- Managing cart quantities
- Confirming orders

## Architecture

- **Playwright**: Browser automation and testing framework
- **Testcontainers**: Manages Docker containers for isolated test environments
- **Docker Compose**: Orchestrates backend, PostgreSQL, and Redis containers

## Prerequisites

1. **Docker** must be installed and running
2. **Node.js** 18+ installed
3. **Frontend dev server** must be running on `http://localhost:5173`
4. **Port 5001** must be available (used by test backend)

## Installation

```bash
npm install
```

## Running Tests

### Before Running Tests

1. Start the frontend development server with the test backend URL:
```bash
cd ../src/frontend
VITE_API_URL=http://localhost:5001 npm run dev
```

Or on Windows:
```bash
cd ../src/frontend
set VITE_API_URL=http://localhost:5001 && npm run dev
```

The frontend should be available at `http://localhost:5173`

**Important**: The frontend must be configured to connect to port 5001 where the test backend runs.

### Run All Tests

```bash
npm test
```

### Run Tests in Headed Mode (See Browser)

```bash
npm run test:headed
```

### Debug Tests

```bash
npm run test:debug
```

### View Test Report

```bash
npm run test:report
```

## Test Scenarios

### 1. Complete Order Flow
- Navigate to menu page for a table
- Browse categories and products
- Add multiple products to cart
- Confirm order
- Verify success message and cart clearing

### 2. Empty Cart Validation
- Attempt to checkout with empty cart
- Verify appropriate warning message

### 3. Cart Quantity Management
- Add product to cart
- Increase/decrease quantities
- Verify subtotal updates

### 4. Remove Items from Cart
- Add product to cart
- Remove it using the remove button
- Verify cart becomes empty

### 5. Category Navigation
- Display all product categories
- Switch between categories
- Verify products update accordingly

## How It Works

### Testcontainers Setup

The `TestEnvironment` class in `tests/helpers/containers.js`:
1. Uses Docker Compose (`docker-compose.test.yml`) to start the backend stack:
   - PostgreSQL database
   - Redis cache
   - .NET backend API
2. Waits for services to be healthy
3. Exposes the backend URL to tests
4. Tears down containers after tests complete

**Note**: Tests use `docker-compose.test.yml` instead of the main `docker-compose.yml` to avoid container name conflicts. This test compose file doesn't specify fixed container names, allowing Testcontainers to manage them dynamically.

### Test Execution Flow

```
Start → Spin up containers → Wait for API → Run tests → Tear down
```

Each test:
1. Uses the backend URL provided by Testcontainers
2. Navigates to the frontend (running on localhost:5173)
3. Frontend connects to the containerized backend
4. Performs user actions and assertions

## Timeouts

- **Container startup**: 4 minutes (includes pulling images if needed)
- **Individual tests**: 2 minutes
- **Assertions**: 10 seconds

## Configuration

Edit `playwright.config.js` to customize:
- Browser types
- Parallel execution
- Retries
- Screenshots
- Video recording

## Troubleshooting

### "API failed to start"
- Ensure Docker is running
- Check Docker has enough resources (CPU/Memory)
- Verify no port conflicts (5432, 6379, 5000)

### "Frontend not found"
- Start the frontend dev server: `cd ../src/frontend && npm run dev`
- Verify it's running on port 5173

### Containers not cleaning up
- Testcontainers should auto-cleanup, but if needed:
- List containers: `docker ps -a`
- Stop specific containers: `docker stop <container-id>`
- Remove containers: `docker rm <container-id>`

### Container name conflicts
- Tests use `docker-compose.test.yml` with dynamic names
- If you have the main app running via `docker-compose.yml`, it won't conflict
- To stop main app containers: `docker-compose down` (from project root)

## CI/CD Integration

For CI pipelines, ensure:
1. Docker is available
2. Sufficient resources (2GB+ RAM recommended)
3. Frontend is built and served before tests
4. Set `CI=true` environment variable for optimized retries

Example GitHub Actions:
```yaml
- name: Start Frontend
  run: |
    cd src/frontend
    npm install
    npm run dev &

- name: Run E2E Tests
  run: |
    cd e2e-tests
    npm install
    npm test
  env:
    CI: true
```

## Notes

- Tests run sequentially (not in parallel) to avoid container conflicts
- Each test uses a different table number to avoid data conflicts
- Containers are shared across all tests in a single run for performance
- The backend uses in-memory repositories, so data resets with each container restart
