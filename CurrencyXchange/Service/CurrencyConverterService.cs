using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq; 
namespace CurrencyXchange.Service
{
	public class CurrencyConverterService : ICurrencyConverterService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly string _baseUrl;

		public CurrencyConverterService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_apiKey = configuration["CurrencyConverter:ApiKey"];
			_baseUrl = configuration["CurrencyConverter:BaseUrl"];
		}

		public async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
		{
			var url = $"{_baseUrl}{_apiKey}/pair/{fromCurrency}/{toCurrency}/{amount}";

			var response = await _httpClient.GetAsync(url);

			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException("Error occurred while fetching currency conversion data.");
			}

			var content = await response.Content.ReadAsStringAsync();
			var json = JObject.Parse(content);
			var convertedAmount = json["conversion_result"].Value<decimal>();

			return convertedAmount;
		}
	}
}