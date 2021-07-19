using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleApp.Core.Entities.Address;
using SampleApp.Core.Entities.Identity;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Exceptions;
using SampleApp.Core.Interfaces;
using SampleApp.Persistence.EntityConfigurations.Address;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Persistence
{
	public class SampleAppDbContext : IdentityDbContext<
		UserEntity,
		RoleEntity,
		Guid,
		UserClaimEntity,
		UserRoleEntity,
		UserLoginEntity,
		RoleClaimEntity,
		UserTokenEntity>
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public SampleAppDbContext(DbContextOptions<SampleAppDbContext> options, IHttpContextAccessor httpContextAccessor)
			: base(options)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public DbSet<AddressEntity> Addresses { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			// Set Cascade Delete Behavior to Restrict except for entities in tableWithDeleteCascade
			string[] tableWithDeleteCascade = new string[] { };

			var cascadeFKs = builder.Model.GetEntityTypes()
				.SelectMany(t => t.GetForeignKeys())
				.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade && !tableWithDeleteCascade.Contains(fk.DeclaringEntityType.Name.Split('.').Last()));

			foreach (var fk in cascadeFKs)
			{
				fk.DeleteBehavior = DeleteBehavior.Restrict;
			}

			builder.ApplyConfiguration(new AddressEntityConfiguration());
			base.OnModelCreating(builder);
		}



		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			OnBeforeSaving();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
		{
			OnBeforeSaving();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void OnBeforeSaving()
		{
			if (Database.CurrentTransaction == null)
			{
				throw new BusinessException("Saving data to db without transaction is forbidden");
			}

			var entries = ChangeTracker.Entries();
			foreach (var entry in entries)
			{
				if (entry.Entity is ITrackable trackable)
				{
					var now = DateTime.UtcNow;
					var user = trackable.CreatedById == Guid.Empty ? GetCurrentUser() : trackable.CreatedById;

					switch (entry.State)
					{
						case EntityState.Modified:
							trackable.ModifiedAt = now;
							trackable.ModifiedById = user;
							break;

						case EntityState.Added:
							trackable.CreatedAt = now;
							trackable.CreatedById = user;
							break;

						case EntityState.Deleted:
							trackable.ModifiedAt = now;
							trackable.ModifiedById = user;
							break;
					}
				}
			}
		}

		private Guid GetCurrentUser()
		{
			var authenticatedUserId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			Guid.TryParse(authenticatedUserId, out Guid result);

			return result;
		}
	}

}
