using SampleApp.Core.Entities.User;
using SampleApp.Core.Models;
using SampleApp.Core.Models.Account;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces.Account
{
	public interface IAccountService
	{

		Task<GenericResponse<UserEntity>> RegisterAsync(
			RegisterRequest request);
	}

}
