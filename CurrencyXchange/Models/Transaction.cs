namespace CurrencyXchange.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		public string SenderId { get; set; }
		public string ReceiverId { get; set; }
		public decimal Amount { get; set; }
		public string FromCurrency { get; set; }
		public string ToCurrency { get; set; }
		public decimal ConvertedAmount { get; set; }
		public DateTime Date { get; set; } = DateTime.UtcNow;
		public DateTime TransferDate { get; set; }
		public decimal PriceAtTransfer { get; set; }
		public decimal CurrentPrice { get; set; }
	}
}
