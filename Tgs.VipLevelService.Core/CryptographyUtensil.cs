using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Tgs.VipLevelService;

namespace Tgs.VipLevelService;
internal class CryptographyUtensil : ICryptographyUtensil
{
	public string Sha512(string source)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(source);
		var hash = SHA512.HashData(bytes);
		return GetStringFromHash(hash);
	}

	private static string GetStringFromHash(byte[] hash)
	{
		StringBuilder result = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			result.Append(hash[i].ToString("X2"));
		}

		return result.ToString();
	}
}
