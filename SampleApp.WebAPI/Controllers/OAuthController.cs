using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Models.Security;
using SampleApp.Core.Interfaces.Security;
using SampleApp.WebAPI.Infrastructure.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.WebAPI.Controllers
{
	[Route("api/[controller]")]
	public class OAuthController : ControllerBase
	{
		private readonly IOAuthService oauthService;

		public OAuthController(IOAuthService oauthService)
		{
			this.oauthService = oauthService;
		}

		[HttpPost]
		[Route("[action]")]
		[ExecuteInTransaction]
		public async Task<IActionResult> Authorize([FromBody] AuthorizePost model)
		{
			if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
			{
				return BadRequest("User and Password are required.");
			}

			var genericResponse = await oauthService.AuthorizePasswordAsync(model.UserName, model.Password);

			if (!genericResponse.Succeded)
			{
				return BadRequest(genericResponse.ToString());
			}

			var token = genericResponse.Models.SingleOrDefault();


			if (token == null)
			{
				return Unauthorized("Incorrect credentials");
			}

			var response = new AuthorizeResponse(token, "bearer");
			Response.Headers.Add("Cache-Control", "no-store");
			Response.Headers.Add("Pragma", "no-cache");
			return Ok(response);
		}

		[Authorize]
		[HttpPost("[action]")]
		public async Task<IActionResult> LogOut()
		{
			var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

			oauthService.RevokeAccessToken(token);

			await Task.CompletedTask;
			return Ok();
		}


		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenPost model)
		{
			var refreshResponse = await oauthService.RefreshTokenAsync(model.RefreshToken);
			if (!refreshResponse.Succeded)
			{
				return BadRequest(refreshResponse.ToString());
			}

			var token = refreshResponse.Models.FirstOrDefault();

			var response = new AuthorizeResponse(token, "bearer");

			Response.Headers.Add("Cache-Control", "no-store");
			Response.Headers.Add("Pragma", "no-cache");

			return Ok(response);
		}
	}

}
