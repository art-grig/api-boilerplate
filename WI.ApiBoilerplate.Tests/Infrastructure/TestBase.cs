using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WI.ApiBoilerplate.Domain.Infrastructure;
using WI.ApiBoilerplate.Helpers;
using WI.ApiBoilerplate.ORM;
using WI.ApiBoilerplate.ORM.DI;
using WI.ApiBoilerplate.Services.DI;
using WI.Foundation.Helpers.Rest;
using WI.Foundation.Helpers.Rest.Interfaces;
using Xunit;

namespace WI.ApiBoilerplate.Tests.Infrastructure
{
	public abstract class TestBase : IAsyncLifetime, IDisposable
	{
		private static readonly Lazy<IServiceProvider> ContainerInitializer =
			new Lazy<IServiceProvider>(CreateContainer, LazyThreadSafetyMode.ExecutionAndPublication);

		private readonly IServiceScope _serviceScope;

		protected TestBase()
		{
			_serviceScope = ContainerInitializer.Value.CreateScope();
			DbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
			WebImpactLogger.LoggerFactory = new LoggerFactory();
		}

		protected ApplicationDbContext DbContext { get; }

		protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

		protected abstract Task SeedAsync();

		public async Task InitializeAsync()
		{
			if (DbContext.Database.IsSqlite())
			{
				DbContext.Database.OpenConnection();
				await DbContext.Database.EnsureCreatedAsync();
			}

			await SeedAsync();
		}

		public virtual async Task DisposeAsync()
		{
			if (DbContext.Database.IsSqlite())
			{
				DbContext.Database.CloseConnection();
				await DbContext.Database.EnsureDeletedAsync();
			}
		}

		public void Dispose()
		{
			_serviceScope?.Dispose();
		}

		private static IServiceCollection ConfigureServices(IServiceCollection services)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.local.json", optional: true)
				.Build();

			services.TryAddSingleton<IConfiguration>(configuration);

			services.AddMemoryCache();
			//services.ConfigureIdentity();
			services.AddLogging();

			var solutionProjects = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(a => a.GetName().Name.Contains(".Central"));
			services.AddAutoMapper(solutionProjects);
			
			services.AddScoped<IRestRequestBuilder, RestRequestBuilder>();

			//services.Configure<MapBoxSettings>(configuration.GetSection("MapBoxSettings"));

			var optionsBuilder = ConfigureOptionsBuilder(configuration);
			services.AddScoped(sp => new ApplicationDbContext(optionsBuilder.Options));
			services.AddTransient<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

			//services.Configure<S3FileStorageOptions>(configuration.GetSection("AWSS3"));
			//services.AddSingleton<IFileStorage, TestFileStorage>();

			services.RegisterDependencies(new RepositoryDIConfigurator());
			services.RegisterDependencies(new ServicesDIConfiguration());

			return services;
		}

		private static IServiceProvider CreateContainer()
		{
			var services = new ServiceCollection();

			// Register controllers.
			var partManager = new ApplicationPartManager();
			partManager.FeatureProviders.Add(new ControllerFeatureProvider());
			partManager.ApplicationParts.Add(new AssemblyPart(Assembly.Load("WI.ApiBoilerplate")));
			var controllersFeature = new ControllerFeature();
			partManager.PopulateFeature(controllersFeature);
			foreach (var controllerType in controllersFeature.Controllers.Select(t => t.AsType()))
			{
				services.TryAddTransient(controllerType);
			}

			ConfigureServices(services);

			var serviceProvider = services.BuildServiceProvider();

			// Validate automapper configuration.
			var mapper = serviceProvider.GetRequiredService<IMapper>();
			mapper.ConfigurationProvider.AssertConfigurationIsValid();

			return serviceProvider;
		}

		public static DbContextOptionsBuilder<ApplicationDbContext> ConfigureOptionsBuilder(
			IConfiguration configuration, ILoggerFactory loggerFactory = null)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.EnableSensitiveDataLogging();
			var connectionString = new SqliteConnectionStringBuilder { Mode = SqliteOpenMode.Memory }.ConnectionString;
			optionsBuilder.UseSqlite(connectionString);

			return optionsBuilder;
		}

		public async Task AddRangeAndSave<T>(IEnumerable<T> entities) where T : BaseEntity
		{
			await DbContext.AddRangeAsync(entities);
			await DbContext.SaveChangesAsync();
		}

		public async Task AddAndSave<T>(params T[] entities) where T : class
		{
			await DbContext.AddRangeAsync(entities);
			await DbContext.SaveChangesAsync();
		}

		public async Task RemoveAllAndSave<T>() where T : class
		{
			var items = await DbContext.Set<T>().ToListAsync();
			DbContext.RemoveRange(items);
			await DbContext.SaveChangesAsync();
		}
	}
}
