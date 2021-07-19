using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Business.Services.Account;
using SampleApp.Business.Services.Address;
using SampleApp.Business.Services.Security;
using SampleApp.Business.Services.User;
using SampleApp.Core.Configuration;
using SampleApp.Core.Interfaces;
using SampleApp.Core.Interfaces.Account;
using SampleApp.Core.Interfaces.Address;
using SampleApp.Core.Interfaces.Security;
using SampleApp.Core.Interfaces.User;
using SampleApp.Persistence;
using SimpleInjector;
using System;

namespace SampleApp.Business
{
	public class BusinessIoCConfig : IIoCConfig
	{
		private readonly Container container;

		public BusinessIoCConfig(Container container, IServiceCollection services, IConfiguration configuration)
		{
			this.container = container ?? throw new ArgumentNullException(nameof(container));

			PersistenceIoCConfig persConfig = new PersistenceIoCConfig(container, configuration.GetConnectionString("DefaultConnection"));
			persConfig.AddServices(services);
			persConfig.RegisterDependencies();


			AddServices(services, configuration);
			RegisterDependencies();
		}

		private void AddServices(IServiceCollection services, IConfiguration configuration)
		{
			var usersConfig = configuration.GetSection(nameof(UsersConfiguration));
			services.Configure<UsersConfiguration>(options =>
			{
				options.SuperAdmin = usersConfig.GetSection(nameof(UsersConfiguration.SuperAdmin)).Get<UserConfiguration>();
			});
		}

		public void RegisterDependencies()
		{
			container.Register<IAccountService, AccountService>(Lifestyle.Scoped);
			container.Register<IPasswordHasherService, PasswordHasherService>(Lifestyle.Singleton);
			container.Register<ITokenHandlerService, TokenHandlerService>(Lifestyle.Scoped);
			container.Register<IOAuthService, OAuthService>(Lifestyle.Scoped);

			#region User
			container.Register<IUserEntityRepositoryService, UserEntityRepositoryService>();
			container.Register<IUserModelRepositoryService, UserModelRepositoryService>();
			#endregion

			#region Address
			container.Register<IAddressEntityRepositoryService, AddressEntityRepositoryService>();
			#endregion
		}
	}

}
