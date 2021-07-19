using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Constants;
using SampleApp.Core.Exceptions;
using SampleApp.Core.Extensions;
using SampleApp.Core.Interfaces;
using SampleApp.Core.Models;
using SampleApp.WebAPI.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApp.WebAPI.Controllers
{
	[ApiController]
	[Route(RoutingConstants.ApiController)]
	public abstract class EntityRestController<TEntity, TModel, TFilter> : ControllerBase
		where TEntity : class
		where TModel : ModelBase
		where TFilter : class, IEntityFilter
	{
		private readonly IModelRepositoryService<TEntity, TModel, TFilter> modelRepositoryService;

		public EntityRestController(IModelRepositoryService<TEntity, TModel, TFilter> modelRepositoryService)
		{
			this.modelRepositoryService = modelRepositoryService;
		}

		[NonInvokable]
		[HttpGet(RoutingConstants.BaseFragmentId)]
		public virtual async Task<ActionResult<TModel>> Get([FromRoute] Guid id)
		{
			return await modelRepositoryService.GetAsync(id);
		}

		[NonInvokable]
		[HttpGet]
		public virtual async Task<ActionResult<PageResult<TModel>>> Get([FromQuery] TFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
		{
			return await modelRepositoryService.ReadAsync(filter, sorts, page);
		}

		[NonInvokable]
		[HttpGet("count")]
		public virtual async Task<ActionResult<int>> Get([FromQuery] TFilter filter)
		{
			return await modelRepositoryService.EntityRepositoryService.CountAsync(filter);
		}

		[NonInvokable]
		[HttpPost]
		[ExecuteInTransaction]
		public virtual async Task<ActionResult<TModel>> Post([FromBody] TModel model)
		{
			return await modelRepositoryService.CreateAsync(model);
		}

		[NonInvokable]
		[HttpPut(RoutingConstants.BaseFragmentId)]
		[ExecuteInTransaction]
		public virtual async Task<ActionResult<TModel>> Put([FromRoute] Guid id, [FromBody] TModel model)
		{
			if (id.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			if (model.Id.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(model.Id));
			}
			if (id != model.Id)
			{
				throw new InvalidModelStateException("Paremeters id and model id should be the same");
			}

			return await modelRepositoryService.UpdateAsync(model);
		}

		[NonInvokable]
		[HttpDelete(RoutingConstants.BaseFragmentId)]
		[ExecuteInTransaction]
		public virtual async Task<ActionResult<TModel>> Delete([FromRoute] Guid id)
		{
			return await modelRepositoryService.DeleteAsync(id);
		}
	}

}
