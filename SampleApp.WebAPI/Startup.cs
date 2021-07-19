using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using SampleApp.Core;
using SampleApp.WebAPI.Infrastructure.Configurations;
using SampleApp.WebAPI.Infrastructure.Middlewares;
using SimpleInjector;
using SwingersCRM.Persistence;
using System.Linq;

namespace SampleApp.WebAPI
{
	public class Startup
	{
		private readonly Container container;
		public Startup(IConfiguration configuration)
		{
			container = new Container();
			container.Options.ResolveUnregisteredConcreteTypes = false;
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureApplication(Configuration);
			services.ConfigureSimpleInjector(Configuration, container);
			services.ConfigureAuthentication(Configuration, container);
			services.ConfigureCors(Configuration, container);

			services.AddOpenApiDocument(document =>
			{
				document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
				{
					Type = OpenApiSecuritySchemeType.ApiKey,
					Name = "Authorization",
					In = OpenApiSecurityApiKeyLocation.Header,
					Description = "Type into the textbox: Bearer {your JWT token}."
				});

				document.OperationProcessors.Add(
					new AspNetCoreOperationSecurityScopeProcessor("JWT"));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseSimpleInjector(container);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.ConfigureExceptionMiddleware(env);
			InitializeContainer(app);
			app.UseOpenApi();
			app.UseSwaggerUi3();
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("Default");
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private void InitializeContainer(IApplicationBuilder app)
		{
			container.Verify();

			DbInitializer.Seed(ServiceLocator.container);
		}
	}
}
