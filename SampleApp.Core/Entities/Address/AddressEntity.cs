using SampleApp.Core.Entities.Base;
using SampleApp.Core.Entities.User;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Core.Entities.Address
{
	public class AddressEntity : TrackableEntityBase<AddressEntity>
	{
		[Required]
		public string Address { get; set; }

		public virtual ICollection<UserEntity> Users { get; set; }
	}
}
