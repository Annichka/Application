using SampleApp.Core.Entities.User;
using SampleApp.Core.Filter.User;
using SampleApp.Core.Models.User;

namespace SampleApp.Core.Interfaces.User
{
	public interface IUserModelRepositoryService : IModelRepositoryService<UserEntity, UserModel, UserFilter>
	{
	}
}
