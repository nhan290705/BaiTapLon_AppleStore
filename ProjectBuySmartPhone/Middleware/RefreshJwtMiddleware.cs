using ProjectBuySmartPhone.Helpers;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace ProjectBuySmartPhone.Middleware
{
    public class RefreshJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _userLocks = new();

        public RefreshJwtMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"🟢 RefreshJwtMiddleware - Path: {context.Request.Path}");

            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    // ✅ Đọc cả 2 tokens để check expiry
                    var accessJwt = tokenHandler.ReadJwtToken(accessToken);
                    var refreshJwt = tokenHandler.ReadJwtToken(refreshToken);

                    var accessExpUtc = accessJwt.ValidTo;
                    var refreshExpUtc = refreshJwt.ValidTo;
                    var nowUtc = DateTime.UtcNow;

                    var userIdClaim = accessJwt.Claims.FirstOrDefault(c => c.Type == "idUser");
                    string userId = userIdClaim?.Value ?? "unknown";

                    // ✅ Log chi tiết cả 2 tokens
                    Console.WriteLine($"👤 User {userId}");
                    Console.WriteLine($"📅 Current Time (UTC): {nowUtc:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"🔑 AccessToken expires: {accessExpUtc:yyyy-MM-dd HH:mm:ss} (còn {(accessExpUtc - nowUtc).TotalSeconds:F0}s)");
                    Console.WriteLine($"🔄 RefreshToken expires: {refreshExpUtc:yyyy-MM-dd HH:mm:ss} (còn {(refreshExpUtc - nowUtc).TotalSeconds:F0}s)");

                    // ✅ KIỂM TRA REFRESH TOKEN HẾT HẠN TRƯỚC
                    if (refreshExpUtc <= nowUtc)
                    {
                        Console.WriteLine($"❌ User {userId} - RefreshToken ĐÃ HẾT HẠN!");
                        Console.WriteLine($"🚪 Xóa cookies và redirect to Login");

                        context.Response.Cookies.Delete("AccessToken", new CookieOptions { Path = "/" });
                        context.Response.Cookies.Delete("RefreshToken", new CookieOptions { Path = "/" });

                        if (!context.Response.HasStarted)
                        {
                            context.Response.Redirect("/Identity/Login/Index");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Response đã started, không thể redirect");
                        }
                    }
                    // ✅ Kiểm tra AccessToken hết hạn (nhưng RefreshToken còn hạn)
                    else if (accessExpUtc <= nowUtc.AddSeconds(30))
                    {
                        Console.WriteLine($"🔄 User {userId} - AccessToken hết hạn, cần refresh");

                        var userLock = _userLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
                        bool lockAcquired = await userLock.WaitAsync(TimeSpan.FromSeconds(2));

                        if (!lockAcquired)
                        {
                            Console.WriteLine($"⚠️ User {userId} - Timeout chờ lock, skip refresh");
                            await _next(context);
                            return;
                        }

                        try
                        {
                            var currentAccessToken = context.Request.Cookies["AccessToken"];

                            if (!string.IsNullOrEmpty(currentAccessToken) && currentAccessToken != accessToken)
                            {
                                var currentJwt = tokenHandler.ReadJwtToken(currentAccessToken);
                                if (currentJwt.ValidTo > nowUtc)
                                {
                                    Console.WriteLine($"✅ User {userId} - Token đã được refresh bởi request khác");
                                    await _next(context);
                                    return;
                                }
                            }

                            using var scope = _scopeFactory.CreateScope();
                            var jwtHelper = scope.ServiceProvider.GetRequiredService<JwtHelper>();

                            var newTokens = jwtHelper.refreshTokens(refreshToken);

                            if (newTokens != null)
                            {
                                var newAccessJwt = tokenHandler.ReadJwtToken(newTokens.accessToken);

                                Console.WriteLine($"✅ User {userId} - Token mới expires: {newAccessJwt.ValidTo:yyyy-MM-dd HH:mm:ss}");

                                context.Response.Cookies.Delete("AccessToken", new CookieOptions { Path = "/" });
                                context.Response.Cookies.Append("AccessToken", newTokens.accessToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Lax,  // ✅ Đổi về Lax
                                    Expires = newAccessJwt.ValidTo,
                                    Path = "/"
                                });

                                Console.WriteLine($"✅ User {userId} - Refresh thành công");
                            }
                            else
                            {
                                Console.WriteLine($"❌ User {userId} - refreshTokens() trả về null (token hết hạn hoặc invalid)");

                                context.Response.Cookies.Delete("AccessToken", new CookieOptions { Path = "/" });
                                context.Response.Cookies.Delete("RefreshToken", new CookieOptions { Path = "/" });

                                if (!context.Response.HasStarted)
                                {
                                    Console.WriteLine($"🚪 User {userId} - Redirect to Login");
                                    context.Response.Redirect("/Identity/Login/Index");
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine($"⚠️ Response đã started, không thể redirect");
                                }
                            }
                        }
                        finally
                        {
                            userLock.Release();
                            Console.WriteLine($"🔓 User {userId} - Lock released");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✔️ User {userId} - Cả 2 tokens đều còn hạn");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❗ Lỗi middleware: {ex.GetType().Name} - {ex.Message}");
                    Console.WriteLine($"❗ StackTrace: {ex.StackTrace}");
                }
            }
            else if (!string.IsNullOrEmpty(accessToken) || !string.IsNullOrEmpty(refreshToken))
            {
                // ✅ Trường hợp thiếu 1 trong 2 tokens
                Console.WriteLine($"⚠️ Thiếu token - AccessToken: {!string.IsNullOrEmpty(accessToken)}, RefreshToken: {!string.IsNullOrEmpty(refreshToken)}");

                context.Response.Cookies.Delete("AccessToken", new CookieOptions { Path = "/" });
                context.Response.Cookies.Delete("RefreshToken", new CookieOptions { Path = "/" });

                if (!context.Response.HasStarted)
                {
                    Console.WriteLine($"🚪 Redirect to Login do thiếu token");
                    context.Response.Redirect("/Identity/Login/Index");
                    return;
                }
            }
            else
            {
                Console.WriteLine($"ℹ️ Không có tokens - có thể là trang public hoặc chưa login");
            }

            await _next(context);
        }
    }
}