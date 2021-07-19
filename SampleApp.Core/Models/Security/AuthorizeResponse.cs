using Newtonsoft.Json;
using SampleApp.Core.Security;

namespace SampleApp.Core.Models.Security
{
	public class AuthorizeResponse
	{
		[JsonProperty(PropertyName = "access_token")]
		public AccessToken AccessToken { get; set; }
		[JsonProperty(PropertyName = "token_type")]
		public string TokenType { get; set; }

		public AuthorizeResponse() { }

		public AuthorizeResponse(AccessToken accessToken, string tokenType)
		{
			AccessToken = accessToken;
			TokenType = tokenType;
		}
	}
}
