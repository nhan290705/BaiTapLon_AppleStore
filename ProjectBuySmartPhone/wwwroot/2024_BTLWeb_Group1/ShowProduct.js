
    function closeModal2() {
        document.getElementById("productModal").style.display = "none";

        }

    (function () {
          // Helper: clamp index
          const clampIndex = (i, len) => (i + len) % len;

    document.querySelectorAll('.nx-slider').forEach(initSlider);

    function initSlider(root) {
            const track   = root.querySelector('.nx-slider__track');
    const slides  = Array.from(root.querySelectorAll('.nx-slide'));
    const prevBtn = root.querySelector('.nx-prev');
    const nextBtn = root.querySelector('.nx-next');
    const dots    = Array.from(root.querySelectorAll('.nx-dot'));
    const thumbs  = Array.from(root.querySelectorAll('.nx-thumb'));
    const autoplay = (root.dataset.autoplay || 'false') === 'true';
    const interval = parseInt(root.dataset.interval || '4000', 10);

    if (slides.length === 0) return;

    let index = 0;
    let timer = null;
    let isPointerDown = false;
    let startX = 0;
    let currentX = 0;
    let deltaX = 0;

            // Set ARIA ids
            slides.forEach((s, i) => s.id = `slide-${i}`);

    function goTo(i, animate = true) {
        index = clampIndex(i, slides.length);
    track.style.transition = animate ? 'transform .5s ease' : 'none';
    track.style.transform = `translateX(-${index * 100}%)`;

              slides.forEach((s, si) => s.setAttribute('aria-hidden', si !== index));
              dots.forEach((d, di) => {
        d.classList.toggle('is-active', di === index);
    d.setAttribute('aria-selected', String(di === index));
              });
              thumbs.forEach((t, ti) => t.classList.toggle('is-active', ti === index));
            }

    function next() {goTo(index + 1); }
    function prev() {goTo(index - 1); }

    // Events
    nextBtn?.addEventListener('click', next);
    prevBtn?.addEventListener('click', prev);

            dots.forEach(d => d.addEventListener('click', () => goTo(+d.dataset.index)));
            thumbs.forEach(t => t.addEventListener('click', () => goTo(+t.dataset.index)));

            // Keyboard
            root.addEventListener('keydown', (e) => {
              if (e.key === 'ArrowRight') next();
    if (e.key === 'ArrowLeft')  prev();
            });
    root.setAttribute('tabindex', '0');

    // Autoplay
    function start() {
              if (!autoplay || slides.length < 2) return;
    stop();
    timer = setInterval(next, interval);
            }
    function stop() { if (timer) {clearInterval(timer); timer = null; } }
    root.addEventListener('mouseenter', stop);
    root.addEventListener('mouseleave', start);
    start();

    // Touch / drag
    const viewport = root.querySelector('.nx-slider__viewport');
            viewport.addEventListener('pointerdown', (e) => {
        isPointerDown = true;
    startX = e.clientX;
    deltaX = 0;
    track.style.transition = 'none';
    viewport.setPointerCapture(e.pointerId);
            });
            viewport.addEventListener('pointermove', (e) => {
              if (!isPointerDown) return;
    currentX = e.clientX;
    deltaX = currentX - startX;
    track.style.transform = `translateX(calc(-${index * 100}% + ${deltaX}px))`;
            });
            viewport.addEventListener('pointerup', (e) => {
              if (!isPointerDown) return;
    isPointerDown = false;
    viewport.releasePointerCapture(e.pointerId);

    const threshold = viewport.clientWidth * 0.15; // 15%
              if (Math.abs(deltaX) > threshold) {
        deltaX < 0 ? next() : prev();
              } else {
        goTo(index); // snap back
              }
            });
            viewport.addEventListener('pointercancel', () => {
        isPointerDown = false;
    goTo(index);
            });

    // Initial
    goTo(0, false);
          }
        })();
document.addEventListener("DOMContentLoaded", function () {
    const target = document.getElementById("list-product-show"); // id của section bạn muốn cuộn đến
    if (target) {
        const yOffset = -120;
        const y = target.getBoundingClientRect().top + window.pageYOffset + yOffset;

        window.scrollTo({
            top: y,
            behavior: "smooth"
        });
    }
});