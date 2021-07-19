using Microsoft.EntityFrameworkCore;
using SampleApp.Core.Entities.Address;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Filter.Address;
using SampleApp.Core.Filter.User;
using SampleApp.Core.Interfaces.Address;
using SampleApp.Core.Interfaces.User;
using SampleApp.Core.Models;
using SampleApp.Core.Models.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Business.Services.User
{
	public class UserModelRepositoryService : ModelRepositoryService<UserEntity, UserModel, UserFilter>, IUserModelRepositoryService
	{
		private readonly IAddressEntityRepositoryService addressEntityRepositoryService;
		public UserModelRepositoryService(IUserEntityRepositoryService UserEntityRepositoryService, IAddressEntityRepositoryService addressEntityRepositoryService) : base(UserEntityRepositoryService)
		{
			this.addressEntityRepositoryService = addressEntityRepositoryService;
		}

		protected override async Task<List<EntityModelPair<UserEntity, UserModel>>> ToModels(List<EntityModelPair<UserEntity, UserModel>> pairs)
		{
			pairs = await base.ToModels(pairs);

			foreach (var pair in pairs)
			{
				pair.Model.FirstName = pair.Entity.FirstName;
				pair.Model.LastName = pair.Entity.LastName;
				pair.Model.IDNumber = pair.Entity.IDNumber;
				pair.Model.IsMarried = pair.Entity.IsMarried;
				pair.Model.HasJob = pair.Entity.HasJob;
				pair.Model.MonthlySalary = pair.Entity.MonthlySalary;
				pair.Model.Address = pair.Entity.Address?.Address ?? string.Empty;
			}

			return pairs;
		}

		protected override async Task<List<EntityModelPair<UserEntity, UserModel>>> ToEntities(List<EntityModelPair<UserEntity, UserModel>> pairs)
		{
			pairs = await base.ToEntities(pairs);

			foreach (var pair in pairs)
			{
				pair.Entity.FirstName = pair.Model.FirstName;
				pair.Entity.LastName = pair.Model.LastName;
				pair.Entity.IDNumber = pair.Model.IDNumber;
				pair.Entity.IsMarried = pair.Model.IsMarried;
				pair.Entity.HasJob = pair.Model.HasJob;
				pair.Entity.MonthlySalary = pair.Model.MonthlySalary;

				var addressFilter = new AddressFilter()
				{
					NameEquals = pair.Model.Address
				};
				var existedAddress = await addressEntityRepositoryService.GetFirstOrDefaultAsync(addressFilter);
				if (existedAddress == null)
				{
					var newAddress = new AddressEntity()
					{
						Address = pair.Model.Address
					};
					await addressEntityRepositoryService.SaveAsync(newAddress);
					existedAddress = await addressEntityRepositoryService.GetFirstOrDefaultAsync(addressFilter);
				}
				pair.Entity.AddressId = existedAddress.Id;
				pair.Entity.Address = existedAddress;
			}

			return pairs;

		}

		protected override IQueryable<UserEntity> GetAssociations(IQueryable<UserEntity> query)
		{
			return base.GetAssociations(query).Include(x => x.Address);
		}
	}
}
