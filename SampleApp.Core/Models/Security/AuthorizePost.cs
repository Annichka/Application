using System.ComponentModel.DataAnnotations;
using Json = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace SampleApp.Core.Models.Security
{
	public class AuthorizePost
	{
		[Json("username")]
		public string UserName { get; set; }

		[DataType(DataType.Password)]
		[Json("password")]
		public string Password { get; set; }
	}

}
