using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.Middleware.Filters
{
	public class OperationCancelledFilter : IAsyncExceptionFilter
	{
		private readonly ILogger<OperationCancelledFilter> _logger;

		public OperationCancelledFilter(ILogger<OperationCancelledFilter> logger)
		{
			_logger = logger;
		}

		public Task OnExceptionAsync(ExceptionContext context)
		{
			if (context.Exception is OperationCanceledException)
			{
				_logger.LogInformation("Request was cancelled");
				context.ExceptionHandled = true;
				context.Result = new StatusCodeResult(499);
			}

			return Task.CompletedTask;
		}
	}
}
