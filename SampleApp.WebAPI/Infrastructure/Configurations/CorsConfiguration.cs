using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace SampleApp.WebAPI.Infrastructure.Configurations
{
	public static class CorsConfiguration
	{
		public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration, Container container)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("Default",
					builder =>
					{
						builder
							.WithOrigins()
							.AllowAnyMethod()
							.AllowAnyHeader()
							.AllowCredentials();
					});
			});
		}
	}
}
