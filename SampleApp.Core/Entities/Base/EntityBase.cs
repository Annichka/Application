using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.Core.Entities.Base
{
	public abstract class EntityBase<TEntity> : IEntityBase, IEntityTypeConfiguration<TEntity>
		where TEntity : class
	{
		[Key]
		public Guid Id { get; set; }

		public virtual bool IsNew()
		{
			return Id == null || Id == Guid.Empty;
		}

		public abstract void Configure(EntityTypeBuilder<TEntity> entityTypeBuilder);
	}
}
