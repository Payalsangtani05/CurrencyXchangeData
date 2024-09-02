using Newtonsoft.Json; // For Newtonsoft.Json

namespace CurrencyXchange.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		[JsonIgnore]
		public string Passwordsalt { get; set; }

	 

		//public string PasswordHashBase64
		//{
		//	get => Convert.ToBase64String(PasswordHash);
		//	set => PasswordHash = Convert.FromBase64String(value);
		//}
	}
}
