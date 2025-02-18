using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.Application.Services;
using VIOBANK.Infrastructure;
using VIOBANK.RedisPersistence.Services;
using VIOBANK.API.Contracts.Account;
using FluentValidation;
using VIOBANK.API.Contracts.Card;
namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("settings")]
    public class AccountsController : Controller
    {
        private readonly UserService _userService;
        private readonly CardService _cardService;
        private readonly ILogger<AccountsController> _logger;
        private readonly AccountService _accountService;
        private readonly IJwtProvider _jwtProvider;
        private readonly JwtBlacklistService _jwtBlacklistService;
        private readonly IValidator<int> _userIdValidator;
        private readonly IValidator<ChangeCardPasswordDTO> _changeCardPasswordValidator;
        private readonly IValidator<ChangeAccountPassword> _changeAccountPasswordValidator;

        public AccountsController(
            UserService userService,
            CardService cardService,
            ILogger<AccountsController> logger,
            AccountService accountService,
            IJwtProvider jwtProvider,
            JwtBlacklistService jwtBlacklistService,
            IValidator<int> userIdValidator,
            IValidator<ChangeCardPasswordDTO> changeCardPasswordValidator,
            IValidator<ChangeAccountPassword> changeAccountPasswordValidator) 
        {
            _userService = userService;
            _cardService = cardService;
            _logger = logger;
            _accountService = accountService;
            _jwtProvider = jwtProvider;
            _jwtBlacklistService = jwtBlacklistService;
            _userIdValidator = userIdValidator;
            _changeCardPasswordValidator = changeCardPasswordValidator;
            _changeAccountPasswordValidator = changeAccountPasswordValidator;
        }

        [HttpPut("change-card-password")]
        public async Task<IActionResult> ChangeCardPassword([FromBody] ChangeCardPasswordDTO request)
        {
            if (request == null)
                return BadRequest(new { status = "error", message = "Invalid request data." });
           
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);

            var validationResult = await _userIdValidator.ValidateAsync(userId);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultCardPassword = await _changeCardPasswordValidator.ValidateAsync(request);
            if (!validationResultCardPassword.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultCardPassword.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var (success, errorMessage) = await _userService.ChangeAllCardsPassword(userId, request.NewPassword, request.OldPassword);
            if (!success)
                return BadRequest(new { status = "error", message = errorMessage });

            return Ok(new { status = "success", message = "Card password updated successfully" });
        }

        [HttpPut("change-account-password")]
        public async Task<IActionResult> ChangeAccountPassword([FromBody] ChangeAccountPassword request)
        {
            if (request == null)
                return BadRequest(new { status = "error", message = "Invalid request data." });

            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);

            var validationResult = await _userIdValidator.ValidateAsync(userId);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultChangeAccountPassword = await _changeAccountPasswordValidator.ValidateAsync(request);
            if (!validationResultChangeAccountPassword.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultChangeAccountPassword.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var (success, errorMessage) = await _userService.ChangeAccountPassword(userId, request.NewPassword, request.OldPassword);
            if (!success)
                return BadRequest(new { status = "error", message = errorMessage });

            return Ok(new { status = "success", message = "Account password updated successfully" });
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);
            if (await _userService.GetUserById(userId) == null)
            {
                return NotFound(new { status = "error", message = "User not found" });
            }

            var validationResult = await _userIdValidator.ValidateAsync(userId);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            await _userService.DeleteUser(userId);

            await _jwtBlacklistService.AddToBlacklistAsync(token);
            Response.Cookies.Delete("tasty-cookies");

            return Ok(new { status = "success", message = "Account deleted successfully" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = Int32.Parse(User.FindFirst("userId")?.Value);
            
            var validationResult = await _userIdValidator.ValidateAsync(userId);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResult.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            await _jwtBlacklistService.AddToBlacklistAsync(token);
            Response.Cookies.Delete("tasty-cookies");

            return Ok(new { status = "success", message = "Logged out successfully" });
        }
    }
}
