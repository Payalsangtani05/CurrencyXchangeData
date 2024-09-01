using CurrencyXchange.Service;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyXchange.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AnalyticsController : ControllerBase
	{
		private readonly AnalyticsService _analyticsService;

		public AnalyticsController(AnalyticsService analyticsService)
		{
			_analyticsService = analyticsService;
		}

		[HttpGet("profit-loss")]
		public async Task<IActionResult> GetProfitLoss([FromQuery] DateTime startDate)
		{
			if (startDate == DateTime.MinValue)
			{
				return BadRequest("Invalid start date.");
			}

			var result = await _analyticsService.GetProfitLossAnalyticsAsync(startDate);

			var response = result.Select(entry => new
			{
				User = entry.Key,
				Type = entry.Value["Profit"] > 0 ? "Profit" : "Loss",
				Amount = entry.Value["Profit"] > 0 ? entry.Value["Profit"] : entry.Value["Loss"]
			});

			return Ok(response);
		}
	}
}