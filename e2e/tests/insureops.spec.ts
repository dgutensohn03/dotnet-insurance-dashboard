import { test, expect } from '@playwright/test';

async function ready(page) {
  await expect(page.getByRole('heading', { level: 1 })).toBeVisible();
}

test('primary routes render without application overflow', async ({ page }) => {
  for (const path of ['', 'policies', 'claims', 'customers', 'analytics']) {
    await page.goto(path);
    await ready(page);
    const overflow = await page.evaluate(() => document.documentElement.scrollWidth > document.documentElement.clientWidth + 1);
    expect(overflow, `${path || 'overview'} should not overflow the viewport`).toBeFalsy();
  }
});

test('claim detail and create dialog preserve context and keyboard behavior', async ({ page }, testInfo) => {
  await page.goto('claims');
  await ready(page);
  await page.getByText('CLM-10482', { exact: true }).click();
  await expect(page.getByLabel('Claim details')).toBeVisible();
  await page.screenshot({ path: testInfo.outputPath('claim-detail.png'), fullPage: true });

  await page.getByRole('button', { name: 'New claim' }).click();
  const dialog = page.getByRole('dialog', { name: 'Create claim' });
  await expect(dialog).toBeVisible();
  await expect(page.locator('body')).toHaveClass(/modal-open/);
  await page.screenshot({ path: testInfo.outputPath('new-claim-modal.png'), fullPage: true });
  await page.keyboard.press('Escape');
  await expect(dialog).toBeHidden();
  await expect(page.locator('body')).not.toHaveClass(/modal-open/);
});

test('customer create flow gives visible feedback', async ({ page }) => {
  await page.goto('customers');
  await ready(page);
  await page.getByRole('button', { name: 'New customer' }).click();
  const dialog = page.getByRole('dialog', { name: 'Create customer' });
  await expect(dialog).toBeVisible();
  await dialog.getByLabel('Full name').fill('Morgan Reed');
  await dialog.getByLabel('Email').fill('morgan.reed@example.com');
  await dialog.getByLabel('State').fill('CO');
  await dialog.getByRole('button', { name: 'Create customer' }).click();
  await expect(page.getByRole('status')).toContainText('Customer created');
});

test('mobile modal is centered and background is locked', async ({ page }, testInfo) => {
  test.skip(!testInfo.project.name.includes('mobile'), 'mobile-specific assertion');
  await page.goto('claims');
  await ready(page);
  await page.getByRole('button', { name: 'New claim' }).click();
  const dialog = page.getByRole('dialog', { name: 'Create claim' });
  await expect(dialog).toBeVisible();
  await expect(page.locator('body')).toHaveCSS('position', 'fixed');
  const box = await dialog.boundingBox();
  const viewport = page.viewportSize();
  expect(box && viewport).toBeTruthy();
  if (box && viewport) {
    expect(box.x).toBeGreaterThanOrEqual(0);
    expect(box.y).toBeGreaterThanOrEqual(0);
    expect(box.x + box.width).toBeLessThanOrEqual(viewport.width + 1);
    expect(box.y + box.height).toBeLessThanOrEqual(viewport.height + 1);
  }
  await page.screenshot({ path: testInfo.outputPath('mobile-new-claim.png'), fullPage: true });
});
