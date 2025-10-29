using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProjectBuySmartPhone.Configuration
{
    public class JwtConfig
    {
        public static void BuildJwtConfig(WebApplicationBuilder builder)
        {
            // Lấy thông tin cấu hình JWT từ file appsettings.json
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var audience = jwtSettings["Audience"];
            var issuer = jwtSettings["Issuer"];
            // Đăng ký dịch vụ xác thực 
            builder.Services.AddAuthentication(options =>
            {
                // Chỉ định mặc định cơ chế xác thực và challenge là JWT Bearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //  Cấu hình cơ chế xác thực JWT Bearer
            .AddJwtBearer(options =>
            {
                // Các quy tắc kiểm tra và xác thực token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ClockSkew = TimeSpan.Zero // // Không cho phép trễ hạn (hết hạn là hết ngay)
                };
                //Cấu hình các sự kiện trong quá trình xác thực JWT
                options.Events = new JwtBearerEvents
                {
                    // Khi nhận request la se đọc token từ Cookie (thay vì Header)
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["AccessToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken; // Gán token cho hệ thống xác thực
                        }
                        return Task.CompletedTask;
                    },
                    // Khi xác thực thất bại (token sai, hết hạn, không đúng key, v.v.)
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    // Khi xác thực thành công (token hợp lệ)
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated for: " );
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();
        }
    }
}
