using SampleApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SampleApp.Core.Interfaces
{
	public interface IRepository<TEntity>
		where TEntity : class
	{
		IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties);

		Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includeProperties);

		TEntity Get(Guid id);

		Task<TEntity> GetAsync(Guid id);

		TEntity Add(TEntity entity);

		void AddRange(IEnumerable<TEntity> entities);

		Task<TEntity> AddAsync(TEntity entity);

		Task AddRangeAsync(IEnumerable<TEntity> entities);

		TEntity Find(Expression<Func<TEntity, bool>> match, params Expression<Func<TEntity, object>>[] includeProperties);

		Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, params Expression<Func<TEntity, object>>[] includeProperties);

		void Delete(TEntity entity);

		void DeleteRange(IEnumerable<TEntity> entities);

		Task DeleteAsync(TEntity entity);

		Task DeleteRangeAsync(IEnumerable<TEntity> entities);

		void Update(TEntity entity);

		void UpdateRange(IEnumerable<TEntity> entities);

		int Count();

		Task<int> CountAsync();

		int Count(Expression<Func<TEntity, bool>> filter);

		Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);

		int Save();

		Task<int> SaveAsync();

		IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> match, IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null, params Expression<Func<TEntity, object>>[] includeProperties);

		Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> match, IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null, params Expression<Func<TEntity, object>>[] includeProperties);

		IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

		(IEnumerable<TEntity> items, int count) FindAllPaged(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			params Expression<Func<TEntity, object>>[] includeProperties);

		Task<(IEnumerable<TEntity> items, int count)> FindAllPagedAsync(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			params Expression<Func<TEntity, object>>[] includeProperties);

		(IEnumerable<TEntity> items, int count) FindAllPaged(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);

		Task<(IEnumerable<TEntity> items, int count)> FindAllPagedAsync(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null);

		void Dispose();
	}

}
