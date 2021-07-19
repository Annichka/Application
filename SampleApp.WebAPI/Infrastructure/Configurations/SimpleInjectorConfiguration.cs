using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Business;
using SampleApp.Core;
using SampleApp.Core.Entities.User;
using SampleApp.WebAPI.Infrastructure.Attributes;
using SimpleInjector;

namespace SampleApp.WebAPI.Infrastructure.Configurations
{
	public static class SimpleInjectorConfiguration
	{
		public static void ConfigureSimpleInjector(this IServiceCollection services, IConfiguration configuration, Container container)
		{
			services.AddScoped<ExecuteInTransactionImplAttribute>();
			services.AddSimpleInjector(container, options =>
			{
				options
					.AddAspNetCore()
					.AddControllerActivation();

				options.CrossWire<UserManager<UserEntity>>();

				options.CrossWire<SignInManager<UserEntity>>();
			});

			BusinessIoCConfig infoConfig = new BusinessIoCConfig(container, services, configuration);
			ServiceLocator.container = container;

			services.UseSimpleInjectorAspNetRequestScoping(container);
		}
	}
}
