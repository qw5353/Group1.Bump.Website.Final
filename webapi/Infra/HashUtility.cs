using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BC = BCrypt.Net.BCrypt;

namespace webapi.Infra
{
	public class HashUtility
	{
		private readonly IConfiguration _config;

		public HashUtility(IConfiguration config)
		{
			_config = config;
		}

		public string ToSHA256(string password, string salt)
		{
			using (var mySHA256 = SHA256.Create())
			{
				var passwordBytes = Encoding.UTF8.GetBytes(password + GetSalt());
				var hash = mySHA256.ComputeHash(passwordBytes);
				var sb = new StringBuilder();
				foreach (var key in hash)
				{
					sb.Append(key.ToString("X2"));
				}
				return sb.ToString();
			}
		}

		public string GetSalt()
		{
			return _config["salt"];
		}

		public string ToBCrypt(string password)
		{
			string passwordHash = BC.HashPassword(password, GetSalt());
			return passwordHash;
		}

		public List<string> ToBCrypt(List<string> passwords)
		{
			List<string> hashPwds = new List<string>();
			foreach(string password in passwords)
			{
			string hashPwd = BC.HashPassword(password, GetSalt());
			hashPwds.Add(hashPwd);
			}
			return hashPwds;
		}
	}
}
