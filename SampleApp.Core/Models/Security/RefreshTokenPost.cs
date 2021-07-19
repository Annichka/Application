using System.ComponentModel.DataAnnotations;

namespace SampleApp.Core.Models.Security
{
	public class RefreshTokenPost
	{
		[Required]
		public string RefreshToken { get; set; }
	}
}
