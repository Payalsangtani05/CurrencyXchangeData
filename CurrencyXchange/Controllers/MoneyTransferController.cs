using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CurrencyXchange.Models;
using CurrencyXchange.Service;
using System.Threading.Tasks;
using System.Linq;
using CurrencyXchange.Data;

namespace CurrencyXchange.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MoneyTransferController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ICurrencyConverterService _currencyConverterService;
		private readonly ApplicationDbContext _context;

		public MoneyTransferController(UserManager<ApplicationUser> userManager,
										ICurrencyConverterService currencyConverterService,
										ApplicationDbContext context)
		{
			_userManager = userManager;
			_currencyConverterService = currencyConverterService;
			_context = context;
		}

		[HttpPost("transfer")]
		public async Task<IActionResult> TransferMoney([FromBody] TransferRequest request)
		{
			if (request.Amount <= 0)
			{
				return BadRequest("Amount must be greater than zero.");
			}

			var sender = await _userManager.FindByIdAsync(request.SenderId);
			var receiver = await _userManager.FindByIdAsync(request.ReceiverId);

			if (sender == null || receiver == null)
			{
				return NotFound("Sender or receiver not found.");
			}

			if (sender.Balance < request.Amount)
			{
				return BadRequest("Insufficient balance.");
			}

			var convertedAmount = await _currencyConverterService.ConvertCurrency(request.FromCurrency, request.ToCurrency, request.Amount);

			// Update sender and receiver balances
			sender.Balance -= request.Amount;
			receiver.Balance += convertedAmount;

			await _userManager.UpdateAsync(sender);
			await _userManager.UpdateAsync(receiver);

			// Record the transaction
			var transaction = new Transaction
			{
				SenderId = request.SenderId,
				ReceiverId = request.ReceiverId,
				Amount = request.Amount,
				FromCurrency = request.FromCurrency,
				ToCurrency = request.ToCurrency,
				ConvertedAmount = convertedAmount
			};

			_context.Transactions.Add(transaction);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Transfer successful", convertedAmount });
		}
	}

	public class TransferRequest
	{
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public decimal Amount { get; set; }
		public string FromCurrency { get; set; }
		public string ToCurrency { get; set; }
	}
}