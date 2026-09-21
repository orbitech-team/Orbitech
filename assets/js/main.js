/**
 * OrbiTech Main Interactions
 * Handles: Mobile Drawer, Tabs, Product Gallery, Quantity Toggle, Swatches
 */

document.addEventListener('DOMContentLoaded', () => {
  
  // 1. Mobile Drawer Toggle
  const navToggle = document.querySelector('.nav-toggle');
  const drawer = document.querySelector('.drawer');
  const drawerClose = document.querySelector('.drawer-close');

  function setDrawer(open) {
    if (!drawer) return;
    drawer.classList.toggle('is-open', open);
    drawer.setAttribute('aria-hidden', String(!open));
    if (navToggle) navToggle.setAttribute('aria-expanded', String(open));
    document.body.style.overflow = open ? 'hidden' : '';
  }

  if (navToggle) {
    navToggle.addEventListener('click', () => setDrawer(true));
  }

  if (drawerClose) {
    drawerClose.addEventListener('click', () => setDrawer(false));
  }

  // Close on Escape
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && drawer && drawer.classList.contains('is-open')) setDrawer(false);
  });

  // 2. Tabs Active State
  const tabs = document.querySelectorAll('.tab');
  tabs.forEach(tab => {
    tab.addEventListener('click', () => {
      const parent = tab.parentElement;
      parent.querySelectorAll('.tab').forEach(t => {
        t.classList.remove('is-active');
        t.setAttribute('aria-selected', 'false');
      });
      tab.classList.add('is-active');
      tab.setAttribute('aria-selected', 'true');
    });
  });

  // 3. Product Gallery (Thumbnail Switcher)
  const thumbBtns = document.querySelectorAll('.gallery-thumbs button');
  const mainImg = document.querySelector('.gallery-main img');

  thumbBtns.forEach(btn => {
    btn.addEventListener('click', () => {
      thumbBtns.forEach(b => b.classList.remove('is-active'));
      btn.classList.add('is-active');
      const img = btn.querySelector('img');
      if (img && mainImg) {
        mainImg.src = img.src.replace(/w=\d+/, 'w=900');
      }
    });
  });

  // 4. Quantity Adjustments
  const qtyBlocks = document.querySelectorAll('.qty');
  qtyBlocks.forEach(block => {
    const input = block.querySelector('input');
    const btnMinus = block.querySelector('[data-act="-"]');
    const btnPlus = block.querySelector('[data-act="+"]');
    
    if (btnMinus) {
      btnMinus.addEventListener('click', () => {
        let val = parseInt(input.value) || 1;
        if (val > 1) input.value = val - 1;
      });
    }
    if (btnPlus) {
      btnPlus.addEventListener('click', () => {
        let val = parseInt(input.value) || 1;
        input.value = val + 1;
      });
    }
  });

  // 5. Visual Option Selectors (Swatches & Pills)
  const swatchGroups = document.querySelectorAll('.color-swatches, .option-pills');
  swatchGroups.forEach(group => {
    group.querySelectorAll('button').forEach(btn => {
      btn.addEventListener('click', () => {
        group.querySelectorAll('button').forEach(b => b.classList.remove('is-active'));
        btn.classList.add('is-active');
      });
    });
  });

});


function doHeaderSearch() {
    var term = document.getElementById('headerSearch').value.trim();
    window.location.href = 'Shop.aspx?search=' + encodeURIComponent(term);
}
document.getElementById('headerSearch').addEventListener('keydown', function (e) {
    if (e.key === 'Enter') { e.preventDefault(); doHeaderSearch(); }
});