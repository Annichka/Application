using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		DbContext Context { get; }

		void Commit();

		Task CommitAsync();
	}
}
