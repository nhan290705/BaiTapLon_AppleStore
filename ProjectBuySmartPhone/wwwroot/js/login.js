$(document).ready(function () {
    const username = $("#Username");
    const password = $("#Password");
    const loginBtn = $("button[type='submit']");

    // 🎯 Khi người dùng focus vào input → đổi màu border (Apple style)
    $("input").on("focus", function () {
        $(this).css("border-color", "#0071e3");
    }).on("blur", function () {
        $(this).css("border-color", "#ccc");
    });

    // 💡 Khi form được submit → thêm hiệu ứng "loading" cho nút đăng nhập
    $("form").on("submit", function () {
        if ($(this).valid()) {
            loginBtn.prop("disabled", true);
            loginBtn.text("Đang đăng nhập...");
            loginBtn.css("background-color", "#005bb5");
        }
    });

    // 🔄 Reset nút khi ModelState có lỗi (sai mật khẩu chẳng hạn)
    if ($(".field-validation-error").length > 0) {
        loginBtn.prop("disabled", false);
        loginBtn.text("Đăng nhập");
        loginBtn.css("background-color", "#1d1d1f");
    }

    // ✨ Hiệu ứng nhỏ: nhấn Enter ở password cũng kích hoạt submit
    password.on("keypress", function (e) {
        if (e.key === "Enter") {
            $("form").submit();
        }
    });
});