using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleApp.Core.Entities.Base
{
	public class TrackableEntityBase<TEntity> : EntityBase<TEntity>, ITrackable
		where TEntity : class
	{
		public virtual DateTimeOffset CreatedAt { get; set; }
		public virtual Guid CreatedById { get; set; }
		public virtual DateTimeOffset? ModifiedAt { get; set; }
		public virtual Guid? ModifiedById { get; set; }

		[ForeignKey(nameof(CreatedById))]
		public virtual UserEntity CreatedBy { get; set; }

		[ForeignKey(nameof(ModifiedById))]
		public virtual UserEntity ModifiedBy { get; set; }

		public override bool IsNew()
		{
			return base.IsNew() || CreatedAt == default || CreatedById == default;
		}

		public override void Configure(EntityTypeBuilder<TEntity> entityTypeBuilder)
		{
		}
	}
}
