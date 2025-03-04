using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        public class ResetPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Custom logic to reset password
            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
