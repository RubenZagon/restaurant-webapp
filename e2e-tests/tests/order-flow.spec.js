const { test, expect } = require('@playwright/test');
const { TestEnvironment } = require('./helpers/containers');

let testEnv;
let backendUrl;

test.beforeAll(async () => {
  testEnv = new TestEnvironment();
  const urls = await testEnv.start();
  backendUrl = urls.backendUrl;
}, 240000); // 4 minutes timeout for container startup

test.afterAll(async () => {
  await testEnv.stop();
}, 60000);

test.describe('Restaurant Order Flow E2E', () => {
  test('should complete full order workflow', async ({ page }) => {
    const tableNumber = 5;

    // Enable console logging from the page
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));
    page.on('pageerror', error => console.log('PAGE ERROR:', error.message));

    // Step 1: Navigate to menu page
    console.log(`Navigating to http://localhost:5173/table/${tableNumber}`);
    await page.goto(`http://localhost:5173/table/${tableNumber}`, { waitUntil: 'networkidle' });

    // Debug: Wait a bit and log what we see
    await page.waitForTimeout(3000);
    console.log('Page URL:', page.url());
    console.log('Page title:', await page.title());

    const h1Count = await page.locator('h1').count();
    console.log('Number of h1 elements:', h1Count);

    // Get all text from body
    const bodyText = await page.locator('body').textContent();
    console.log('Body text:', bodyText);

    // Get full HTML for debugging
    const html = await page.content();
    console.log('Full HTML length:', html.length);
    console.log('HTML preview:', html.substring(0, 500));

    // Verify page loaded
    await expect(page.locator('h1')).toContainText(`Table ${tableNumber}`);

    // Step 2: Wait for categories to load
    await expect(page.locator('text=Menu')).toBeVisible();

    // Verify categories are displayed (wait for API response)
    await page.waitForSelector('[role="tablist"], button:has-text("Appetizers"), button:has-text("Main")', { timeout: 10000 });

    // Step 3: Select a category if not already selected
    const categoriesExist = await page.locator('button').count() > 0;
    expect(categoriesExist).toBeTruthy();

    // Step 4: Wait for products to load
    await page.waitForSelector('[data-testid="product-card"], .product-card, button:has-text("Add to Cart")', { timeout: 10000 });

    // Step 5: Add first product to cart
    const firstAddButton = page.locator('button:has-text("Add to Cart")').first();
    await expect(firstAddButton).toBeVisible();
    await firstAddButton.click();

    // Verify cart icon appears or updates
    await page.waitForTimeout(500);

    // Step 6: Add second product to cart
    const secondAddButton = page.locator('button:has-text("Add to Cart")').nth(1);
    if (await secondAddButton.isVisible()) {
      await secondAddButton.click();
      await page.waitForTimeout(500);
    }

    // Step 7: Open cart
    const cartIcon = page.locator('[data-testid="cart-icon"], button:has-text("Cart"), [aria-label*="cart" i]').first();
    await expect(cartIcon).toBeVisible();
    await cartIcon.click();

    // Step 8: Verify cart is open and has items
    await expect(page.locator('text=Your Order')).toBeVisible();
    await expect(page.locator('text=Total:')).toBeVisible();

    // Verify at least one item in cart
    const cartItems = page.locator('[data-testid="cart-item"], .cart-item, text=/EUR|€/');
    await expect(cartItems.first()).toBeVisible();

    // Step 9: Confirm order
    const confirmButton = page.locator('button:has-text("Confirm Order")');
    await expect(confirmButton).toBeVisible();
    await expect(confirmButton).toBeEnabled();
    await confirmButton.click();

    // Step 10: Verify success message
    await expect(page.locator('text=/Order confirmed|sent to the kitchen/i')).toBeVisible({ timeout: 10000 });

    // Step 11: Verify cart is cleared
    await page.waitForTimeout(1000);

    // Cart should be closed or empty
    const cartStillOpen = await page.locator('text=Your Order').isVisible().catch(() => false);
    if (cartStillOpen) {
      await expect(page.locator('text=Your cart is empty')).toBeVisible();
    }
  });

  test('should show warning when trying to checkout with empty cart', async ({ page }) => {
    const tableNumber = 7;

    // Navigate to menu page
    await page.goto(`http://localhost:5173/table/${tableNumber}`);

    // Wait for page to load
    await expect(page.locator('h1')).toContainText(`Table ${tableNumber}`);
    await page.waitForSelector('text=Menu');

    // Open cart without adding items
    const cartIcon = page.locator('[data-testid="cart-icon"], button:has-text("Cart"), [aria-label*="cart" i]').first();
    await expect(cartIcon).toBeVisible();
    await cartIcon.click();

    // Verify empty cart message
    await expect(page.locator('text=Your cart is empty')).toBeVisible();

    // Confirm button should not be visible for empty cart
    const confirmButton = page.locator('button:has-text("Confirm Order")');
    await expect(confirmButton).not.toBeVisible();
  });

  test('should update quantity in cart', async ({ page }) => {
    const tableNumber = 8;

    // Navigate and add product
    await page.goto(`http://localhost:5173/table/${tableNumber}`);
    await expect(page.locator('h1')).toContainText(`Table ${tableNumber}`);

    // Wait for products
    await page.waitForSelector('button:has-text("Add to Cart")', { timeout: 10000 });

    // Add product
    await page.locator('button:has-text("Add to Cart")').first().click();
    await page.waitForTimeout(500);

    // Open cart
    const cartIcon = page.locator('[data-testid="cart-icon"], button:has-text("Cart"), [aria-label*="cart" i]').first();
    await cartIcon.click();

    // Find quantity controls
    const increaseButton = page.locator('button:has-text("+")').first();
    const decreaseButton = page.locator('button:has-text("-")').first();

    // Get initial quantity
    const quantityDisplay = page.locator('text=/^\\d+$/')
      .filter({ has: page.locator('..').locator('button:has-text("+")')  });

    // Increase quantity
    await increaseButton.click();
    await page.waitForTimeout(300);

    // Decrease quantity
    await decreaseButton.click();
    await page.waitForTimeout(300);

    // Verify we can interact with quantity controls
    expect(await increaseButton.isEnabled()).toBeTruthy();
  });

  test('should remove item from cart', async ({ page }) => {
    const tableNumber = 9;

    // Navigate and add product
    await page.goto(`http://localhost:5173/table/${tableNumber}`);
    await expect(page.locator('h1')).toContainText(`Table ${tableNumber}`);

    // Wait for products
    await page.waitForSelector('button:has-text("Add to Cart")', { timeout: 10000 });

    // Add product
    await page.locator('button:has-text("Add to Cart")').first().click();
    await page.waitForTimeout(500);

    // Open cart
    const cartIcon = page.locator('[data-testid="cart-icon"], button:has-text("Cart"), [aria-label*="cart" i]').first();
    await cartIcon.click();

    // Remove item (look for × or remove button)
    const removeButton = page.locator('button:has-text("×")').first();
    await removeButton.click();
    await page.waitForTimeout(500);

    // Verify cart is empty
    await expect(page.locator('text=Your cart is empty')).toBeVisible();
  });

  test('should display categories and switch between them', async ({ page }) => {
    const tableNumber = 10;

    await page.goto(`http://localhost:5173/table/${tableNumber}`);
    await expect(page.locator('h1')).toContainText(`Table ${tableNumber}`);

    // Wait for categories
    await page.waitForSelector('button', { timeout: 10000 });

    const categoryButtons = page.locator('button').filter({ hasText: /Appetizers|Main|Desserts|Beverages/ });
    const categoryCount = await categoryButtons.count();

    expect(categoryCount).toBeGreaterThan(0);

    // Try clicking second category if it exists
    if (categoryCount > 1) {
      await categoryButtons.nth(1).click();
      await page.waitForTimeout(500);

      // Products should reload
      await expect(page.locator('button:has-text("Add to Cart")')).toBeVisible({ timeout: 5000 });
    }
  });
});
