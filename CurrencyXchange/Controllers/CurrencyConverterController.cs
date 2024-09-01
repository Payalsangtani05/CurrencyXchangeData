using CurrencyXchange.Service;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyXchange.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CurrencyConverterController : ControllerBase
	{
		private readonly ICurrencyConverterService _currencyConverterService;

		public CurrencyConverterController(ICurrencyConverterService currencyConverterService)
		{
			_currencyConverterService = currencyConverterService;
		}

		[HttpGet]
		public async Task<IActionResult> ConvertCurrency([FromQuery] string fromCurrency, [FromQuery] string toCurrency, [FromQuery] decimal amount)
		{
			if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency) || amount <= 0)
			{
				return BadRequest("Invalid input parameters.");
			}

			try
			{
				var convertedAmount = await _currencyConverterService.ConvertCurrency(fromCurrency, toCurrency, amount);
				return Ok(new { fromCurrency, toCurrency, originalAmount = amount, convertedAmount });
			}
			catch (HttpRequestException)
			{
				return StatusCode(500, "Error occurred while fetching conversion data.");
			}
		}
	}
}