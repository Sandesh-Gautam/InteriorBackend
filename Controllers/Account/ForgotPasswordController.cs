using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InteriorBackend.Services;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService; // Custom email service

        public ForgotPasswordController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        public class ForgotPasswordRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.FindUserByEmailAsync(request.Email);
            if (user == null)
            {
                // For security, don't reveal if the email exists or not
                return Ok(new { message = "If your email is registered, you will receive a password reset link." });
            }

            // Generate password reset token
            var resetToken = await _userService.GeneratePasswordResetTokenAsync(user.Id);

            // Construct reset password URL
            var resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?token={resetToken}&email={request.Email}";

            // Send email with reset link
            await _emailService.SendEmailAsync(request.Email, "Password Reset Request",
                $"Click <a href='{resetUrl}'>here</a> to reset your password.");

            return Ok(new { message = "Password reset link sent successfully." });
        }
    }
}
