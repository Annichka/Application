using SampleApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces
{
	public interface IModelRepositoryService<TEntity, TModel, TFilter>
		where TEntity : class
		where TModel : ModelBase
		where TFilter : class, IEntityFilter
	{
		IEntityRepositoryService<TEntity, TFilter, IRepository<TEntity>> EntityRepositoryService { get; }
		Task<TModel> GetAsync(Guid id);
		Task<PageResult<TModel>> ReadAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Page page = null);
		Task<TModel> CreateAsync(TModel model);
		Task<TModel> UpdateAsync(TModel model);
		Task<TModel> DeleteAsync(Guid id);
	}
}
