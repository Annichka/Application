using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleApp.Core.Entities.Address;

namespace SampleApp.Persistence.EntityConfigurations.Address
{
	public class AddressEntityConfiguration : IEntityTypeConfiguration<AddressEntity>
	{
		public void Configure(EntityTypeBuilder<AddressEntity> builder)
		{
			builder.HasMany(x => x.Users).WithOne(x => x.Address).HasForeignKey(x => x.AddressId);
		}
	}
}
