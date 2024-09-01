using Microsoft.AspNetCore.Identity;

namespace CurrencyXchange.Models
{
	public class ApplicationUser : IdentityUser
	{
		public decimal Balance { get; set; } = 0.0M;  // Initial balance set to 0
	}
}
