using SampleApp.Core.Exceptions;
using System;
using System.Linq;
using System.Security.Claims;

namespace SampleApp.Core.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid DeserializeIdClaim(this ClaimsPrincipal claimsPrincipal)
		{
			var userIdString = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
			Guid.TryParse(userIdString, out Guid userId);
			if (userId.IsNullOrEmpty())
			{
				throw new BusinessException("User id is not provided in claims.");
			}
			return userId;
		}
	}

}
