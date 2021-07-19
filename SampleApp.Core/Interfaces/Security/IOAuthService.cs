using SampleApp.Core.Models;
using SampleApp.Core.Security;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces.Security
{
	public interface IOAuthService
	{
		Task<GenericResponse<AccessToken>> AuthorizePasswordAsync(string email,
			string password);

		Task<GenericResponse<AccessToken>> RefreshTokenAsync(string refreshToken);

		void RevokeAccessToken(string accessToken);
	}
}
