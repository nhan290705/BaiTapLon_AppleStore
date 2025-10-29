using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

            // Nếu URL hiện tại là public thì bỏ qua kiểm tra
            if (publicPaths.Any(p => path != null && path.StartsWith(p)))
            {
                base.OnActionExecuting(context);
                return;
            }

            var accessToken = context.HttpContext.Request.Cookies["AccessToken"];
            var refreshToken = context.HttpContext.Request.Cookies["RefreshToken"];

            // ✅ Nếu chưa có token => ép về trang Login
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

            base.OnActionExecuting(context);
        }
    }
}
