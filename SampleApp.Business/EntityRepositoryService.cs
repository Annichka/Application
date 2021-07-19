using Microsoft.EntityFrameworkCore;
using SampleApp.Core;
using SampleApp.Core.Entities.Base;
using SampleApp.Core.Enums;
using SampleApp.Core.Extensions;
using SampleApp.Core.Interfaces;
using SampleApp.Core.Models;
using SampleApp.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SampleApp.Business
{
	public abstract class EntityRepositoryService<TEntity, TFilter, TRepository>
		where TEntity : class, IEntityBase, new()
		where TFilter : class, IEntityFilter, new()
		where TRepository : IRepository<TEntity>
	{
		private readonly TRepository repository;
		private readonly IDbTransaction dbTransaction;

		public EntityRepositoryService(TRepository repository)
		{
			this.repository = repository;
			dbTransaction = ServiceLocator.container.GetInstance<IDbTransaction>();
		}

		public virtual async Task<TEntity> GetAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			if (id.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(id));
			}

			await FindingAsync(id);

			var query = (IQueryable<TEntity>)dbTransaction.DbContext.Set<TEntity>();

			if (getAssociations != null)
			{
				query = getAssociations(query);
			}

			// Temp solution
			var entity = await query.SingleOrDefaultAsync(GetMatchById(id));

			_ = entity ?? throw new ArgumentNullException(
				$"Entity of type {typeof(TEntity).Name} with id {id} was not found.");

			await FoundAsync(entity);

			return entity;
		}

		public async Task<TEntity> GetFirstAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var page = new Page()
			{
				PageSize = 1
			};

			var entities = await ReadAsync(filter, sorts, page, getAssociations);

			return entities.Items.Single();
		}

		public async Task<TEntity> GetFirstOrDefaultAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var page = new Page()
			{
				PageSize = 1
			};

			var entities = await ReadAsync(filter, sorts, page, getAssociations);

			return entities.Items.SingleOrDefault();
		}

		public async Task<TEntity> GetUniqueAsync(TFilter filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var page = new Page()
			{
				PageSize = 2
			};

			var entities = await ReadAsync(filter, null, page, getAssociations);

			return entities.Items.Single();
		}

		public async Task<TEntity> GetUniqueOrDefaultAsync(TFilter filter = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var page = new Page()
			{
				PageSize = 2
			};

			var entities = await ReadAsync(filter, null, page, getAssociations);

			return entities.Items.SingleOrDefault();
		}

		public async Task<PageResult<TEntity>> ReadAsync(TFilter filter = null, IEnumerable<Sort> sorts = null, Page page = null, Func<IQueryable<TEntity>, IQueryable<TEntity>> getAssociations = null)
		{
			var result = new PageResult<TEntity>();

			if (filter == null)
			{
				filter = new TFilter();
			}

			if (sorts == null)
			{
				sorts = new List<Sort>();
			}
			if (sorts.Any(x => string.IsNullOrWhiteSpace(x.Field)))
			{
				throw new ArgumentException("A sorting field is either null or empty.", nameof(sorts));
			}

			if (page == null)
			{
				page = new Page();
			}
			if (!page.PageNumber.HasValue)
			{
				page.PageNumber = 1;
			}
			if (!page.PageSize.HasValue)
			{
				page.PageSize = int.MaxValue;
			}

			var suppressQuery = false;
			var filterExpression = GetMatchByFilter(filter, ref suppressQuery);

			if (!suppressQuery)
			{
				var pageResult = await repository.FindAllPagedAsync(
					filterExpression,
					page.PageNumber.Value,
					page.PageSize.Value,
					GetOrderFields(sorts),
					getAssociations);

				result.Items = pageResult.items;
				result.TotalCount = pageResult.count;
			}

			return result;
		}

		public async Task<int> CountAsync(TFilter filter)
		{
			var result = 0;

			if (filter == null)
			{
				filter = new TFilter();
			}

			var suppressQuery = false;
			var filterExpression = GetMatchByFilter(filter, ref suppressQuery);

			if (!suppressQuery)
			{
				result = await repository.CountAsync(filterExpression);
			}

			return result;
		}

		public async Task<bool> ExistsAsync(TFilter filter)
		{
			var result = false;

			if (filter == null)
			{
				filter = new TFilter();
			}

			var suppressQuery = false;
			var filterExpression = GetMatchByFilter(filter, ref suppressQuery);

			if (!suppressQuery)
			{
				result = await dbTransaction.DbContext.Set<TEntity>().AnyAsync(filterExpression);
			}

			return result;
		}

		public virtual async Task SaveAsync(TEntity entity, IDictionary<string, object> additionalParameters = null)
		{
			if (additionalParameters == null)
			{
				additionalParameters = new Dictionary<string, object>();
			}

			TEntity originalEntity = GetOriginalEntity(entity, additionalParameters);

			await ValidateAsync(entity, originalEntity, additionalParameters);

			bool suppressSave = false;
			await SavingAsync(entity, originalEntity, suppressSave, additionalParameters);

			if (!suppressSave)
			{
				if (entity.IsNew())
				{
					entity = await repository.AddAsync(entity);
				}
				else
				{
					repository.Update(entity);
				}

				await repository.SaveAsync();

				await SavedAsync(entity, originalEntity, additionalParameters);
			}
		}

		public virtual async Task<TEntity> DeleteAsync(TEntity entity, IDictionary<string, object> additionalParameters = null)
		{
			if (additionalParameters == null)
			{
				additionalParameters = new Dictionary<string, object>();
			}

			await DeletingAsync(entity, additionalParameters);

			await repository.DeleteAsync(entity);
			await repository.SaveAsync();

			await DeletedAsync(entity, additionalParameters);

			return entity;
		}

		protected virtual TEntity GetOriginalEntity(TEntity entity, IDictionary<string, object> additionalParameters)
		{
			return (TEntity)dbTransaction.DbContext.Entry(entity).OriginalValues.ToObject();
		}

		protected virtual async Task ValidateAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters)
		{
			var validationContext = new ValidationContext(entity);
			Validator.ValidateObject(entity, validationContext, true);
			await Task.CompletedTask;
		}

		protected virtual async Task FindingAsync(Guid id)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task FoundAsync(TEntity entity)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task SavingAsync(TEntity entity, TEntity originalEntity, bool suppressSave, IDictionary<string, object> additionalParameters)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task SavedAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task DeletingAsync(TEntity entity, IDictionary<string, object> additionalParameters)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task DeletedAsync(TEntity entity, IDictionary<string, object> additionalParameters)
		{
			await Task.CompletedTask;
		}

		protected virtual Expression<Func<TEntity, bool>> GetMatchById(Guid id)
		{
			return x => x.Id == id;
		}

		protected virtual Expression<Func<TEntity, bool>> GetMatchByFilter(TFilter filter, ref bool suppressQuery)
		{
			return x => true;
		}

		protected virtual List<(Expression<Func<TEntity, object>>, SortDirection)> GetOrderFields(IEnumerable<Sort> sorts)
		{
			return new List<(Expression<Func<TEntity, object>>, SortDirection)>();
		}
	}

}
