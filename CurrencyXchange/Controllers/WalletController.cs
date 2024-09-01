using CurrencyXchange.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyXchange.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WalletController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public WalletController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		// GET: api/wallet/{userId}
		[HttpGet("{userId}")]
		[Authorize]
		public async Task<IActionResult> GetBalance(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			return Ok(new { Balance = user.Balance });
		}

		// POST: api/wallet/{userId}/add
		[HttpPost("{userId}/add")]
		[Authorize]
		public async Task<IActionResult> AddBalance(string userId, [FromBody] decimal amount)
		{
			if (amount <= 0)
			{
				return BadRequest("Amount must be greater than zero.");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			user.Balance += amount;
			await _userManager.UpdateAsync(user);

			return Ok(new { Balance = user.Balance });
		}

		// POST: api/wallet/{userId}/subtract
		[HttpPost("{userId}/subtract")]
		[Authorize]
		public async Task<IActionResult> SubtractBalance(string userId, [FromBody] decimal amount)
		{
			if (amount <= 0)
			{
				return BadRequest("Amount must be greater than zero.");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			if (user.Balance < amount)
			{
				return BadRequest("Insufficient balance.");
			}

			user.Balance -= amount;
			await _userManager.UpdateAsync(user);

			return Ok(new { Balance = user.Balance });
		}
	}
}