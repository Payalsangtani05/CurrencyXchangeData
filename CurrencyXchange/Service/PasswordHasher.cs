﻿using System.Security.Cryptography;

namespace CurrencyXchange.Service
{
	public class PasswordHasher
	{
		private const int SaltSize = 16; // 128 bit 
		private const int KeySize = 32;  // 256 bit
		private const int Iterations = 10000; // Number of iterations

		public string HashPassword(string password)
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				byte[] salt = new byte[SaltSize];
				rng.GetBytes(salt);

				var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
				byte[] hash = pbkdf2.GetBytes(KeySize);

				byte[] hashBytes = new byte[SaltSize + KeySize];
				Array.Copy(salt, 0, hashBytes, 0, SaltSize);
				Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

				return Convert.ToBase64String(hashBytes);
			}
		}

		public bool VerifyPassword(string password, string hashedPassword)
		{
			byte[] hashBytes = Convert.FromBase64String(hashedPassword);
			byte[] salt = new byte[SaltSize];
			Array.Copy(hashBytes, 0, salt, 0, SaltSize);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
			byte[] hash = pbkdf2.GetBytes(KeySize);

			for (int i = 0; i < KeySize; i++)
			{
				if (hashBytes[i + SaltSize] != hash[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}