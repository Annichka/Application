using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.WebAPI.Infrastructure.Attributes;

namespace SampleApp.WebAPI.Infrastructure.Configurations
{
	public static class ApplicationConfiguation
	{
		public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDistributedRedisCache(options =>
			{
				options.Configuration = configuration.GetConnectionString("Redis");
			});

			services.AddControllers(options =>
			{
				options.Filters.Add(typeof(ValidateModelStateAttribute));
			});

			services.Configure<ApiBehaviorOptions>(options =>
			{
				// This is needed, because the logic in ValidateModelStateAttribute 
				// won't be triggered for controllers marked with ApiControllerAttribute
				options.SuppressModelStateInvalidFilter = true;
			});

			services.AddSignalR();
		}
	}
}
