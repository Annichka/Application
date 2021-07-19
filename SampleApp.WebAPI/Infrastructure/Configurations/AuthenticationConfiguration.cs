using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SampleApp.Core.Configuration;
using SimpleInjector;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.WebAPI.Infrastructure.Configurations
{
	public static class AuthenticationConfiguration
	{
		public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, Container container)
		{
			var signingConfigurations = new SigningConfigurations();
			var tokenConfigurationsSection = configuration.GetSection(nameof(TokenConfigurations));
			var tokenConfigurations = tokenConfigurationsSection.Get<TokenConfigurations>();

			services.Configure<TokenConfigurations>(tokenConfigurationsSection);

			services.AddSingleton(signingConfigurations);

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(jwtBearerOptions =>
				{
					jwtBearerOptions.RequireHttpsMetadata = false;
					jwtBearerOptions.SaveToken = true;

					jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = tokenConfigurations.Issuer,
						ValidAudience = tokenConfigurations.Audience,
						IssuerSigningKey = signingConfigurations.Key,
						ClockSkew = TimeSpan.Zero
					};

					jwtBearerOptions.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{

							if (context.Request.Headers.Any(x => x.Key == "Authorization"
									&& !string.IsNullOrEmpty(x.Value)))
							{
								context.Token = context.Request.Headers["Authorization"].ToString().Remove(0, 7);
							}
							else if (context.Request.Cookies.Any(c => c.Key == "access_token"))
							{
								context.Token = context.Request.Cookies["access_token"].ToString().Remove(0, 7);
							}

							return Task.CompletedTask;
						},

						OnAuthenticationFailed = context =>
						{
							if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
							{
								context.Response.Headers.Add("Token-Expired", "true");
							}
							return Task.CompletedTask;
						},

						OnTokenValidated = context =>
						{
							return Task.CompletedTask;
						}
					};
				});

			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
				  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
				  .RequireAuthenticatedUser()
				  .Build();
			});
		}
	}

}
