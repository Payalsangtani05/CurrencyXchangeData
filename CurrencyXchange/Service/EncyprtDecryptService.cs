using System.Security.Cryptography;
using System.Text;

namespace CurrencyXchange.Service
{
	public class EncyprtDecryptService
	{
		private readonly string key;
		private readonly string iv;

		public EncyprtDecryptService(string key, string iv)
		{
			this.key = key;
			this.iv = iv;
		}

		public string Encrypt(string plainText)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = Encoding.UTF8.GetBytes(iv);

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}
						return Convert.ToBase64String(msEncrypt.ToArray());
					}
				}
			}
		}

		public string Decrypt(string cipherText)
		{
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Encoding.UTF8.GetBytes(key);
				aesAlg.IV = Encoding.UTF8.GetBytes(iv);

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							return srDecrypt.ReadToEnd();
						}
					}
				}
			}
		}
	}
}