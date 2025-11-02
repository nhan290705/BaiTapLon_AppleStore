using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectBuySmartPhone.Middleware
{
    public class JwtAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path.Value?.ToLower();

            // ✅ Những đường dẫn công khai (không cần token)
            string[] publicPaths =
            {
                "/identity/login",
                "/identity/register",
                "/identity/logout",
                "/favicon.ico"
            };

            // Nếu là public route → bỏ qua
            if (publicPaths.Any(p => path != null && path.StartsWith(p)))
            {
                base.OnActionExecuting(context);
                return;
            }

            var accessToken = context.HttpContext.Request.Cookies["AccessToken"];
            var refreshToken = context.HttpContext.Request.Cookies["RefreshToken"];

            // ✅ Nếu chưa có token → về Login
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new RedirectToRouteResult(new
                {
                    area = "Identity",
                    controller = "Login",
                    action = "Index"
                });
                return;
            }

            try
            {
                // ✅ Giải mã token để đọc role
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);

                // Lấy role trong claim (ví dụ: "ADMIN", "USER")
                var roleClaim = token.Claims
                    .FirstOrDefault(c => c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                    ?.Value;

                // Lấy idUser để debug
                var idUserClaim = token.Claims
                    .FirstOrDefault(c => c.Type.Equals("idUser", StringComparison.OrdinalIgnoreCase))
                    ?.Value;

                // ✅ Nếu cố vào khu vực Admin mà role không phải ADMIN
                if (path != null && path.StartsWith("/Admin") &&
                    !string.Equals(roleClaim, "ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new RedirectToRouteResult(new
                    {
                        area = "Identity",
                        controller = "Login",
                        action = "AccessDenied"
                    });
                    return;
                }

                // ✅ Nếu cố vào khu vực User mà role không phải USER
                if (path != null && path.StartsWith("/user") &&
                    !string.Equals(roleClaim, "USER", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new RedirectToRouteResult(new
                    {
                        area = "Identity",
                        controller = "Login",
                        action = "AccessDenied"
                    });
                    return;
                }

                // (Tuỳ chọn) Debug in ra role để test
                Console.WriteLine($"[JWT] Path: {path}, Role: {roleClaim}, UserId: {idUserClaim}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JWT ERROR] {ex.Message}");
                context.Result = new RedirectToRouteResult(new
                {
                    area = "Identity",
                    controller = "Login",
                    action = "Index"
                });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
