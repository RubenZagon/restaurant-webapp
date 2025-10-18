const { TestEnvironment } = require('./tests/helpers/containers');

async function main() {
  console.log('Starting test backend...');
  console.log('Press Ctrl+C to stop');

  const testEnv = new TestEnvironment();

  try {
    const { backendUrl } = await testEnv.start();
    console.log('\n✅ Backend is ready!');
    console.log(`   Backend URL: ${backendUrl}`);
    console.log(`   Swagger: ${backendUrl}/swagger`);
    console.log('\nNow you can:');
    console.log('1. Open http://localhost:5173/table/5 in your browser');
    console.log('2. Run the E2E tests: npm test');
    console.log('\nPress Ctrl+C to stop the backend...\n');

    // Keep the process running
    await new Promise(() => {});
  } catch (error) {
    console.error('Failed to start backend:', error);
    process.exit(1);
  }
}

// Handle Ctrl+C gracefully
process.on('SIGINT', async () => {
  console.log('\n\nStopping backend...');
  process.exit(0);
});

main();
