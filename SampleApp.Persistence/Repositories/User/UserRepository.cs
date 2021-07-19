using SampleApp.Core;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces;

namespace SampleApp.Persistence.Repositories.User
{
	public class UserRepository : GenericRepository<UserEntity>
	{
		public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}
	}
}
