using SampleApp.Core.Entities.User;
using SampleApp.Core.Security;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces.Security
{
	public interface ITokenHandlerService
	{
		Task<AccessToken> CreateAccessTokenAsync(UserEntity user);

		string TakeUserToken(string refreshToken);

		void RevokeAccessToken(string token);
	}
}
