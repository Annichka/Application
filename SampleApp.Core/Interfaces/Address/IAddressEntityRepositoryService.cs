using SampleApp.Core.Entities.Address;
using SampleApp.Core.Filter.Address;

namespace SampleApp.Core.Interfaces.Address
{
	public interface IAddressEntityRepositoryService : IEntityRepositoryService<AddressEntity, AddressFilter, IRepository<AddressEntity>>
	{
	}
}
