document.addEventListener("DOMContentLoaded", function () {
    const msg = document.getElementById("msg");
    const form = document.getElementById("registerForm");
    const captchaText = document.getElementById("captchaText");
    const refreshBtn = document.getElementById("refreshBtn");
    const captchaInput = document.getElementById("captchaInput");
    const agreeTerms = document.getElementById("agreeTerms");

    function generateCaptcha() {
        const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        let text = "";
        for (let i = 0; i < 5; i++) {
            text += chars[Math.floor(Math.random() * chars.length)];
        }
        captchaText.textContent = text;
    }

    generateCaptcha();

    refreshBtn.addEventListener("click", (e) => {
        e.preventDefault();
        generateCaptcha();
        captchaInput.value = "";
        msg.textContent = "";
    });

    form.addEventListener("submit", function (e) {
        let valid = true;

        // Kiểm tra ASP.NET validation
        if (!$(form).valid()) valid = false;

        // Kiểm tra Captcha
        const input = captchaInput.value.trim().toUpperCase();
        const expected = captchaText.textContent.toUpperCase();
        if (input !== expected) {
            valid = false;
            msg.textContent = "❌ Mã xác thực không đúng!";
            msg.style.color = "#e63946";
            captchaInput.value = "";
            generateCaptcha();
        }

        // Kiểm tra tick điều khoản
        if (!agreeTerms.checked) {
            valid = false;
            msg.textContent = "⚠️ Vui lòng đồng ý với điều khoản trước khi tiếp tục.";
            msg.style.color = "#e63946";
        }

        if (!valid) e.preventDefault();
    });

    window.addEventListener("pageshow", () => {
        generateCaptcha();
        captchaInput.value = "";
        msg.textContent = "";
    });
});
