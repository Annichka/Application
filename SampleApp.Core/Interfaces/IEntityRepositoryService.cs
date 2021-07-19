using SampleApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces
{
	public interface IEntityRepositoryService<TEntity, TFilter, TRepository>
		where TEntity : class
		where TFilter : class, IEntityFilter
		where TRepository : IRepository<TEntity>
	{
		Task<TEntity> GetAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<TEntity> GetFirstAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<TEntity> GetFirstOrDefaultAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<TEntity> GetUniqueAsync(TFilter filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<TEntity> GetUniqueOrDefaultAsync(TFilter filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<PageResult<TEntity>> ReadAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Page page = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);
		Task<int> CountAsync(TFilter filter);
		Task<bool> ExistsAsync(TFilter filter);
		Task SaveAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);
		Task<TEntity> DeleteAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);
	}
}
