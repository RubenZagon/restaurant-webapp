const { GenericContainer, Network, Wait } = require('testcontainers');

class TestEnvironment {
  constructor() {
    this.network = null;
    this.postgresContainer = null;
    this.redisContainer = null;
    this.backendContainer = null;
    this.backendUrl = null;
  }

  async start() {
    console.log('Starting test environment with Testcontainers...');

    try {
      // Create network
      console.log('Creating Docker network...');
      this.network = await new Network().start();

      // Start PostgreSQL
      console.log('Starting PostgreSQL...');
      this.postgresContainer = await new GenericContainer('postgres:16-alpine')
        .withNetwork(this.network)
        .withNetworkAliases('postgres')
        .withEnvironment({
          POSTGRES_DB: 'restaurant_db',
          POSTGRES_USER: 'restaurant_user',
          POSTGRES_PASSWORD: 'restaurant_pass_dev'
        })
        .withExposedPorts(5432)
        .withHealthCheck({
          test: ['CMD-SHELL', 'pg_isready -U restaurant_user -d restaurant_db'],
          interval: 10000,
          timeout: 5000,
          retries: 5
        })
        .withWaitStrategy(Wait.forHealthCheck())
        .start();

      console.log('PostgreSQL started');

      // Start Redis
      console.log('Starting Redis...');
      this.redisContainer = await new GenericContainer('redis:7-alpine')
        .withNetwork(this.network)
        .withNetworkAliases('redis')
        .withExposedPorts(6379)
        .withHealthCheck({
          test: ['CMD', 'redis-cli', 'ping'],
          interval: 10000,
          timeout: 3000,
          retries: 5
        })
        .withWaitStrategy(Wait.forHealthCheck())
        .start();

      console.log('Redis started');

      // Build and start backend
      console.log('Building backend image...');
      const backendPath = require('path').resolve(__dirname, '../../../src/backend');

      // First build the image
      const builtImage = await GenericContainer.fromDockerfile(backendPath, 'Dockerfile').build();
      console.log('Backend image built successfully');

      // Then create and start the container
      console.log('Starting backend container...');
      const BACKEND_TEST_PORT = 5001;

      this.backendContainer = await builtImage
        .withNetwork(this.network)
        .withNetworkAliases('backend')
        .withEnvironment({
          ASPNETCORE_ENVIRONMENT: 'Development',
          ASPNETCORE_URLS: 'http://+:80',
          ConnectionStrings__PostgreSQL: 'Host=postgres;Port=5432;Database=restaurant_db;Username=restaurant_user;Password=restaurant_pass_dev',
          ConnectionStrings__Redis: 'redis:6379'
        })
        .withExposedPorts({ container: 80, host: BACKEND_TEST_PORT })
        .withWaitStrategy(Wait.forLogMessage(/Starting Restaurant Application API/i))
        .start();

      this.backendUrl = `http://localhost:${BACKEND_TEST_PORT}`;

      console.log(`Backend started at: ${this.backendUrl}`);

      // Wait for API to be ready
      await this.waitForApi();

      return {
        backendUrl: this.backendUrl,
      };
    } catch (error) {
      console.error('Failed to start test environment:', error);
      await this.stop();
      throw error;
    }
  }

  async waitForApi() {
    const maxRetries = 30;
    let retries = 0;

    while (retries < maxRetries) {
      try {
        const response = await fetch(`${this.backendUrl}/api/categories`);
        if (response.ok) {
          console.log('API is ready!');
          return;
        }
      } catch (error) {
        // API not ready yet
      }

      retries++;
      console.log(`Waiting for API... (${retries}/${maxRetries})`);
      await new Promise(resolve => setTimeout(resolve, 2000));
    }

    throw new Error('API failed to start within the timeout period');
  }

  async stop() {
    console.log('Stopping test environment...');

    try {
      if (this.backendContainer) {
        await this.backendContainer.stop();
        this.backendContainer = null;
      }
    } catch (error) {
      console.error('Error stopping backend:', error);
    }

    try {
      if (this.redisContainer) {
        await this.redisContainer.stop();
        this.redisContainer = null;
      }
    } catch (error) {
      console.error('Error stopping redis:', error);
    }

    try {
      if (this.postgresContainer) {
        await this.postgresContainer.stop();
        this.postgresContainer = null;
      }
    } catch (error) {
      console.error('Error stopping postgres:', error);
    }

    try {
      if (this.network) {
        await this.network.stop();
        this.network = null;
      }
    } catch (error) {
      console.error('Error stopping network:', error);
    }

    this.backendUrl = null;
  }

  getBackendUrl() {
    return this.backendUrl;
  }
}

module.exports = { TestEnvironment };
