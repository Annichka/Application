using SampleApp.Core.Entities.User;
using SampleApp.Core.Filter.User;

namespace SampleApp.Core.Interfaces.User
{
	public interface IUserEntityRepositoryService : IEntityRepositoryService<UserEntity, UserFilter, IRepository<UserEntity>>
	{
	}
}
