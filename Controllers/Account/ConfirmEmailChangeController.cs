using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using InteriorBackend.Models;
using InteriorBackend.Services;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class ConfirmEmailChangeController : ControllerBase
    {
        private readonly IUserService _userService; // Custom service for user management

        public ConfirmEmailChangeController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromQuery] string userId, [FromQuery] string email, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return BadRequest(new { message = "Invalid request parameters." });
            }

            var user = await _userService.FindUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID '{userId}' not found." });
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Validate the code (assumes you store generated codes in the DB)
            bool isCodeValid = await _userService.ValidateEmailConfirmationCodeAsync(userId, code);
            if (!isCodeValid)
            {
                return BadRequest(new { message = "Invalid or expired email confirmation code." });
            }

            // Update email and username
            bool emailChanged = await _userService.ChangeUserEmailAsync(userId, email);
            if (!emailChanged)
            {
                return BadRequest(new { message = "Error changing email." });
            }

            return Ok(new { message = "Email change confirmed successfully." });
        }
    }
}
