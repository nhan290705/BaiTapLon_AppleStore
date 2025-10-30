using Microsoft.IdentityModel.Tokens;
using ProjectBuySmartPhone.Dtos;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectBuySmartPhone.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        private readonly ILogger<JwtHelper> _logger;
        private  string? _jwtKey;
        private  string _accessTokenExpiredMintute;
        private   string _refreshTokenExpiredDay;
        private string _jwtIssuer;
        private string _jwtAudience;
        public JwtHelper(IConfiguration configuration, MyDbContext context, ILogger<JwtHelper> logger)
        {
            _configuration = configuration.GetSection("Jwt");
            _context = context;
            _logger = logger;
            _jwtKey = _configuration["Key"];
            _accessTokenExpiredMintute = _configuration["AccessTokenDurationMinute"];
            _refreshTokenExpiredDay = _configuration["RefreshTokenDurationDays"];
            _jwtIssuer = _configuration["Issuer"];
            _jwtAudience = _configuration["Audience"];
        }
        //test random 64bit key
        public static string GenerateJwtKey(int size = 64)
        {
            var keyByes = new byte[size];
            using (var generator = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                generator.GetBytes(keyByes);
                return Convert.ToBase64String(keyByes);
            }

        }
        public Token GenerateTokens(int userId)
        {
            return new Token
            {
                accessToken = GenerateAccessToken(userId),
                refreshToken = GenerateRefreshToken(userId)
            };
        }
        //gen access token
        private string GenerateAccessToken(int userId)
        {
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var roles = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Role)
                .FirstOrDefault();
            string idClaim = _context.Users.FirstOrDefault(u => u.UserId == userId)?.UserId.ToString() ?? "";
            var claims = new List<Claim>
            {
                new Claim("idUser", userId.ToString()),
                new Claim("idClaim", idClaim)
            };
            //them claim role
            claims.Add(new Claim(ClaimTypes.Role, roles));
            //gan claim, signature cho token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtAudience,
                Issuer = _jwtIssuer,
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_accessTokenExpiredMintute)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }
        private string GenerateRefreshToken(int userId)
        {
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var roles = _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Role)
                .FirstOrDefault();
            string idClaim = _context.Users.FirstOrDefault(u => u.UserId == userId)?.UserId.ToString() ?? "";
            var claims = new List<Claim>
            {
                new Claim("idUser", userId.ToString()),
                new Claim("idClaim", idClaim)
            };
            //them claim role
            claims.Add(new Claim(ClaimTypes.Role, roles));
            //gan claim, signature cho token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtAudience,
                Issuer = _jwtIssuer,
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_refreshTokenExpiredDay)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //add refresh token vao db

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.WriteToken(token);
            
            return refreshToken;
        }
        public Token? refreshTokens(string oldRefreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            try
            {
                //  Xác thực refresh token bằng key gốc
                var principal = tokenHandler.ValidateToken(oldRefreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true, // kiểm tra hạn luôn
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                //  Lấy claim idUser
                var userIdClaim = principal.FindFirst("idUser");
                if (userIdClaim == null)
                {
                    _logger.LogWarning("refresh token khong chua user id");
                    return null;
                }

                int userId = int.Parse(userIdClaim.Value);

                //  Kiểm tra user tồn tại trong DB
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    _logger.LogWarning("User không tồn tại cho refresh token");
                    return null;
                }

                //  Sinh access token mới, giữ refresh token cũ
                var newAccessToken = GenerateAccessToken(userId);

                _logger.LogInformation($" Access token mới được sinh cho user {userId}");

                return new Token
                {
                    accessToken = newAccessToken,
                    refreshToken = oldRefreshToken
                };
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning(" Refresh token đã hết hạn");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($" Lỗi refresh token: {ex.Message}");
                return null;
            }
        }


    }
}
