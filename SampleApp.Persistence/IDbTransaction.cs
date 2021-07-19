using Microsoft.EntityFrameworkCore;
using System;

namespace SampleApp.Persistence
{
	public interface IDbTransaction : IDisposable
	{
		DbContext DbContext { get; }

		void Begin();
		void Commit();
		void Rollback();

		event EventHandler Committing;
		event EventHandler Committed;
	}
}
