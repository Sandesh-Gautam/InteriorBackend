using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using InteriorBackend.Services;
using InteriorBackend.Models;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class ResendEmailConfirmationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public ResendEmailConfirmationController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        public class ResendEmailRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost("resend-email-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.FindUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Ok(new { message = "If your email is registered, you will receive a verification link." });
            }

            if (user.IsEmailConfirmed)
            {
                return BadRequest(new { message = "Email is already confirmed." });
            }

            // Generate a new email confirmation token
            var confirmationCode = await _userService.GenerateEmailConfirmationCodeAsync(user.Id);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/account/confirm-email?userId={user.Id}&code={encodedCode}";

            // Send confirmation email
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");

            return Ok(new { message = "Verification email sent. Please check your email." });
        }
    }
}
