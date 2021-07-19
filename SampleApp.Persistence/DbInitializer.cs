using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SampleApp.Core.Configuration;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Exceptions;
using SampleApp.Persistence;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;

namespace SwingersCRM.Persistence
{
	public static class DbInitializer
	{
		public static void Seed(Container container)
		{
			using (AsyncScopedLifestyle.BeginScope(container))
			{
				var context = container.GetInstance<DbContext>() as SampleAppDbContext ?? throw new ArgumentNullException(nameof(DbContext));
				var dbTransaction = container.GetInstance<IDbTransaction>() ?? throw new ArgumentNullException(nameof(IDbTransaction));
				var usersConfiguration = container.GetInstance<IOptions<UsersConfiguration>>().Value ?? throw new ArgumentNullException(nameof(UsersConfiguration));
				var userManager = container.GetInstance<UserManager<UserEntity>>() ?? throw new ArgumentNullException(nameof(UserManager<UserEntity>));
				var utcNow = DateTimeOffset.UtcNow;

				dbTransaction.DbContext.Database.Migrate();

				dbTransaction.Begin();

				#region Admin user

				var adminEmail = usersConfiguration.SuperAdmin.Email;
				var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
				if (adminUser == null)
				{
					var id = Guid.NewGuid();
					adminUser = new UserEntity
					{
						Id = id,
						UserName = adminEmail,
						Email = adminEmail,
						EmailConfirmed = true,
						FirstName = "Super",
						LastName = "Admin",
						IDNumber = "77777777777",
						IsMarried = false,
						HasJob = true,
						MonthlySalary = 10000,
						IsAdmin = true,
						CreatedById = id,
						CreatedAt = utcNow
					};

					var identityResult = userManager.CreateAsync(adminUser).Result;
					if (!identityResult.Succeeded)
					{
						throw new BusinessException("The admin user could not be created.");
					}
				}

				var isPasswordValid = userManager.CheckPasswordAsync(adminUser, usersConfiguration.SuperAdmin.Password).Result;
				if (!isPasswordValid)
				{
					var token = userManager.GeneratePasswordResetTokenAsync(adminUser).Result;
					var result = userManager.ResetPasswordAsync(adminUser, token, usersConfiguration.SuperAdmin.Password).Result;
					if (!result.Succeeded)
					{
						throw new BusinessException("The admin user's password could not be set.");
					}
				}

				#endregion


				dbTransaction.Commit();
			}
		}
	}
}