using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using WI.Foundation.Exceptions;
using WI.Foundation.Helpers;
using WI.Foundation.ViewModels;

namespace WI.ApiBoilerplate.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;
		private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.None,
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		public GlobalExceptionHandler(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext, ILogger<GlobalExceptionHandler> logger)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				var telemetryClient = new TelemetryClient();
				if (ex is WebImpactBaseException webImpactBaseException)
				{
					var props = new Dictionary<string, string>
					{
						{"StackTrace", webImpactBaseException.StackTrace},
						{"Message", webImpactBaseException.Message},
						{"WebImpactDebugInfo", webImpactBaseException.SystemMessage},
					};
					telemetryClient.TrackEvent(webImpactBaseException.GetType().Name, props);
				}
				else
				{
					telemetryClient.TrackException(ex);
				}
				telemetryClient.Flush();

				var absoluteUri = httpContext.Request == null ? "n/a" : string.Concat(
					httpContext.Request.Scheme,
					"://",
					httpContext.Request.Host.ToUriComponent(),
					httpContext.Request.PathBase.ToUriComponent(),
					httpContext.Request.Path.ToUriComponent(),
					httpContext.Request.QueryString.ToUriComponent());
					
				logger.LogError(ex, absoluteUri);

				if (httpContext.Response.HasStarted)
				{
					throw;
				}

				if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
					// TODO: should we set inner exception to real one?
					ex = new WebImpactNotAuthenticatedException("Unauthorized.");

				await WriteExceptionToResponse(ex, httpContext);
			}
		}

		private async Task WriteExceptionToResponse(Exception ex, HttpContext httpContext)
		{
			var responseVM = new BaseResponseVm(ex);

			if (ex is WebImpactExternalResourceException externalException)
			{
				if (externalException.WebImpactDebugInfo is BaseResponseVm webImpactResponse)
				{
					responseVM.SystemErrorMessage += "/////" + webImpactResponse.SystemErrorMessage;
					responseVM.SystemErrorStack += "/////" + webImpactResponse.SystemErrorStack;
					responseVM.SystemMessage += "/////" + webImpactResponse.SystemMessage;
					responseVM.Result = webImpactResponse.Result;
				}
				else
				{
					responseVM.SystemMessage = JsonConvert.SerializeObject(externalException.WebImpactDebugInfo, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
				}

				responseVM.DebugInfo = new
				{
					Request = externalException.RequestData,
					Response = externalException.WebImpactDebugInfo
				};
			}

			// httpContext.Response.Clear(); // Do not clear headers to support CORS

			var calculatedStatusCode = StatusCodeHelper.GetIntStatusCodeForResponse(responseVM);
			if (httpContext.Response.StatusCode != calculatedStatusCode)
				httpContext.Response.StatusCode = calculatedStatusCode;

			httpContext.Response.ContentType = "application/json";

			var response = JsonConvert.SerializeObject(responseVM, _jsonSettings);

			await httpContext.Response.WriteAsync(response);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class GlobalExceptionHandlerExtensions
	{
		public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<GlobalExceptionHandler>();
		}
	}
}
