using System;
using Newtonsoft.Json;

namespace SampleApp.Core.Security
{
	public abstract class JsonWebToken
	{
		public string Token { get; protected set; }
		[JsonProperty(PropertyName = "expiress_in")]
		public long Expiration { get; protected set; }

		public JsonWebToken(string token, long expiration)
		{
			if (string.IsNullOrWhiteSpace(token))
				throw new ArgumentException("Invalid token.");

			if (expiration <= 0)
				throw new ArgumentException("Invalid expiration.");

			Token = token;
			Expiration = expiration;
		}

		public bool IsExpired() => DateTime.UtcNow.Ticks > Expiration;
	}
}
