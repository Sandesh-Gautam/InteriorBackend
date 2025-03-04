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
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public RegisterController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        public class RegisterRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userService.FindUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            var user = new User
            {
                Email = request.Email,
                Username = request.Email.Split('@')[0], // Use part of email as username
            };

            var success = await _userService.RegisterUserAsync(user, request.Password);
            if (!success)
            {
                return StatusCode(500, new { message = "User registration failed." });
            }

            // Generate email confirmation token
            var confirmationCode = await _userService.GenerateEmailConfirmationCodeAsync(user.Id);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/account/confirm-email?userId={user.Id}&code={encodedCode}";

            // Send confirmation email
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");

            return Ok(new { message = "User registered successfully. Please check your email to confirm your account." });
        }
    }
}
