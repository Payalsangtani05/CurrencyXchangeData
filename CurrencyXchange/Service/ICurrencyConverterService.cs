using Newtonsoft.Json.Linq;

namespace CurrencyXchange.Service
{
	public interface ICurrencyConverterService
	{
		Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount);
	}

	 
}
