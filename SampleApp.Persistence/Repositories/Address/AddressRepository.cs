using SampleApp.Core;
using SampleApp.Core.Entities.Address;
using SampleApp.Core.Interfaces;

namespace SampleApp.Persistence.Repositories.Address
{
	public class AddressRepository : GenericRepository<AddressEntity>
	{
		public AddressRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}
	}
}
