using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WI.ApiBoilerplate.Helpers;
using WI.ApiBoilerplate.Swagger;

namespace WI.ApiBoilerplate.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void ConfigureApplicationInsights(this IServiceCollection services, string env)
		{
			services.AddApplicationInsightsTelemetry();
			services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>(x => new TelemetryInitializer(env));
		}

		public static void ConfigureApiVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
				options.AssumeDefaultVersionWhenUnspecified = true;
			});

			//var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
			//var wrapper = new ApiVersionDescriptionProviderWrapper
			//{
			//	ApiVersionDescriptionProvider = apiVersionDescriptionProvider
			//};

			//services.AddSingleton(wrapper);
		}

		public static IServiceCollection AddVersionedApiExplorer(this IServiceCollection serviceCollection,
			string groupNameFormat, bool substituteApiVersionInUrl, string substitutionFormat)
		{
			return serviceCollection.AddVersionedApiExplorer(options =>
			{
				options.GroupNameFormat = groupNameFormat;

				// note: this option is only necessary when versioning by url segment. the SubstitutionFormat
				// can also be used to control the format of the API version in route templates
				options.SubstituteApiVersionInUrl = substituteApiVersionInUrl;
				options.SubstitutionFormat = substitutionFormat;
			});
		}

		public static void ConfigureSwaggerGen(this IServiceCollection services,
			InfoApiVersionDto infoApiVersion, bool jwtToken = false, Action<SwaggerGenOptions> additionalSetup = null)
		{
			services.AddSwaggerGen(options =>
			{
				// resolve the IApiVersionDescriptionProvider service
				// note: that we have to build a temporary service provider here because one has not been created yet
				var provider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

				if (provider?.ApiVersionDescriptions != null)
				{
					// add a swagger document for each discovered API version
					// note: you might choose to skip or document deprecated API versions differently
					foreach (var description in provider.ApiVersionDescriptions)
					{
						var info = new Info
						{
							Title = string.Format(infoApiVersion.TitleTemplate, description.ApiVersion.ToString()),
							Version = string.Format(infoApiVersion.VersionTemplate, description.ApiVersion.ToString()),
							Description = infoApiVersion.Description,
							Contact = new Contact { Name = infoApiVersion.ContactName, Email = infoApiVersion.ContactEmail },
							TermsOfService = infoApiVersion.TermsOfService,
							License = new License { Name = infoApiVersion.LicenseName, Url = infoApiVersion.LicenseUrl }
						};

						if (description.IsDeprecated)
						{
							info.Description += infoApiVersion.DescriptionPartWhenDepricated;
						}

						options.SwaggerDoc(description.GroupName, info);
					}
				}

				options.DocumentFilter<SwaggerEnumDescriptionsFilter>();

				options.IncludeXmlComments(infoApiVersion.XmlCommentsFilePath);
				//options.DescribeAllEnumsAsStrings();
				//options.DescribeAllParametersInCamelCase();
				//options.DescribeStringEnumsInCamelCase();

				if (jwtToken)
				{
					options.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
					options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
						{ "Bearer", Enumerable.Empty<string>() },
					});
				}

				additionalSetup?.Invoke(options);
			});
		}
	}
}
