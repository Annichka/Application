using Newtonsoft.Json;
using System;

namespace SampleApp.Core.Security
{
	public class AccessToken : JsonWebToken
	{

		[JsonProperty(PropertyName = "refresh_token")]
		public RefreshToken RefreshToken { get; private set; }

		public AccessToken(string token, long expiration, RefreshToken refreshToken) : base(token, expiration)
		{
			if (refreshToken == null)
				throw new ArgumentException("Specify a valid refresh token.");

			RefreshToken = refreshToken;
		}
	}
}
