using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.API.Contracts;
using VIOBANK.Application.Services;
using VIOBANK.Infrastructure;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        //    private readonly UserService _userService;
        //    private readonly CardService _cardService;
        //    private readonly IHttpContextAccessor _httpContextAccessor;
        //    private readonly ILogger<SettingsController> _logger;
        //    private readonly IJwtProvider _jwtProvider;
        //    private readonly IPasswordHasher _passwordHash;

        //    public SettingsController(
        //        UserService userService,
        //        CardService cardService,
        //        IHttpContextAccessor httpContextAccessor,
        //        ILogger<SettingsController> logger,
        //        IJwtProvider jwtProvider,
        //        IPasswordHasher passwordHash)
        //    {
        //        _userService = userService;
        //        _cardService = cardService;
        //        _httpContextAccessor = httpContextAccessor;
        //        _logger = logger;
        //        _jwtProvider = jwtProvider;
        //        _passwordHash = passwordHash;
        //    }

        //    [HttpPut("change-card-password")]
        //    public async Task<IActionResult> ChangeCardPassword([FromBody] ChangeCardPasswordDTO request)
        //    {
        //        try
        //        {
        //            var userId = GetUserId();
        //            if (userId == null)
        //            {
        //                return Unauthorized(new { status = "error", message = "Token is missing or invalid" });
        //            }

        //            var card = await _cardService.GetCardByNumber(request.CardNumber);
        //            if (card == null || card.Account.UserId != userId)
        //            {
        //                return NotFound(new { status = "error", message = "Card not found or does not belong to the user" });
        //            }

        //            card.CardPassword = request.NewPassword;
        //            await _cardService.UpdateCard(card);

        //            return Ok(new { status = "success", message = "Card password updated successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Error changing card password: {ex.Message}");
        //            return StatusCode(500, new { status = "error", message = "Internal server error" });
        //        }
        //    }

        //    [HttpPut("change-account-password")]
        //    public async Task<IActionResult> ChangeAccountPassword([FromBody] ChangeAccountPasswordDTO request)
        //    {
        //        try
        //        {
        //            var userId = GetUserId();
        //            if (userId == null)
        //            {
        //                return Unauthorized(new { status = "error", message = "Token is missing or invalid" });
        //            }

        //            var user = await _userService.GetUserById(userId.Value);
        //            if (user == null)
        //            {
        //                return NotFound(new { status = "error", message = "User not found" });
        //            }

        //            user.PasswordHash = _passwordHash.Generate(user.PasswordHash);
        //            await _userService.UpdateUser(user);

        //            return Ok(new { status = "success", message = "Account password updated successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Error changing account password: {ex.Message}");
        //            return StatusCode(500, new { status = "error", message = "Internal server error" });
        //        }
        //    }

        //    [HttpDelete("delete-account")]
        //    public async Task<IActionResult> DeleteAccount()
        //    {
        //        try
        //        {
        //            var userId = GetUserId();
        //            if (userId == null)
        //            {
        //                return Unauthorized(new { status = "error", message = "Token is missing or invalid" });
        //            }

        //            var user = await _userService.GetUserById(userId.Value);
        //            if (user == null)
        //            {
        //                return NotFound(new { status = "error", message = "User not found" });
        //            }

        //            await _userService.DeleteUser(userId.Value);

        //            return Ok(new { status = "success", message = "Account deleted successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Error deleting account: {ex.Message}");
        //            return StatusCode(500, new { status = "error", message = "Internal server error" });
        //        }
        //    }

        //    [HttpPost("logout")]
        //    public IActionResult Logout()
        //    {
        //        try
        //        {
        //            Response.Cookies.Delete("tasty-cookies");

        //            return Ok(new { status = "success", message = "Logged out successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Error logging out: {ex.Message}");
        //            return StatusCode(500, new { status = "error", message = "Internal server error" });
        //        }
        //    }

        //    private int? GetUserId()
        //    {
        //        var token = _httpContextAccessor.HttpContext?.Request.Cookies["tasty-cookies"];
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            return null;
        //        }

        //        var claims = _jwtProvider.GetClaimsFromToken(token);
        //        return int.Parse(User.FindFirst("userId")?.Value);
        //    }
       }

}
