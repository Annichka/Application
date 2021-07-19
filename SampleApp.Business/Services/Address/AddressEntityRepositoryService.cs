using SampleApp.Core.Entities.Address;
using SampleApp.Core.Enums;
using SampleApp.Core.Filter.Address;
using SampleApp.Core.Interfaces;
using SampleApp.Core.Interfaces.Address;
using SampleApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SampleApp.Business.Services.Address
{
	public class AddressEntityRepositoryService : EntityRepositoryService<AddressEntity, AddressFilter, IRepository<AddressEntity>>, IAddressEntityRepositoryService
	{
		public AddressEntityRepositoryService(IRepository<AddressEntity> repository) : base(repository)
		{
		}

		protected override Expression<Func<AddressEntity, bool>> GetMatchByFilter(AddressFilter filter, ref bool suppressQuery)
		{
			Expression<Func<AddressEntity, bool>> result = x =>
				(string.IsNullOrEmpty(filter.NameEquals) || x.Address.Equals(filter.NameEquals)) &&
				(!filter.CreatedById.HasValue || x.CreatedById.Equals(filter.CreatedById.Value)) &&
				(!filter.IdNotEquals.HasValue || !x.Id.Equals(filter.IdNotEquals.Value));

			return result;
		}

		protected override List<(Expression<Func<AddressEntity, object>>, SortDirection)> GetOrderFields(IEnumerable<Sort> sorts)
		{
			var result = base.GetOrderFields(sorts);

			foreach (var sort in sorts)
			{
				if (string.Compare(sort.Field, "Address", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result.Add((x => x.Address, sort.Dir));
				}
			}

			return result;
		}
	}
}
