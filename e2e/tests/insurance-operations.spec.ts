import { test, expect } from '@playwright/test';

async function ready(page) {
  await page.goto('./');
  await expect(page.getByRole('heading', { name: /operations overview/i })).toBeVisible();
}

test('primary routes render without application-level horizontal overflow', async ({ page }) => {
  await ready(page);
  for (const route of ['', 'policies', 'claims', 'customers', 'analytics']) {
    await page.goto(`./${route}`);
    await expect(page.locator('h1')).toBeVisible();
    const overflow = await page.evaluate(() => document.documentElement.scrollWidth > document.documentElement.clientWidth + 1);
    expect(overflow).toBeFalsy();
  }
});

test('claim modal locks background, traps focus, and closes with Escape', async ({ page }) => {
  await page.goto('./claims');
  await page.getByRole('button', { name: /new claim/i }).click();
  const dialog = page.getByRole('dialog', { name: /create claim/i });
  await expect(dialog).toBeVisible();
  await expect(page.locator('body')).toHaveClass(/modal-open/);
  await expect(page.locator('body')).toHaveCSS('position', 'fixed');
  await page.keyboard.press('Escape');
  await expect(dialog).toBeHidden();
  await expect(page.locator('body')).not.toHaveClass(/modal-open/);
  await expect(page.getByRole('button', { name: /new claim/i })).toBeFocused();
});

test('claim details open only after selecting a record', async ({ page }) => {
  await page.goto('./claims');
  await expect(page.getByLabel('Claim details')).toHaveCount(0);
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await expect(page.getByLabel('Claim details')).toContainText('CLM-10482');
});

test('archive and restore claim lifecycle is recoverable', async ({ page }) => {
  await page.goto('./claims');
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await page.getByRole('button', { name: /archive claim/i }).click();
  await expect(page.getByRole('alertdialog')).toBeVisible();
  await page.getByRole('alertdialog').getByRole('button', { name: /archive claim/i }).click();
  await expect(page.getByRole('status')).toContainText('archived');
  await page.getByLabel('Filter claim lifecycle').selectOption('archived');
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await page.getByRole('button', { name: /restore claim/i }).click();
  await expect(page.getByRole('status')).toContainText('restored');
});

test('mobile navigation stays inside the viewport and modal is centered', async ({ page }, testInfo) => {
  test.skip(!testInfo.project.name.includes('mobile'), 'mobile assertion');
  await page.goto('./claims');
  const nav = page.getByRole('navigation', { name: 'Primary navigation' });
  await expect(nav).toBeVisible();
  const navBox = await nav.boundingBox();
  expect(navBox?.x).toBeGreaterThanOrEqual(0);
  expect((navBox?.x || 0) + (navBox?.width || 0)).toBeLessThanOrEqual((await page.viewportSize())!.width + 1);
  await page.getByRole('button', { name: /new claim/i }).click();
  const box = await page.getByRole('dialog').boundingBox();
  const viewport = (await page.viewportSize())!;
  expect(Math.abs(((box?.y || 0) + (box?.height || 0) / 2) - viewport.height / 2)).toBeLessThan(45);
});
