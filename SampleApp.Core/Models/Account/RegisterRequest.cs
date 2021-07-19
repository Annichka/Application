using System.ComponentModel.DataAnnotations;

namespace SampleApp.Core.Models.Account
{
	public class RegisterRequest
	{
		[Required]
		[StringLength(32)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(32)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[DataType(DataType.EmailAddress)]
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }
	}

}
