namespace SampleApp.Core.Interfaces.Security
{
	public interface IPasswordHasherService
	{
		string HashPassword(string password);

		bool PasswordMatches(string providedPassword, string passwordHash);
	}
}
