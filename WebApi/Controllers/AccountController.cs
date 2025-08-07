using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;

		public AccountController(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Welcome()
		{
			if (User.Identity is null || !User.Identity.IsAuthenticated)
				return Unauthorized("You must be logged in to access this resource.");

			return Ok("Welcome to the Account API!");
		}

		[Authorize]
		[HttpGet("Profile")]
		public async Task<IActionResult> Profile()
		{
			if (User.Identity is null || !User.Identity.IsAuthenticated)
				return Unauthorized("You must be logged in to access this resource.");

			var currentUser = await _userManager.GetUserAsync(User);

			if (currentUser == null)
				return NotFound("User not found.");

			var userProfile = new
			{
				currentUser.Id,
				Name = currentUser.UserName ?? string.Empty,
				Email = currentUser.Email ?? string.Empty,
				PhoneNumber = currentUser.PhoneNumber ?? string.Empty
			};
			return Ok(userProfile);
		}
	}
}
