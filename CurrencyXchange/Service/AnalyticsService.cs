using CurrencyXchange.Data;
using Microsoft.EntityFrameworkCore;

namespace CurrencyXchange.Service
{
	public class AnalyticsService
	{
		private readonly ApplicationDbContext _context;

		public AnalyticsService(ApplicationDbContext context)
		{
			_context = context;
		}
	 
		public async Task<Dictionary<string, Dictionary<string, decimal>>> GetProfitLossAnalyticsAsync(DateTime startDate)
		{
			var transactions = await _context.Transactions
				.Where(t => t.TransferDate >= startDate)
				.ToListAsync();

			var profitLoss = new Dictionary<string, Dictionary<string, decimal>>();

			var userTransactions = transactions.GroupBy(t => t.SenderId)
				.Concat(transactions.GroupBy(t => t.ReceiverId));

			foreach (var group in userTransactions)
			{
				var userId = group.Key;
				var totalProfit = 0m;
				var totalLoss = 0m;

				foreach (var transaction in group)
				{
					var profitOrLoss = (transaction.CurrentPrice - transaction.PriceAtTransfer) * transaction.Amount;

					if (profitOrLoss > 0)
					{
						totalProfit += profitOrLoss;
					}
					else
					{
						totalLoss -= profitOrLoss;
					}
				}

				var result = new Dictionary<string, decimal>
				{
					{ "Profit", totalProfit },
					{ "Loss", totalLoss }
				};

				profitLoss[userId] = result;
			}

			return profitLoss;
		}
	}
}