using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using InteriorBackend.Services;

namespace InteriorBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class ConfirmEmailController : ControllerBase
    {
        private readonly IUserService _userService;

        public ConfirmEmailController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
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

            // Mark email as confirmed
            bool emailConfirmed = await _userService.ConfirmUserEmailAsync(userId);
            if (!emailConfirmed)
            {
                return BadRequest(new { message = "Error confirming email." });
            }

            return Ok(new { message = "Email confirmed successfully." });
        }
    }
}
