using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Core;
using SampleApp.Core.Entities.Address;
using SampleApp.Core.Entities.Identity;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces;
using SampleApp.Persistence.Repositories.Address;
using SampleApp.Persistence.Repositories.User;
using SimpleInjector;
using System;

namespace SampleApp.Persistence
{
	public class PersistenceIoCConfig : IIoCConfig
	{
		private readonly Container container;
		private readonly string connectionString;

		public PersistenceIoCConfig(Container container, string connectionString)
		{
			this.container = container;
			this.connectionString = connectionString;
		}

		public void AddServices(IServiceCollection services)
		{
			services.AddScoped<DbContext, SampleAppDbContext>();
			services.AddScoped<IDbTransaction, DbTransaction>();
			services
				.AddDbContext<SampleAppDbContext>(options =>
				{
					options.UseLazyLoadingProxies();
					options.UseSqlServer(connectionString, x =>
					{
						x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
					});
				}, ServiceLifetime.Scoped)
				.AddIdentity<UserEntity, RoleEntity>(options =>
				{
					options.Password.RequiredLength = 7;
					options.Password.RequireDigit = true;
					options.Password.RequiredUniqueChars = 0;
					options.Password.RequireLowercase = true;
					options.Password.RequireNonAlphanumeric = true;
					options.Password.RequireUppercase = true;
				}).AddEntityFrameworkStores<DbContext>()
				.AddDefaultTokenProviders();
		}

		public void RegisterDependencies()
		{
			#region Common
			container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);
			#endregion

			#region User
			container.Register<IRepository<UserEntity>, UserRepository>(Lifestyle.Scoped);
			#endregion

			#region Address
			container.Register<IRepository<AddressEntity>, AddressRepository>(Lifestyle.Scoped);
			#endregion
		}
	}

}
