using Microsoft.EntityFrameworkCore;
using SampleApp.Core.Interfaces;
using System.Threading.Tasks;

namespace SampleApp.Core
{
	public class UnitOfWork : IUnitOfWork
	{
		public DbContext Context { get; }

		public UnitOfWork(DbContext context)
		{
			Context = context;
		}

		public void Commit()
		{
			Context.SaveChanges();
		}

		public async Task CommitAsync()
		{
			await Context.SaveChangesAsync();
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}
