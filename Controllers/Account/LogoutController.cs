using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize] // Requires authentication to access
    public class LogoutController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, bool> _tokenBlacklist = new();

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Extract token from Authorization header
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Add token to blacklist (if using a token revocation mechanism)
            _tokenBlacklist[token] = true;

            return Ok(new { message = "Logout successful. Token invalidated." });
        }

        public static bool IsTokenRevoked(string token)
        {
            return _tokenBlacklist.ContainsKey(token);
        }
    }
}
