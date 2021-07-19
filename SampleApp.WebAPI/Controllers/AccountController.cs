using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Constants;
using SampleApp.Core.Interfaces.Account;
using SampleApp.Core.Models.Account;
using SampleApp.WebAPI.Infrastructure.Attributes;
using System.Threading.Tasks;

namespace SampleApp.WebAPI.Controllers
{
	[Route(RoutingConstants.ApiController)]
	public class AccountController : ControllerBase
	{
		private readonly IAccountService accountService;

		public AccountController(IAccountService accountService)
		{
			this.accountService = accountService;
		}

		[HttpPost]
		[Route(RoutingConstants.ActionFragment)]
		[ExecuteInTransaction]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var response = await accountService.RegisterAsync(request);
			if (!response.Succeded)
			{
				return BadRequest(response.ToString());
			}

			return Ok();
		}


	}

}
