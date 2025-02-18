using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VIOBANK.API.Contracts.Contact;
using VIOBANK.Application.Services;
using VIOBANK.Domain.Models;
using VIOBANK.Infrastructure;

namespace VIOBANK.Controllers
{
    [Authorize]
    [ApiController]
    [Route("operations/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly ContactService _contactService;
        private readonly UserService _userService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IValidator<int> _userIdValidator;
        private readonly IValidator<ContactRequestDTO> _contactRequestValidator;

        public ContactsController(IValidator<ContactRequestDTO> contactRequestValidator, IValidator<int> userIdValidator, ContactService contactService, IJwtProvider jwtProvider, UserService userService)
        {
            _contactService = contactService;
            _jwtProvider = jwtProvider;
            _userService = userService;
            _userIdValidator = userIdValidator;
            _contactRequestValidator = contactRequestValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var token = HttpContext.Request.Cookies["tasty-cookies"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var contacts = await _contactService.GetContactsByUserId(userId);

            var contactDTOs = contacts.Select(c => new ContactResponseDTO
            {
                Name = c.ContactName,
                PhoneNumber = c.ContactPhone,
                CardNumber = c.ContactCard
            });

            return Ok(contactDTOs);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddContact([FromBody] ContactRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.CardNumber))
            {
                return BadRequest(new { status = "error", message = "Invalid request data" });
            }

            var token = HttpContext.Request.Cookies["tasty-cookies"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { status = "error", message = "Token is missing in cookies" });
            }

            var claims = _jwtProvider.GetClaimsFromToken(token);
            var userId = int.Parse(User.FindFirst("userId")?.Value);

            var validationResultId = await _userIdValidator.ValidateAsync(userId);
            if (!validationResultId.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultId.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var validationResultContactRequest = await _contactRequestValidator.ValidateAsync(request);
            if (!validationResultContactRequest.IsValid)
            {
                return BadRequest(new { status = "error", message = validationResultContactRequest.Errors.Select(e => e.ErrorMessage).ToArray() });
            }

            var userFromContact = await _userService.GetUserByCardNumber(request.CardNumber);

            if (userFromContact == null)
            {
                return BadRequest(new { status = "error", message = "Card not found or has no owner" });
            }
            if (userFromContact.UserId == userId)
            {
                return BadRequest(new { status = "error", message = "You cannot add yourself as a contact." });
            }

            var contact = new Contact
            {
                UserId = userId,
                ContactName = userFromContact.Surname + " " + userFromContact.Name,
                ContactPhone = userFromContact.Phone,
                ContactCard = request.CardNumber
            };

            await _contactService.AddContact(contact);

            return Ok(new { status = "success", message = "Contact added successfully" });
        }
    }
}
