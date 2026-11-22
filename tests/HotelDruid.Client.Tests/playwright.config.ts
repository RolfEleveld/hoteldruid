import { defineConfig, devices } from '@playwright/test';

// Node runner for the browser tests in this project.
// npx playwright test --config tests/HotelDruid.Client.Tests/playwright.config.ts
export default defineConfig({
  testDir: './Playwright',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'https://localhost:5001',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],

  webServer: {
    command: 'dotnet run --project src/HotelDruid.Api --urls https://localhost:5001',
    cwd: '../..',
    url: 'https://localhost:5001',
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000,
  },
});