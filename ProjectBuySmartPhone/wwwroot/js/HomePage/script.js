document.addEventListener("DOMContentLoaded", function () {
    // ------------------ Locomotive Scroll + GSAP ------------------
    function loco() {
        if (typeof LocomotiveScroll === "undefined" || typeof ScrollTrigger === "undefined") return;

        gsap.registerPlugin(ScrollTrigger);

        const locoScroll = new LocomotiveScroll({
            el: document.querySelector("body"),
            smooth: true
        });

        locoScroll.on("scroll", ScrollTrigger.update);

        ScrollTrigger.scrollerProxy("body", {
            scrollTop(value) {
                return arguments.length
                    ? locoScroll.scrollTo(value, 0, 0)
                    : locoScroll.scroll.instance.scroll.y;
            },
            getBoundingClientRect() {
                return {
                    top: 0,
                    left: 0,
                    width: window.innerWidth,
                    height: window.innerHeight
                };
            },
            pinType: document.querySelector("body").style.transform ? "transform" : "fixed"
        });

        ScrollTrigger.addEventListener("refresh", () => locoScroll.update());
        ScrollTrigger.refresh();
    }

    // Chạy hàm scroll
    loco();

    // ------------------ Swiper Slider ------------------
    if (typeof Swiper !== "undefined") {
        new Swiper(".mySwiper", {
            slidesPerView: "1.2",
            centeredSlides: true,
            spaceBetween: 10,
            navigation: {
                nextEl: ".swiper-button-next",
                prevEl: ".swiper-button-prev",
            },
            autoplay: {
                delay: 2500,
                disableOnInteraction: false,
            },
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
            },
            keyboard: true,
            loop: true,
        });
    }

    // ------------------ Menu mở/đóng trên mobile ------------------
    const menu = document.querySelector(".ri-menu-line");
    const close = document.querySelector(".ri-close-line");
    const navbar = document.querySelector(".subnav");

    if (menu && close && navbar) {
        menu.addEventListener("click", function () {
            navbar.style.top = "0%";
        });

        close.addEventListener("click", function () {
            navbar.style.top = "-109%";
        });
    }

    // ------------------ Hover trên CỬA HÀNG (nav3) ------------------
    const store = document.querySelector("#Store");
    const storeMenu = document.querySelector(".nav3");

    if (store && storeMenu) {
        // Hover vào icon Cửa hàng
        store.addEventListener("mouseenter", () => {
            closeAllMenus();
            storeMenu.style.top = "5%";
            storeMenu.style.pointerEvents = "all";
        });

        // Rời icon Cửa hàng
        store.addEventListener("mouseleave", () => {
            setTimeout(() => {
                if (!storeMenu.matches(":hover")) {
                    storeMenu.style.top = "-100%";
                    storeMenu.style.pointerEvents = "none";
                }
            }, 200);
        });

        // Di chuột vào trong menu vẫn giữ mở
        storeMenu.addEventListener("mouseenter", () => {
            storeMenu.style.top = "5%";
            storeMenu.style.pointerEvents = "all";
        });

        // Rời khỏi menu hoặc vùng mờ thì ẩn đi
        storeMenu.addEventListener("mouseleave", () => {
            storeMenu.style.top = "-100%";
            storeMenu.style.pointerEvents = "none";
        });
    }

    // ------------------ Hover icon USER để mở menu tài khoản ------------------
    const userIcon = document.querySelector("#User-nav");
    const userMenu = document.querySelector(".nav-user");

    if (userIcon && userMenu) {
        // Mở khi hover icon user
        userIcon.addEventListener("mouseenter", () => {
            closeAllMenus();
            userMenu.style.top = "5%";
            userMenu.style.pointerEvents = "all";
        });

        // Rời khỏi icon user
        userIcon.addEventListener("mouseleave", () => {
            setTimeout(() => {
                if (!userMenu.matches(":hover")) {
                    userMenu.style.top = "-100%";
                    userMenu.style.pointerEvents = "none";
                }
            }, 200);
        });

        // Giữ mở khi di chuột vào menu
        userMenu.addEventListener("mouseenter", () => {
            userMenu.style.top = "5%";
            userMenu.style.pointerEvents = "all";
        });

        // Đóng khi rời khỏi menu
        userMenu.addEventListener("mouseleave", () => {
            userMenu.style.top = "-100%";
            userMenu.style.pointerEvents = "none";
        });
    }

    // ------------------ Đóng khi click vùng mờ (nav-bottom) ------------------
    document.querySelectorAll(".nav-bottom").forEach(bg => {
        bg.addEventListener("click", () => {
            bg.parentElement.style.top = "-100%";
            bg.parentElement.style.pointerEvents = "none";
        });
    });

    // ------------------ Hàm tiện ích: Đóng tất cả menu đang mở ------------------
    function closeAllMenus() {
        const allMenus = document.querySelectorAll(".nav3, .nav-user");
        allMenus.forEach(menu => {
            menu.style.top = "-100%";
            menu.style.pointerEvents = "none";
        });
    }
});
