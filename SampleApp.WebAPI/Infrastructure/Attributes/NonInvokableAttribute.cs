using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SampleApp.WebAPI.Infrastructure.Attributes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class NonInvokableAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			throw new MethodAccessException();
		}
	}
}
