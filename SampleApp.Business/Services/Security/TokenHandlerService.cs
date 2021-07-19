using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SampleApp.Core.Configuration;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces.Security;
using SampleApp.Core.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleApp.Business.Services.Security
{
	public class TokenHandlerService : ITokenHandlerService
	{
		private readonly IPasswordHasherService passwordHasherService;
		private readonly TokenConfigurations tokenConfigurations;
		private readonly SigningConfigurations signingConfigurations;
		private readonly IDistributedCache tokensDistributedCache;

		public TokenHandlerService(IOptions<TokenConfigurations> tokenOptionsSnapshot, SigningConfigurations signingConfigurations, IPasswordHasherService passwordHasherService, IDistributedCache tokensDistributedCache)
		{
			tokenConfigurations = tokenOptionsSnapshot.Value;
			this.signingConfigurations = signingConfigurations;
			this.passwordHasherService = passwordHasherService;
			this.tokensDistributedCache = tokensDistributedCache;
		}

		public async Task<AccessToken> CreateAccessTokenAsync(UserEntity user)
		{
			var (refreshToken, expirationRefreshToken) = BuildRefreshToken();
			var (accessToken, expirationAccessToken) = BuildAccessToken(user, refreshToken);

			await SetStringInCacheAsync(accessToken.Token, user.Email, expirationAccessToken);

			await SetStringInCacheAsync(refreshToken.Token, user.Email, expirationRefreshToken);

			return accessToken;
		}

		private async Task SetStringInCacheAsync(string token, string email, long expirationTime)
		{
			var distributedCacheEntry = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirationTime)
			};

			await tokensDistributedCache.SetStringAsync(token, email, distributedCacheEntry);
		}

		public string TakeUserToken(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
			{
				throw new Exception("refresh_token must not be empty!");
			}

			var userName = tokensDistributedCache.GetString(refreshToken);

			if (!string.IsNullOrWhiteSpace(userName))
			{
				tokensDistributedCache.Remove(refreshToken);
			}

			return userName;
		}

		public void RevokeAccessToken(string token)
		{
			tokensDistributedCache.Remove(token);
		}

		#region Private Methods

		private (RefreshToken, long) BuildRefreshToken()
		{
			var seconds = tokenConfigurations.RefreshTokenExpiration;

			var refreshToken = new RefreshToken
			(
				token: passwordHasherService.HashPassword(Guid.NewGuid().ToString()),
				expiration: DateTime.UtcNow.AddSeconds(seconds).Ticks
			);

			return (refreshToken, seconds);
		}

		private (AccessToken, long) BuildAccessToken(UserEntity user,
			RefreshToken refreshToken)
		{
			var seconds = tokenConfigurations.AccessTokenExpiration;

			var accessTokenExpiration = DateTime.UtcNow.AddSeconds(seconds);


			var securityToken = new JwtSecurityToken
			(
				issuer: tokenConfigurations.Issuer,
				audience: tokenConfigurations.Audience,
				claims: GetClaims(user),
				expires: accessTokenExpiration,
				notBefore: DateTime.UtcNow,
				signingCredentials: signingConfigurations.SigningCredentials
			);

			var handler = new JwtSecurityTokenHandler();
			var accessToken = handler.WriteToken(securityToken);

			return (new AccessToken(accessToken, accessTokenExpiration.Ticks, refreshToken), seconds);
		}

		private IEnumerable<Claim> GetClaims(UserEntity user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			};

			return claims;
		}

		#endregion
	}

}
