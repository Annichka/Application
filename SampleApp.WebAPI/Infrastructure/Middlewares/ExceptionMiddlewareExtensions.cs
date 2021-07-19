using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace SampleApp.WebAPI.Infrastructure.Middlewares
{
	public static class ExceptionMiddlewareExtensions
	{
		public static void ConfigureExceptionMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseMiddleware<ExceptionMiddleware>(env);
		}
	}
}
