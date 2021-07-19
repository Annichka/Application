using Microsoft.EntityFrameworkCore;
using SampleApp.Core.Entities.Base;
using SampleApp.Core.Enums;
using SampleApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SampleApp.Core
{
	public abstract class GenericRepository<TEntity> : IRepository<TEntity>
		where TEntity : class, IEntityBase, new()
	{
		protected readonly IUnitOfWork unitOfWork;
		private bool disposed;

		public GenericRepository(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public virtual IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var result = Include(includeProperties)
				.ToList();

			return result;
		}

		public virtual async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var result = await Include(includeProperties)
				.ToListAsync()
				.ConfigureAwait(false);

			return result;
		}

		public virtual TEntity Get(Guid id)
		{
			var entity = unitOfWork.Context.Set<TEntity>().Find(id);

			if (entity != null)
			{
				unitOfWork.Context.Entry(entity).State = EntityState.Detached;

				return entity;
			}

			return null;
		}

		public virtual async Task<TEntity> GetAsync(Guid id)
		{
			var entity = await unitOfWork.Context.Set<TEntity>()
				.FindAsync(id)
				.ConfigureAwait(false);

			if (entity != null)
			{
				unitOfWork.Context.Entry(entity).State = EntityState.Detached;

				return entity;
			}

			return null;
		}

		public virtual TEntity Add(TEntity entity)
		{
			var result = unitOfWork.Context.Set<TEntity>().Add(entity);

			return result.Entity;
		}

		public virtual void AddRange(IEnumerable<TEntity> entities)
		{
			unitOfWork.Context.Set<TEntity>().AddRange(entities);
		}

		public virtual async Task<TEntity> AddAsync(TEntity entity)
		{
			var result = await unitOfWork.Context.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);

			return result.Entity;
		}

		public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
		{
			await unitOfWork.Context.Set<TEntity>()
				.AddRangeAsync(entities)
				.ConfigureAwait(false);
		}

		public virtual TEntity Find(Expression<Func<TEntity, bool>> match, params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var result = Include(includeProperties)
				.FirstOrDefault(match);

			return result;
		}

		public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var result = await Include(includeProperties)
				.FirstOrDefaultAsync(match)
				.ConfigureAwait(false);

			return result;
		}

		public virtual void Delete(TEntity entity)
		{
			var existing = unitOfWork.Context.Set<TEntity>().Find(entity);
			if (existing != null)
				unitOfWork.Context.Set<TEntity>().Remove(entity);
		}

		public virtual void DeleteRange(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
			{
				Delete(entity);
			}
		}

		public virtual async Task DeleteAsync(TEntity entity)
		{
			unitOfWork.Context.Set<TEntity>().Remove(entity);
			await Task.CompletedTask;
		}

		public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
		{
			unitOfWork.Context.Set<TEntity>().RemoveRange(entities);
			await Task.CompletedTask;
		}

		public virtual void Update(TEntity entity)
		{
			entity = entity ?? throw new ArgumentNullException(nameof(entity));

			unitOfWork.Context.Entry(entity).State = EntityState.Modified;
		}

		public virtual void UpdateRange(IEnumerable<TEntity> entities)
		{
			entities = entities ?? throw new ArgumentNullException(nameof(entities));

			foreach (var entity in entities)
			{
				Update(entity);
			}
		}

		public virtual int Count()
		{
			return unitOfWork.Context.Set<TEntity>().AsNoTracking().Count();
		}

		public virtual async Task<int> CountAsync()
		{
			return await unitOfWork.Context.Set<TEntity>().AsNoTracking().CountAsync().ConfigureAwait(false);
		}

		public virtual int Count(Expression<Func<TEntity, bool>> filter)
		{
			return unitOfWork.Context.Set<TEntity>()
				.AsNoTracking()
				.Where(filter)
				.Count();
		}

		public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter)
		{
			return await unitOfWork.Context.Set<TEntity>()
				.AsNoTracking()
				.Where(filter)
				.CountAsync()
				.ConfigureAwait(false);
		}

		public virtual int Save()
		{
			return unitOfWork.Context.SaveChanges();
		}

		public async virtual Task<int> SaveAsync()
		{
			return await unitOfWork.Context.SaveChangesAsync().ConfigureAwait(false);
		}

		public virtual IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> match, IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null, params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var expression = Include(includeProperties).Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			var result = expression
				.ToList();

			return result;
		}

		public virtual async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> match, IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null, params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var expression = Include(includeProperties).Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			var result = await expression
				.ToListAsync()
				.ConfigureAwait(false);

			return result;
		}

		public virtual IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var result = Include(includeProperties).ToList();

			return result;
		}

		public virtual (IEnumerable<TEntity> items, int count) FindAllPaged(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var expression = Include(includeProperties).Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			var result = expression.Skip((currentPage - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var count = Count(match);

			return (result, count);
		}



		public virtual async Task<(IEnumerable<TEntity> items, int count)> FindAllPagedAsync(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			params Expression<Func<TEntity, object>>[] includeProperties)
		{
			var expression = Include(includeProperties).Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			// EF Core doesn't support multiple parallel operations being run on the same context instance.
			// Info: https://docs.microsoft.com/en-us/ef/core/querying/async
			var result = await expression.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
			var count = await CountAsync(match).ConfigureAwait(false);

			return (result, count);
		}

		public virtual (IEnumerable<TEntity> items, int count) FindAllPaged(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var expression = (IQueryable<TEntity>)unitOfWork.Context.Set<TEntity>().AsNoTracking();

			if (getAssociations != null)
			{
				expression = getAssociations(expression);
			}

			expression = expression.Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			var result = expression.Skip((currentPage - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var count = Count(match);

			return (result, count);
		}

		public virtual async Task<(IEnumerable<TEntity> items, int count)> FindAllPagedAsync(
			Expression<Func<TEntity, bool>> match,
			int currentPage = 1,
			int pageSize = 10,
			IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orderFields = null,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var expression = (IQueryable<TEntity>)unitOfWork.Context.Set<TEntity>().AsNoTracking();

			if (getAssociations != null)
			{
				expression = getAssociations(expression);
			}

			expression = expression.Where(match);

			if (orderFields != null && orderFields.Any())
			{
				expression = Order(expression, orderFields);
			}

			// EF Core doesn't support multiple parallel operations being run on the same context instance.
			// Info: https://docs.microsoft.com/en-us/ef/core/querying/async
			var result = await expression.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
			var count = await CountAsync(match).ConfigureAwait(false);

			return (result, count);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					unitOfWork.Context.Dispose();
				}

				this.disposed = true;
			}
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> queryable = unitOfWork.Context.Set<TEntity>().AsNoTracking();

			if (includeProperties != null)
			{
				for (int i = 0; i < includeProperties.Length; i++)
				{
					queryable = queryable.Include<TEntity, object>(includeProperties[i]);
				}
			}

			return queryable;
		}

		protected virtual IOrderedQueryable<TEntity> Order(IQueryable<TEntity> expression, IEnumerable<(Expression<Func<TEntity, object>> item, SortDirection direction)> orders)
		{
			IOrderedQueryable<TEntity> orderedResult = null;
			var ordersArray = orders.ToArray();

			var orderBy = ordersArray[0];
			switch (orderBy.direction)
			{
				case SortDirection.Asc:
					orderedResult = expression.OrderBy(orderBy.item);
					break;
				case SortDirection.Desc:
					orderedResult = expression.OrderByDescending(orderBy.item);
					break;
			}

			if (orders.Count() > 1)
			{
				for (int i = 1; i < ordersArray.Count(); i++)
				{
					orderBy = ordersArray[i];
					switch (orderBy.direction)
					{
						case SortDirection.Asc:
							orderedResult = orderedResult.ThenBy(orderBy.item);
							break;
						case SortDirection.Desc:
							orderedResult = orderedResult.ThenByDescending(orderBy.item);
							break;
					}
				}
			}

			return orderedResult;
		}

		protected virtual void UpdateEntity(TEntity entity)
		{
			TEntity exist = unitOfWork.Context.Set<TEntity>().Find(entity.Id);
			if (exist != null)
			{
				unitOfWork.Context.Entry(exist)
					.CurrentValues
					.SetValues(entity);
			}
			else
			{
				throw new Exception($"Entity with id {entity.Id} not found.");
			}
		}

		protected virtual async Task UpdateEntityAsync(TEntity entity)
		{
			TEntity exist = await unitOfWork.Context.Set<TEntity>().FindAsync(entity.Id)
				.ConfigureAwait(false);

			if (exist != null)
			{
				unitOfWork.Context.Entry(exist)
					.CurrentValues
					.SetValues(entity);
			}
			else
			{
				throw new Exception($"Entity with id {entity.Id} not found.");
			}
		}
	}

}
