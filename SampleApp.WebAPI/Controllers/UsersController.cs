using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Entities.User;
using SampleApp.Core.Filter.User;
using SampleApp.Core.Interfaces.User;
using SampleApp.Core.Models;
using SampleApp.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleApp.WebAPI.Controllers
{
	[Authorize]
	public class UsersController : EntityRestController<UserEntity, UserModel, UserFilter>
	{
		public UsersController(IUserModelRepositoryService userModelRepositoryService) : base(userModelRepositoryService)
		{
		}

		public override async Task<ActionResult<UserModel>> Get([FromRoute] Guid id)
		{
			return await base.Get(id);
		}

		public override async Task<ActionResult<PageResult<UserModel>>> Get([FromQuery] UserFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
		{
			return await base.Get(filter, sorts, page);
		}

		public override async Task<ActionResult<UserModel>> Put([FromRoute] Guid id, [FromBody] UserModel model)
		{
			return await base.Put(id, model);
		}

		public override async Task<ActionResult<UserModel>> Delete([FromRoute] Guid id)
		{
			return await base.Delete(id);
		}
	}
}
