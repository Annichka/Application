using Microsoft.AspNetCore.Identity;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces.Account;
using SampleApp.Core.Models;
using SampleApp.Core.Models.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Business.Services.Account
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<UserEntity> userManager;

		public AccountService(UserManager<UserEntity> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<GenericResponse<UserEntity>> RegisterAsync(RegisterRequest request)
		{
			var id = Guid.NewGuid();
			var user = new UserEntity
			{
				Id = id,
				UserName = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				CreatedAt = DateTime.UtcNow,
				Email = request.Email,
				IDNumber = string.Empty,
				CreatedById = id,
			};

			IdentityResult result = await userManager.CreateAsync(user, request.Password);

			var response = new GenericResponse<UserEntity>();

			if (!result.Succeeded)
			{
				response.Succeded = false;
				response.Errors.AddRange(result.Errors.Select(r => r.Description));
			}

			response.Models.Add(user);
			return response;
		}


	}

}
