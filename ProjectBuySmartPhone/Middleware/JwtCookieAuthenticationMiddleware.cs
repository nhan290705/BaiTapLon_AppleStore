using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectBuySmartPhone.Middleware
{
    public class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtCookieAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                    // ✅ Validate và extract claims từ JWT
                    var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = _configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    // ✅ Set User.Identity với claims từ JWT
                    context.User = principal;

                    Console.WriteLine($"✅ JWT Authentication - User authenticated: {context.User.Identity.IsAuthenticated}");
                    Console.WriteLine($"✅ UserId from 'idUser' claim: {context.User.FindFirst("idUser")?.Value}");
                }
                catch (SecurityTokenExpiredException)
                {
                    Console.WriteLine("⚠️ JWT expired - sẽ được refresh bởi RefreshJwtMiddleware");
                    // Không làm gì, để RefreshJwtMiddleware xử lý
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ JWT validation error: {ex.Message}");
                }
            }

            await _next(context);
        }
    }
}