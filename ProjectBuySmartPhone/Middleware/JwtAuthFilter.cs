using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectBuySmartPhone.Middleware
{
    public class JwtAuthFilter : ActionFilterAttribute
    {
        private readonly IConfiguration _config;

        public JwtAuthFilter(IConfiguration config)
        {
            _config = config.GetSection("Jwt");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var hasAllowAnonymous = descriptor?.MethodInfo
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).Any() ?? false;

            if (hasAllowAnonymous)
            {
                base.OnActionExecuting(context);
                return;
            }

            var path = context.HttpContext.Request.Path.Value?.ToLower();

            // ✅ Các đường dẫn công khai
            string[] publicPaths =
            {
                "/identity/login",
                "/identity/register",
                "/identity/logout",
                "/favicon.ico"
            };
            if (publicPaths.Any(p => path != null && path.StartsWith(p)))
            {
                base.OnActionExecuting(context);
                return;
            }

            // ✅ Lấy token
            var accessToken = context.HttpContext.Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                context.Result = new RedirectToRouteResult(new
                {
                    area = "Identity",
                    controller = "Login",
                    action = "Index"
                });
                return;
            }

            // ✅ Xác thực token
            try
            {
                var jwtKey = _config["Key"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtKey);

                var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                context.HttpContext.User = principal; // ✅ Gắn user hợp lệ
            }
            catch (SecurityTokenExpiredException)
            {
                // ✅ Token hết hạn → redirect login
                context.Result = new RedirectToRouteResult(new
                {
                    area = "Identity",
                    controller = "Login",
                    action = "Index"
                });
                return;
            }
            catch
            {
                // ✅ Token lỗi → redirect login
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
