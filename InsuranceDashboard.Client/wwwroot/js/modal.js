(() => {
  let returnFocus = null;
  let activeDialog = null;
  let lockedScrollY = 0;

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

  function lockPage() {
    if (document.body.classList.contains('modal-open')) return;
    lockedScrollY = window.scrollY || document.documentElement.scrollTop || 0;
    const scrollbarGap = Math.max(0, window.innerWidth - document.documentElement.clientWidth);
    document.body.classList.add('modal-open');
    document.documentElement.classList.add('modal-open');
    Object.assign(document.body.style, {
      position: 'fixed',
      top: `-${lockedScrollY}px`,
      left: '0',
      right: '0',
      width: '100%',
      overflow: 'hidden',
      paddingRight: scrollbarGap ? `${scrollbarGap}px` : ''
    });
  }

  function unlockPage() {
    document.body.classList.remove('modal-open');
    document.documentElement.classList.remove('modal-open');
    Object.assign(document.body.style, {
      position: '', top: '', left: '', right: '', width: '', overflow: '', paddingRight: ''
    });
    window.scrollTo(0, lockedScrollY);
  }

  function activate(dialog) {
    if (!dialog || dialog === activeDialog) return;
    if (!activeDialog) returnFocus = document.activeElement instanceof HTMLElement ? document.activeElement : null;
    activeDialog = dialog;
    lockPage();
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
    unlockPage();
    const target = returnFocus;
    returnFocus = null;
    requestAnimationFrame(() => target?.focus?.({ preventScroll: true }));
  }

  document.addEventListener('keydown', event => {
    const dialog = topDialog();
    if (!dialog) return;
    if (event.key === 'Escape') {
      const close = dialog.querySelector('.modal-close, .button-secondary');
      if (close instanceof HTMLElement) { event.preventDefault(); close.click(); }
      return;
    }
    if (event.key !== 'Tab') return;
    const items = [...dialog.querySelectorAll(focusableSelector)].filter(el => el instanceof HTMLElement && el.offsetParent !== null);
    if (!items.length) { event.preventDefault(); dialog.focus(); return; }
    const first = items[0];
    const last = items[items.length - 1];
    if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
    else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
  });

  const observer = new MutationObserver(() => {
    const dialog = topDialog();
    if (dialog) activate(dialog);
    else deactivateIfNeeded();
  });
  observer.observe(document.body, { childList: true, subtree: true });
})();
