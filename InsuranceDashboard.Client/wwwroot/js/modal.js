(() => {
  let returnFocus = null;
  let activeDialog = null;

  const focusableSelector = [
    'a[href]',
    'button:not([disabled])',
    'input:not([disabled])',
    'select:not([disabled])',
    'textarea:not([disabled])',
    '[tabindex]:not([tabindex="-1"])'
  ].join(',');

  function topDialog() {
    const dialogs = [...document.querySelectorAll('.modal-backdrop [role="dialog"], .modal-backdrop [role="alertdialog"]')];
    return dialogs.at(-1) || null;
  }

  function activate(dialog) {
    if (!dialog || dialog === activeDialog) return;
    if (!activeDialog) returnFocus = document.activeElement instanceof HTMLElement ? document.activeElement : null;
    activeDialog = dialog;
    document.body.classList.add('modal-open');
    if (!dialog.hasAttribute('tabindex')) dialog.setAttribute('tabindex', '-1');
    requestAnimationFrame(() => {
      const preferred = dialog.querySelector('input:not([disabled]), select:not([disabled]), button:not([disabled]), [href]');
      (preferred || dialog).focus({ preventScroll: true });
    });
  }

  function deactivateIfNeeded() {
    const next = topDialog();
    if (next) {
      activate(next);
      return;
    }
    if (!activeDialog) return;
    activeDialog = null;
    document.body.classList.remove('modal-open');
    const target = returnFocus;
    returnFocus = null;
    requestAnimationFrame(() => target?.focus?.({ preventScroll: true }));
  }

  document.addEventListener('keydown', event => {
    const dialog = topDialog();
    if (!dialog) return;

    if (event.key === 'Escape') {
      const close = dialog.querySelector('.modal-close, .button-secondary');
      if (close instanceof HTMLElement) {
        event.preventDefault();
        close.click();
      }
      return;
    }

    if (event.key !== 'Tab') return;
    const items = [...dialog.querySelectorAll(focusableSelector)].filter(el => el instanceof HTMLElement && el.offsetParent !== null);
    if (!items.length) {
      event.preventDefault();
      dialog.focus();
      return;
    }
    const first = items[0];
    const last = items[items.length - 1];
    if (event.shiftKey && document.activeElement === first) {
      event.preventDefault();
      last.focus();
    } else if (!event.shiftKey && document.activeElement === last) {
      event.preventDefault();
      first.focus();
    }
  });

  const observer = new MutationObserver(() => {
    const dialog = topDialog();
    if (dialog) activate(dialog);
    else deactivateIfNeeded();
  });

  observer.observe(document.body, { childList: true, subtree: true });
})();
