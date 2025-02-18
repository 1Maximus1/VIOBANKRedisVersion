using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using Twilio.Jwt.AccessToken;
using VIOBANK.API.Contracts.Auth;
using VIOBANK.API.Contracts.Card;
using VIOBANK.API.Validation;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.RedisPersistence.Services;

namespace VIOBANK.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly JwtBlacklistService _jwtBlacklistService;
        private readonly IValidator<RegisterUserDTO> _registerValidator;
        private readonly IValidator<LoginUserDTO> _loginValidator;

        public AuthController(IValidator<LoginUserDTO> loginValidator, IValidator<RegisterUserDTO> registerValidator, UserService userService, JwtBlacklistService jwtBlacklistService)
        {
            _userService = userService;
            _jwtBlacklistService = jwtBlacklistService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDTO)
        {
            if (HttpContext.Request.Cookies.ContainsKey("tasty-cookies"))
            {
                var tokenOld = HttpContext.Request.Cookies["tasty-cookies"];

                await _jwtBlacklistService.AddToBlacklistAsync(tokenOld);
                Response.Cookies.Delete("tasty-cookies");
            }

            var validationResultReg = await _registerValidator.ValidateAsync(registerUserDTO);
            if (!validationResultReg.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultReg.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var user = new User
            {
                Name = registerUserDTO.Name,
                Surname = registerUserDTO.Surname,
                Phone = registerUserDTO.Phone,
                Email = registerUserDTO.Email,
                Employment = new Employment
                {
                    Type = registerUserDTO.Employment.Type,
                    Income = registerUserDTO.Employment.Income,
                },
                IdCard = registerUserDTO.IdCard,
                TaxNumber = registerUserDTO.TaxNumber,
                Registration = registerUserDTO.Registration,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.Register(user, registerUserDTO.Password, registerUserDTO.CardPassword);
            return Ok(new { status = "success", message = "User registered successfully"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            if (HttpContext.Request.Cookies.ContainsKey("tasty-cookies"))
            {
                var tokenOld = HttpContext.Request.Cookies["tasty-cookies"];

                await _jwtBlacklistService.AddToBlacklistAsync(tokenOld);
                Response.Cookies.Delete("tasty-cookies");
            }

            var validationResultlogib = await _loginValidator.ValidateAsync(loginUserDTO);
            if (!validationResultlogib.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultlogib.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            try
            {
                var token = await _userService.Login(loginUserDTO.Email, loginUserDTO.Password);
                Response.Cookies.Append("tasty-cookies", token);
                return Ok(new { status = "success", message = "User login successfully" });
            }
            catch (InvalidCredentialException ex)
            {
                return Unauthorized(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "An unexpected error occurred." });
            }
        }
    }
}
