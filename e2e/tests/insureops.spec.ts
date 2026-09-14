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

test('claim detail and create dialog preserve context, lock scroll, and restore focus', async ({ page }, testInfo) => {
  await page.goto('claims'); await ready(page);
  await page.getByText('CLM-10482', { exact: true }).click();
  await expect(page.getByLabel('Claim details')).toBeVisible();
  await page.screenshot({ path: testInfo.outputPath('claim-detail.png'), fullPage: true });
  const trigger = page.getByRole('button', { name: 'New claim' });
  await trigger.click();
  const dialog = page.getByRole('dialog', { name: 'Create claim' });
  await expect(dialog).toBeVisible();
  await expect(page.locator('body')).toHaveClass(/modal-open/);
  await expect(page.locator('body')).toHaveCSS('position', 'fixed');
  const topBefore = await page.locator('body').evaluate(el => getComputedStyle(el).top);
  await page.mouse.wheel(0, 900);
  await expect(page.locator('body')).toHaveCSS('top', topBefore);
  await page.screenshot({ path: testInfo.outputPath('new-claim-modal.png'), fullPage: true });
  await page.keyboard.press('Escape');
  await expect(dialog).toBeHidden();
  await expect(page.locator('body')).not.toHaveClass(/modal-open/);
  await expect(trigger).toBeFocused();
});

test('claim edits persist in session and recalculate overview data', async ({ page }) => {
  await page.goto('claims'); await ready(page);
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await page.getByRole('button', { name: 'Edit claim' }).click();
  const dialog = page.getByRole('dialog', { name: /Edit CLM-10482/i });
  await dialog.getByLabel('Exposure').fill('75000');
  await dialog.getByLabel('Status').selectOption('Open');
  await dialog.getByRole('button', { name: 'Save changes' }).click();
  await expect(page.getByRole('status')).toContainText('CLM-10482 updated');
  await expect(page.getByLabel('Claim details')).toContainText('$75,000');
  await page.goto(''); await ready(page);
  await expect(page.getByRole('row').filter({ hasText: 'CLM-10482' })).toContainText('$75,000');
});

test('policy maintenance updates premium and status-derived data', async ({ page }) => {
  await page.goto('policies'); await ready(page);
  await page.getByRole('row').filter({ hasText: 'POL-48392' }).click();
  await page.getByRole('button', { name: 'Edit policy' }).click();
  const dialog = page.getByRole('dialog', { name: /Edit POL-48392/i });
  await dialog.getByLabel('Monthly premium').fill('350');
  await dialog.getByLabel('Status').selectOption('true');
  await dialog.getByRole('button', { name: 'Save changes' }).click();
  await expect(page.getByRole('status')).toContainText('POL-48392 updated');
  await expect(page.getByLabel('Policy details')).toContainText('$350');
});

test('customer create flow gives visible feedback', async ({ page }, testInfo) => {
  await page.goto('customers'); await ready(page);
  await page.getByRole('button', { name: 'New customer' }).click();
  const dialog = page.getByRole('dialog', { name: 'Create customer' });
  await expect(dialog).toBeVisible();
  await dialog.getByLabel('Full name').fill('Morgan Reed');
  await dialog.getByLabel('Email').fill('morgan.reed@example.com');
  await dialog.getByLabel('State').fill('CO');
  await dialog.getByRole('button', { name: 'Create customer' }).click();
  await expect(page.getByRole('status')).toContainText('Customer created');
  await page.screenshot({ path: testInfo.outputPath('customer-success-feedback.png'), fullPage: true });
});

test('claim archive and restore lifecycle is recoverable', async ({ page }, testInfo) => {
  await page.goto('claims'); await ready(page);
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await page.getByRole('button', { name: /archive claim/i }).click();
  const confirm = page.getByRole('alertdialog');
  await expect(confirm).toBeVisible();
  await page.screenshot({ path: testInfo.outputPath('archive-confirmation.png'), fullPage: true });
  await confirm.getByRole('button', { name: /archive claim/i }).click();
  await expect(page.getByRole('status')).toContainText('archived');
  await page.getByLabel('Filter claim lifecycle').selectOption('archived');
  await page.getByRole('row').filter({ hasText: 'CLM-10482' }).click();
  await page.getByRole('button', { name: /restore claim/i }).click();
  await expect(page.getByRole('status')).toContainText('restored');
});

test('mobile navigation stays in viewport and modal locks the document', async ({ page }, testInfo) => {
  test.skip(!testInfo.project.name.includes('mobile'), 'mobile-specific assertion');
  await page.goto('claims'); await ready(page);
  const nav = page.getByRole('navigation', { name: 'Primary navigation' });
  const navBox = await nav.boundingBox(); const viewport = page.viewportSize();
  expect(navBox && viewport).toBeTruthy();
  if (navBox && viewport) expect(navBox.x + navBox.width).toBeLessThanOrEqual(viewport.width + 1);
  await page.screenshot({ path: testInfo.outputPath('mobile-navigation.png'), fullPage: true });
  await page.getByRole('button', { name: 'New claim' }).click();
  const dialog = page.getByRole('dialog', { name: 'Create claim' });
  await expect(dialog).toBeVisible();
  await expect(page.locator('body')).toHaveCSS('position', 'fixed');
  const box = await dialog.boundingBox(); expect(box && viewport).toBeTruthy();
  if (box && viewport) { expect(box.x).toBeGreaterThanOrEqual(0); expect(box.y).toBeGreaterThanOrEqual(0); expect(box.x + box.width).toBeLessThanOrEqual(viewport.width + 1); expect(box.y + box.height).toBeLessThanOrEqual(viewport.height + 1); expect(Math.abs((box.y + box.height / 2) - viewport.height / 2)).toBeLessThan(50); }
  await page.screenshot({ path: testInfo.outputPath('mobile-new-claim.png'), fullPage: true });
});

test('reduced-motion preference disables decorative motion', async ({ page }) => {
  await page.emulateMedia({ reducedMotion: 'reduce' });
  await page.goto('claims'); await ready(page);
  const duration = await page.locator('.page-content').evaluate(el => getComputedStyle(el).animationDuration);
  expect(['0s', '0.001s']).toContain(duration);
});
