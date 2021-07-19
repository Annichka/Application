using Microsoft.AspNetCore.Identity;
using SampleApp.Core.Entities.Address;
using SampleApp.Core.Entities.Base;
using SampleApp.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Core.Entities.User
{
	public class UserEntity : IdentityUser<Guid>, IEntityBase, ITrackable
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }
		[Required]
		[MaxLength(11)]
		public string IDNumber { get; set; }
		[Required]
		public bool IsMarried { get; set; }
		[Required]
		public bool HasJob { get; set; }
		public decimal MonthlySalary { get; set; }

		public Guid? AddressId { get; set; }
		[ForeignKey(nameof(AddressId))]
		public virtual AddressEntity Address { get; set; }

		public bool IsAdmin { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public Guid CreatedById { get; set; }

		public DateTimeOffset? ModifiedAt { get; set; }

		public Guid? ModifiedById { get; set; }
		public bool IsNew()
		{
			return Id == null || Id == Guid.Empty || CreatedAt == default || CreatedById == default;
		}
	}
}
