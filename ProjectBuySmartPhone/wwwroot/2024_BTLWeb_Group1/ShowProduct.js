// ========================================
// 🛑 Ngăn trình duyệt tự động khôi phục vị trí cuộn
// ========================================
if ("scrollRestoration" in history) {
    history.scrollRestoration = "manual";
}

// ===============================
// 🎞️ SLIDER (Slick carousel)
// ===============================
$(document).ready(function () {
    $(".slider").slick({
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 3000,
        responsive: [
            {
                breakpoint: 1024,
                settings: { slidesToShow: 1, slidesToScroll: 1, infinite: true, dots: true }
            },
            { breakpoint: 768, settings: { slidesToShow: 1, slidesToScroll: 1 } },
            { breakpoint: 480, settings: { slidesToShow: 1, slidesToScroll: 1 } }
        ]
    });
});

// ===============================
// 🧩 MODAL SẢN PHẨM
// ===============================
document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("myModal");
    const links = document.querySelectorAll(".openModalLink");
    const colorContainer = document.querySelector(".color-options");

    links.forEach(link => {
        link.onclick = e => {
            e.preventDefault();

            const name = link.dataset.productName;
            const image = link.dataset.productImage;
            const colors = JSON.parse(link.dataset.productColors);

            modal.querySelector("h1").innerText = name;
            modal.querySelector("img").src = image;
            colorContainer.innerHTML = "";

            colors.forEach(c => {
                const div = document.createElement("div");
                div.style.cssText = `
                    background:${c};
                    width:30px;height:30px;
                    border:1px solid #000;
                    display:inline-block;margin:0 5px;
                `;
                div.onclick = () => console.log(`Màu đã chọn: ${c}`);
                colorContainer.appendChild(div);
            });

            modal.style.display = "flex";
        };
    });

    if (modal) {
        modal.onclick = e => {
            if (e.target === modal) closeModal();
        };
    }
});

function closeModal() {
    document.getElementById("myModal").style.display = "none";
}

// ===============================
// 🔔 POPUP THÔNG BÁO GIỎ HÀNG / TÌM KIẾM
// ===============================
document.addEventListener("DOMContentLoaded", function () {
    const popup = document.getElementById("popup");
    const content = document.querySelector(".popup-content h2");
    const close = document.querySelector(".close");
    const btnCart = document.getElementById("showPopup");
    const btnSearch = document.getElementById("searchPopup");

    if (btnCart)
        btnCart.addEventListener("click", () => {
            content.innerText = "Hiện chưa có chức năng giỏ hàng";
            popup.style.display = "block";
        });

    if (btnSearch)
        btnSearch.addEventListener("click", () => {
            content.innerText = "Hiện chưa có chức năng tìm kiếm";
            popup.style.display = "block";
        });

    if (close)
        close.addEventListener("click", () => (popup.style.display = "none"));

    window.addEventListener("click", e => {
        if (e.target === popup) popup.style.display = "none";
    });
});

// ===============================
// 💾 XỬ LÝ CHỌN DUNG LƯỢNG
// ===============================
function selectStorage(button) {
    document.querySelectorAll(".storage-btn").forEach(btn => btn.classList.remove("active"));
    button.classList.add("active");
}

// ================================
// 🖼️ SLIDER ẢNH CHÍNH (trong modal sản phẩm)
// ================================
function closeModal2() {
    const modal = document.getElementById("productModal");
    if (modal) modal.style.display = "none";
}

(function () {
    const clamp = (i, len) => (i + len) % len;

    document.querySelectorAll(".nx-slider").forEach(root => {
        const track = root.querySelector(".nx-slider__track");
        const slides = Array.from(root.querySelectorAll(".nx-slide"));
        const prev = root.querySelector(".nx-prev");
        const next = root.querySelector(".nx-next");
        const dots = Array.from(root.querySelectorAll(".nx-dot"));
        const thumbs = Array.from(root.querySelectorAll(".nx-thumb"));
        const autoplay = (root.dataset.autoplay || "false") === "true";
        const interval = parseInt(root.dataset.interval || "4000", 10);

        if (!slides.length) return;
        let index = 0, timer = null;

        const goTo = (i, animate = true) => {
            index = clamp(i, slides.length);
            track.style.transition = animate ? "transform .5s ease" : "none";
            track.style.transform = `translateX(-${index * 100}%)`;
            dots.forEach((d, di) => d.classList.toggle("is-active", di === index));
            thumbs.forEach((t, ti) => t.classList.toggle("is-active", ti === index));
        };

        const nextSlide = () => goTo(index + 1);
        const prevSlide = () => goTo(index - 1);

        next?.addEventListener("click", nextSlide);
        prev?.addEventListener("click", prevSlide);
        dots.forEach(d => d.addEventListener("click", () => goTo(+d.dataset.index)));
        thumbs.forEach(t => t.addEventListener("click", () => goTo(+t.dataset.index)));

        if (autoplay && slides.length > 1) {
            timer = setInterval(nextSlide, interval);
            root.addEventListener("mouseenter", () => clearInterval(timer));
            root.addEventListener("mouseleave", () => (timer = setInterval(nextSlide, interval)));
        }

        goTo(0, false);
    });
})();

// ================================
// 🔍 CHỈ CUỘN XUỐNG KHI TÌM KIẾM
// ================================
document.addEventListener("DOMContentLoaded", function () {
    const params = new URLSearchParams(window.location.search);
    const keyword = (params.get("keyword") || "").trim();
    const path = window.location.pathname.toLowerCase();
    const isHome = path.includes("/viewhome/trangchu/index") || path.endsWith("/viewhome/trangchu");

    if (keyword && isHome) {
        const target = document.getElementById("list-product-show");
        if (!target) return;

        const header = document.querySelector("header");
        const offset = header ? -(header.offsetHeight + 16) : -120;
        const y = target.getBoundingClientRect().top + window.pageYOffset + offset;

        window.scrollTo({ top: y, behavior: "smooth" });

        target.classList.add("highlight-search");
        setTimeout(() => target.classList.remove("highlight-search"), 2000);
    }
});

