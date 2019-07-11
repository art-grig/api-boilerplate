using System;
using System.Diagnostics;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using WI.ApiBoilerplate.Extensions;
using WI.ApiBoilerplate.Helpers;
using WI.ApiBoilerplate.Middleware;
using WI.ApiBoilerplate.Middleware.Filters;
using WI.ApiBoilerplate.ORM;
using WI.ApiBoilerplate.ORM.DI;
using WI.ApiBoilerplate.Services.DI;
using WI.ApiBoilerplate.Swagger;

[assembly: ApiController]
namespace WI.ApiBoilerplate
{
	public class Startup
	{
		private readonly ILoggerFactory _loggerFactory;

		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			Configuration = configuration;
			_loggerFactory = loggerFactory;
			WebImpactLogger.LoggerFactory = loggerFactory;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureApplicationInsights(Configuration["ApplicationInsights:Environment"]);

			services.AddCors();
			services.AddMemoryCache();

			services.AddDbContextPool<ApplicationDbContext>(options =>
			{
				options.EnableSensitiveDataLogging();
				options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));

				if (Debugger.IsAttached)
					options.UseLoggerFactory(_loggerFactory);
			});

			services.AddMvcCore()
				.SetCompatibilityVersion(CompatibilityVersion.Latest);
			services.AddVersionedApiExplorer("'v'VVV", true, "VVV");

			services.AddMvc(options =>
			{
				options.Filters.Add<OperationCancelledFilter>();
				options.Filters.Add<EnableRewindAuthorizationFilter>();
				options.AllowEmptyInputInBodyModelBinding = true;
			})
			.AddControllersAsServices();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.ConfigureApiVersioning();

			//services.ConfigureHealthCheck();

			services.AddAutoMapper();

			services.ConfigureSwaggerGen(new InfoApiVersionDto
			{
				TitleTemplate = "WebImpact AppName API {0}",
				VersionTemplate = "{0}",
				Description = "WebImpact AppName server",
				ContactName = "Some Guy",
				ContactEmail = "some.guy@somewhere.com",
				TermsOfService = "Shareware",
				LicenseName = "MIT",
				LicenseUrl = "https://opensource.org/licenses/MIT",
				DescriptionPartWhenDepricated = " This API version has been deprecated.",
				XmlCommentsFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
					$"{PlatformServices.Default.Application.ApplicationName}.xml")
			});

			services.RegisterDependencies(new RepositoryDIConfigurator());
			services.RegisterDependencies(new ServicesDIConfiguration());
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, AutoMapper.IConfigurationProvider autoMapper)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			
			app.UseGlobalExceptionHandler();

			app.UseSwagger();
			app.UseSwaggerUI( c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "WI.ApiBoilerplate V1");
			});

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());

			app.UseMvc();

			autoMapper.AssertConfigurationIsValid();
		}
	}

	//TODO: move to foundation
	public static class ServiceCollectionExtensions
	{
		public static void RegisterDependencies<T>(this IServiceCollection services, T configurator) where T : IDependencyInjectionConfigurator
		{
			configurator.RegisterServices(SimpleRegister);

			void SimpleRegister(Type inter, Type impl)
			{
				services.AddScoped(inter, impl);
			}
		}
	}
}
