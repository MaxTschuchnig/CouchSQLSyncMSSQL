using System;
using System.Security.Cryptography;
using System.Text;

namespace AddData.SyncMiddleware
{
	public class Hashing : IDisposable
	{
		public void Dispose() { }

		public string GenerateSHA256String(string inputString)
		{
			SHA256 sha512 = SHA256Managed.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(inputString);
			byte[] hash = sha512.ComputeHash(bytes);
			return GetStringFromHash(hash);
		}

		private string GetStringFromHash(byte[] hash)
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				result.Append(hash[i].ToString("X2"));
			}
			return result.ToString();
		}
	}
}
