// wwwroot/js/register.js
document.addEventListener("DOMContentLoaded", function () {
    // ----- Captcha -----
    const captchaText = document.getElementById("captchaText");
    const refreshBtn = document.getElementById("refreshBtn");

    function generateCaptcha() {
        const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        let text = "";
        for (let i = 0; i < 5; i++) {
            text += chars[Math.floor(Math.random() * chars.length)];
        }
        captchaText.textContent = text;
    }

    if (refreshBtn) refreshBtn.addEventListener("click", generateCaptcha);
    generateCaptcha();

    // ----- Màn hình / phần tử -----
    const registerScreen = document.getElementById("registerScreen");
    const verifyScreen = document.getElementById("verifyScreen");
    const form = document.getElementById("registerForm");
    const msg = document.getElementById("msg");

    // ----- Submit đăng ký -----
    if (form) {
        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            // Check captcha
            const captchaInput = document.getElementById("captchaInput");
            if (!captchaInput || captchaInput.value.trim().toUpperCase() !== captchaText.textContent.toUpperCase()) {
                if (msg) {
                    msg.textContent = "Mã xác thực không đúng!";
                    msg.style.color = "red";
                }
                generateCaptcha();
                return;
            }

            // Thu thập dữ liệu: chú ý đúng tên thuộc tính với DTO UserRegister.cs
            const lastname = document.getElementById("lastname")?.value.trim() ?? "";
            const firstname = document.getElementById("firstname")?.value.trim() ?? "";
            const email = document.getElementById("email")?.value.trim() ?? "";
            const phone = document.getElementById("phone")?.value.trim() ?? "";
            const password = document.getElementById("password")?.value.trim() ?? "";
            const confirmPwd = document.getElementById("confirmPassword")?.value.trim() ?? "";

            const user = {
                // FullName = "Họ Tên" đúng thứ tự form của bạn
                FullName: `${lastname} ${firstname}`,
                Email: email,
                PhoneNumber: phone,
                // Tuỳ bạn: tách username từ email, hoặc thêm input Username riêng
                Username: email.split("@")[0],
                Password: password,
                ConfirmPassword: confirmPwd
            };

            try {
                const res = await fetch("/Identity/Register/register", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(user)
                });

                if (res.ok) {
                    if (msg) msg.textContent = "";
                    // Hiệu ứng chuyển cảnh
                    if (registerScreen) {
                        registerScreen.classList.remove("visible");
                        registerScreen.classList.add("hidden");
                    }
                    setTimeout(() => {
                        if (verifyScreen) verifyScreen.classList.add("active");
                    }, 600);
                } else {
                    let text = "Đăng ký thất bại.";
                    try {
                        const data = await res.json();
                        text = data?.message || text;
                    } catch { }
                    if (msg) {
                        msg.textContent = text;
                        msg.style.color = "red";
                    }
                }
            } catch (err) {
                if (msg) {
                    msg.textContent = "Không thể kết nối đến máy chủ.";
                    msg.style.color = "red";
                }
            }
        });
    }

    // ----- Xử lý OTP giả lập -----
    const verifyBtn = document.getElementById("verifyBtn");
    const verifyMsg = document.getElementById("verifyMsg");

    // auto focus từng ô OTP
    const otpInputs = Array.from(document.querySelectorAll(".otp-box input"));
    otpInputs.forEach((ip, idx) => {
        ip.addEventListener("input", () => {
            ip.value = ip.value.replace(/\D/g, "").slice(0, 1);
            if (ip.value && idx < otpInputs.length - 1) otpInputs[idx + 1].focus();
        });
        ip.addEventListener("keydown", (e) => {
            if (e.key === "Backspace" && !ip.value && idx > 0) otpInputs[idx - 1].focus();
        });
    });

    if (verifyBtn) {
        verifyBtn.addEventListener("click", () => {
            if (verifyMsg) {
                verifyMsg.textContent = "Tài khoản của bạn đã được xác minh thành công!";
                verifyMsg.style.color = "#0071e3";
            }
            setTimeout(() => {
                window.location.href = "/Identity/Login";
            }, 1500);
        });
    }
});
