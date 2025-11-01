function initStoreNav() {
    console.log("Nav.js initialized");

    // Hàm đóng tất cả menu
    function closeAllMenus() {
        document.querySelectorAll(".nav3, .nav-user").forEach(menu => {
            menu.style.top = "-100%";
            menu.style.pointerEvents = "none";
        });
    }

    const store = document.querySelector("#Store");
    const storeMenu = document.querySelector(".nav3");
    const userIcon = document.querySelector("#User-nav");
    const userMenu = document.querySelector(".nav-user");

    if (store && storeMenu) {
        store.addEventListener("mouseenter", () => {
            closeAllMenus();
            storeMenu.style.top = "40px";
            storeMenu.style.pointerEvents = "all";
        });
        store.addEventListener("mouseleave", () => {
            setTimeout(() => {
                if (!storeMenu.matches(":hover")) {
                    storeMenu.style.top = "-100%";
                    storeMenu.style.pointerEvents = "none";
                }
            }, 200);
        });
        storeMenu.addEventListener("mouseleave", () => {
            storeMenu.style.top = "-100%";
            storeMenu.style.pointerEvents = "none";
        });
    }

    if (userIcon && userMenu) {
        userIcon.addEventListener("mouseenter", () => {
            closeAllMenus();
            userMenu.style.top = "40px";
            userMenu.style.pointerEvents = "all";
        });
        userIcon.addEventListener("mouseleave", () => {
            setTimeout(() => {
                if (!userMenu.matches(":hover")) {
                    userMenu.style.top = "-100%";
                    userMenu.style.pointerEvents = "none";
                }
            }, 200);
        });
        userMenu.addEventListener("mouseleave", () => {
            userMenu.style.top = "-100%";
            userMenu.style.pointerEvents = "none";
        });
    }

    console.log("Nav hover logic bound");
}

// Tự gọi hàm nếu load trực tiếp (reload)
document.addEventListener("DOMContentLoaded", initStoreNav);
