using System;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Core.Models.User
{
	public class UserModel : ModelBase
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }
		[Required]
		[StringLength(11)]
		public string IDNumber { get; set; }
		[Required]
		public bool IsMarried { get; set; }
		[Required]
		public bool HasJob { get; set; }
		public decimal MonthlySalary { get; set; }
		[Required]
		public string Address{ get; set; }
	}
}
