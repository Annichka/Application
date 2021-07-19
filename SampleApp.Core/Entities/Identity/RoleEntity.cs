using Microsoft.AspNetCore.Identity;
using SampleApp.Core.Entities.Base;
using SampleApp.Core.Interfaces;
using System;

namespace SampleApp.Core.Entities.Identity
{
	public class RoleEntity : IdentityRole<Guid>, IEntityBase, ITrackable
	{
		public DateTimeOffset CreatedAt { get; set; }

		public Guid CreatedById { get; set; }

		public DateTimeOffset? ModifiedAt { get; set; }

		public Guid? ModifiedById { get; set; }
		public bool IsNew()
		{
			throw new NotImplementedException();
		}
	}
}


