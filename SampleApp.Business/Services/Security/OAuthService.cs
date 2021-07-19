using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces.Security;
using SampleApp.Core.Models;
using SampleApp.Core.Security;
using System;
using System.Threading.Tasks;

namespace SampleApp.Business.Services.Security
{
	public class OAuthService : IOAuthService
	{
		private readonly ILogger<OAuthService> logger;
		private readonly ITokenHandlerService tokenHandlerService;
		private readonly UserManager<UserEntity> userManager;
		private readonly SignInManager<UserEntity> signInManager;

		public OAuthService(ITokenHandlerService tokenHandlerService,UserManager<UserEntity> userManager,SignInManager<UserEntity> signInManager,ILogger<OAuthService> logger)
		{
			this.logger = logger;
			this.tokenHandlerService = tokenHandlerService;
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<GenericResponse<AccessToken>> AuthorizePasswordAsync(string email, string password)
		{
			GenericResponse<AccessToken> response = new GenericResponse<AccessToken>();

			var user = await userManager.FindByEmailAsync(email);
			if (user == null)
			{
				response.AddError("User does not exist.");
				return response;
			}

			var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, password, user.LockoutEnabled);

			if (!passwordCheck.Succeeded)
			{
				response.AddError("Incorrect credintials");
				return response;
			}

			var token = await tokenHandlerService.CreateAccessTokenAsync(user);

			logger.LogWarning($"[{DateTime.UtcNow}] ({user.Email}) User logged in.");

			response.Models.Add(token);
			return response;
		}

		public async Task<GenericResponse<AccessToken>> RefreshTokenAsync(string refreshToken)
		{
			var userName = tokenHandlerService.TakeUserToken(refreshToken);
			var response = new GenericResponse<AccessToken>();
			if (string.IsNullOrWhiteSpace(userName))
			{
				response.AddError("Refresh token is already expired!");
				return response;
			}

			var user = await userManager.FindByEmailAsync(userName);
			if (user == null)
			{
				response.AddError("User of Refresh token does not exist!");
			}
			else
			{
				var token = await tokenHandlerService.CreateAccessTokenAsync(user);
				response.Models.Add(token);
			}

			return response;
		}

		public void RevokeAccessToken(string accessToken)
		{
			tokenHandlerService.RevokeAccessToken(accessToken);
		}
	}

}
