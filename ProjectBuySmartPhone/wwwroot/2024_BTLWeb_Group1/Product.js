// ===============================
// 🎞️ SLIDER (Slick carousel)
// ===============================
$(document).ready(function () {
    $('.slider').slick({
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
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }
        ]
    });
});


// ===============================
// 🧩 MODAL SẢN PHẨM
// ===============================
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.openModalLink').forEach(link => {
        link.onclick = function (event) {
            event.preventDefault();

            const productName = this.getAttribute('data-product-name');
            const productImage = this.getAttribute('data-product-image');
            const productColors = JSON.parse(this.getAttribute('data-product-colors'));

            // Cập nhật modal
            document.querySelector('#myModal h1').innerText = productName;
            document.querySelector('#myModal img').src = productImage;

            // Cập nhật màu sắc
            const colorOptionsContainer = document.querySelector('.color-options');
            colorOptionsContainer.innerHTML = '';
            productColors.forEach(color => {
                const colorDiv = document.createElement('div');
                colorDiv.style.backgroundColor = color;
                colorDiv.style.width = '30px';
                colorDiv.style.height = '30px';
                colorDiv.style.border = '1px solid #000';
                colorDiv.style.display = 'inline-block';
                colorDiv.style.margin = '0 5px';
                colorDiv.onclick = () => console.log(`Màu đã chọn: ${color}`);
                colorOptionsContainer.appendChild(colorDiv);
            });

            document.getElementById('myModal').style.display = 'flex';
        };
    });

    // Đóng modal khi nhấn ra ngoài
    const modal = document.getElementById('myModal');
    if (modal) {
        modal.onclick = function (event) {
            if (event.target === modal) closeModal();
        };
    }
});

function closeModal() {
    document.getElementById('myModal').style.display = 'none';
}


// ===============================
// 🔔 MODAL THÔNG BÁO POPUP
// ===============================
document.addEventListener("DOMContentLoaded", function () {
    var cartButton = document.getElementById("showPopup");
    var searchButton = document.getElementById("searchPopup");
    var popup = document.getElementById("popup");
    var popupContent = document.querySelector(".popup-content h2");
    var close = document.querySelector(".close");

    if (cartButton) {
        cartButton.addEventListener("click", function () {
            popupContent.innerText = "Hiện chưa có chức năng giỏ hàng";
            popup.style.display = "block";
        });
    }

    if (searchButton) {
        searchButton.addEventListener("click", function () {
            popupContent.innerText = "Hiện chưa có chức năng tìm kiếm";
            popup.style.display = "block";
        });
    }

    if (close) {
        close.addEventListener("click", function () {
            popup.style.display = "none";
        });
    }

    window.addEventListener("click", function (event) {
        if (event.target === popup) popup.style.display = "none";
    });
});


// ===============================
// 📦 XỬ LÝ DUNG LƯỢNG SP
// ===============================
function selectStorage(button, storage) {
    document.querySelectorAll('.storage-btn').forEach(btn => {
        btn.classList.remove('active');
    });
    button.classList.add('active');
}


// ===============================
// 🔍 CUỘN XUỐNG CHỈ KHI CÓ TÌM KIẾM
// ===============================
document.addEventListener("DOMContentLoaded", function () {
    const params = new URLSearchParams(window.location.search);
    const keyword = (params.get("keyword") || "").trim();

    // Kiểm tra có đang ở đúng trang chủ (Index)
    const path = window.location.pathname.toLowerCase();
    const isHomeIndex =
        path.includes("/viewhome/trangchu/index") ||
        path.endsWith("/viewhome/trangchu");

    // ✅ Chỉ cuộn khi có keyword và đang ở đúng trang chủ
    if (keyword && isHomeIndex) {
        const target = document.getElementById("list-product-show") || document.querySelector(".spip");
        if (!target) return;

        const header = document.querySelector("header");
        const yOffset = header ? -(header.offsetHeight + 16) : -120;
        const y = target.getBoundingClientRect().top + window.pageYOffset + yOffset;

        window.scrollTo({
            top: y,
            behavior: "smooth"
        });

        // (Tùy chọn) Hiệu ứng highlight phần kết quả
        target.classList.add("highlight-search");
        setTimeout(() => target.classList.remove("highlight-search"), 2000);
    }
});
