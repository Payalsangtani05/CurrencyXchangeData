using CurrencyXchange.Data;
using CurrencyXchange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
 
namespace CurrencyXchange.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public AuthController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost("signup")]
		public async Task<IActionResult> SignUp([FromBody] User user)
		{
			if (await _context.Users.AnyAsync(u => u.Username == user.Username || u.Email == user.Email))
				return BadRequest("User already exists");

			using (var hmac = new HMACSHA512())
			{
				user.PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash)));
			}

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok("User created successfully");
		}


		[HttpPost("signin")]
		public async Task<IActionResult> SignIn([FromBody] User user)
		{
			var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);

			if (existingUser == null)
				return BadRequest("Invalid username");

			using (var hmac = new HMACSHA512())
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
				if (Convert.ToBase64String(computedHash) != existingUser.PasswordHash)
					return BadRequest("Invalid password");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes("YourJwtKey");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
			new Claim(ClaimTypes.Name, existingUser.Username)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return Ok(new { Token = tokenHandler.WriteToken(token) });
		}
	}
}