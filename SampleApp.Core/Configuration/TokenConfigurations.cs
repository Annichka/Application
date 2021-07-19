namespace SampleApp.Core.Configuration
{
	public class TokenConfigurations
	{
		public string Audience { get; set; }
		public string Issuer { get; set; }
		public long AccessTokenExpiration { get; set; }
		public long RefreshTokenExpiration { get; set; }
	}
}