using SampleApp.Core.Interfaces;

namespace SampleApp.Core.Filter.User
{
	public class UserFilter : IEntityFilter
	{
		public string NameContains { get; set; }
		public bool? OnlyAdmins { get; set; }
	}
}
