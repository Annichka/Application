using Microsoft.AspNetCore.Http;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Enums;
using SampleApp.Core.Exceptions;
using SampleApp.Core.Filter.Address;
using SampleApp.Core.Filter.User;
using SampleApp.Core.Interfaces;
using SampleApp.Core.Interfaces.Address;
using SampleApp.Core.Interfaces.User;
using SampleApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SampleApp.Business.Services.User
{
	public class UserEntityRepositoryService : EntityRepositoryService<UserEntity, UserFilter, IRepository<UserEntity>>, IUserEntityRepositoryService
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IAddressEntityRepositoryService addressEntityRepositoryService;
		public UserEntityRepositoryService(IRepository<UserEntity> repository, IHttpContextAccessor httpContextAccessor, IAddressEntityRepositoryService addressEntityRepositoryService) : base(repository)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.addressEntityRepositoryService = addressEntityRepositoryService;
		}

		protected override async Task ValidateAsync(UserEntity entity, UserEntity originalEntity, IDictionary<string, object> additionalParameters)
		{
			await base.ValidateAsync(entity, originalEntity, additionalParameters);
			if (entity.HasJob && entity.MonthlySalary <= 0)
			{
				throw new BusinessException("Monthly Salary must be greater than 0");
			}
			else if (!entity.HasJob && entity.MonthlySalary > 0)
			{
				throw new BusinessException("Monthly Salary must be 0, because user does not have job");
			}
		}

		protected override async Task SavingAsync(UserEntity entity, UserEntity originalEntity, bool suppressSave, IDictionary<string, object> additionalParameters)
		{
			await base.SavingAsync(entity, originalEntity, suppressSave, additionalParameters);

			var loggedInUserId = GetLoggedInUserId();
			if (entity.Id != loggedInUserId)
			{
				var loggedInUser = await GetAsync(loggedInUserId);
				if (!loggedInUser.IsAdmin)
				{
					throw new BusinessException("Only Admins are allowed to get/update/delete other user's profile");
				}
			}

			if (!string.IsNullOrEmpty(originalEntity.IDNumber) && originalEntity.IDNumber != entity.IDNumber)
			{
				throw new BusinessException("Users are not allowed to change ID Number.");
			}
		}

		protected override async Task DeletingAsync(UserEntity entity, IDictionary<string, object> additionalParameters)
		{
			await base.DeletingAsync(entity, additionalParameters);

			var loggedInUserId = GetLoggedInUserId();
			if (entity.Id != loggedInUserId)
			{
				var loggedInUser = await GetAsync(loggedInUserId);
				if (!loggedInUser.IsAdmin)
				{
					throw new BusinessException("Only Admins are allowed to get/update/delete other users");
				}
			}

			//replace createdBy & modifiedBy to adminUser for this user's address.
			var address = entity.Address;
			if(address != null)
			{
				if (address.CreatedById == entity.Id)
				{
					var userFilter = new UserFilter()
					{
						OnlyAdmins = true
					};
					var adminUser = await GetFirstOrDefaultAsync(userFilter);
					if (adminUser != null)
					{
						address.CreatedById = adminUser.Id;
						address.CreatedBy = adminUser;
						await addressEntityRepositoryService.SaveAsync(address);
					}

					if (address.ModifiedById == entity.Id)
					{
						if (adminUser != null)
						{
							address.ModifiedById = adminUser.Id;
							address.ModifiedBy = adminUser;
							await addressEntityRepositoryService.SaveAsync(address);
						}
					}
				}
			}

			//replace createdBy & modifiedBy to adminUser for addresses that are createdByThisUser.
			var addressFilter = new AddressFilter()
			{
				CreatedById = entity.Id,
				IdNotEquals = address?.Id
			};
			var addressesThatAreCreatedByThisUser = (await addressEntityRepositoryService.ReadAsync(addressFilter)).Items.ToList();
			if (addressesThatAreCreatedByThisUser.Any())
			{
				var userFilter = new UserFilter()
				{
					OnlyAdmins = true
				};
				var adminUser = await GetFirstOrDefaultAsync(userFilter);

				foreach (var existedAddress in addressesThatAreCreatedByThisUser)
				{
					if (existedAddress.CreatedById == entity.Id)
					{
						if (adminUser != null)
						{
							existedAddress.CreatedById = adminUser.Id;
							existedAddress.CreatedBy = adminUser;
							await addressEntityRepositoryService.SaveAsync(existedAddress);
						}

					}

					if (existedAddress.ModifiedById == entity.Id)
					{
						if (adminUser != null)
						{
							existedAddress.ModifiedById = adminUser.Id;
							existedAddress.ModifiedBy = adminUser;
							await addressEntityRepositoryService.SaveAsync(existedAddress);
						}
					}

				}
			}

		}

		protected override async Task FindingAsync(Guid id)
		{
			await base.FindingAsync(id);

			var loggedInUserId = GetLoggedInUserId();
			if (id != loggedInUserId)
			{
				var loggedInUser = await GetAsync(loggedInUserId);
				if (!loggedInUser.IsAdmin)
				{
					throw new BusinessException("Only Admins are allowed to get/update/delete other user's profile");
				}
			}
		}

		protected override Expression<Func<UserEntity, bool>> GetMatchByFilter(UserFilter filter, ref bool suppressQuery)
		{
			var loggedInUserId = GetLoggedInUserId();
			var loggedInUser = GetAsync(loggedInUserId).Result;
			if (!loggedInUser.IsAdmin && !filter.OnlyAdmins.HasValue)
			{
				suppressQuery = true;
			}

			Expression<Func<UserEntity, bool>> result = x =>
				(string.IsNullOrEmpty(filter.NameContains) || x.UserName.Contains(filter.NameContains)) &&
				(!filter.OnlyAdmins.HasValue || x.IsAdmin);

			return result;
		}

		protected override List<(Expression<Func<UserEntity, object>>, SortDirection)> GetOrderFields(IEnumerable<Sort> sorts)
		{
			var result = base.GetOrderFields(sorts);

			foreach (var sort in sorts)
			{
				if (string.Compare(sort.Field, "FirstName", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result.Add((x => x.UserName, sort.Dir));
				}
			}

			return result;
		}

		private Guid GetLoggedInUserId()
		{
			var authenticatedUserId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			Guid.TryParse(authenticatedUserId, out Guid result);

			return result;
		}
	}
}
