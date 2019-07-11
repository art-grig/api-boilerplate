using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.Helpers
{
	/// <summary>
	/// Shared logger
	/// </summary>
	public static class WebImpactLogger
	{
		public static ILoggerFactory LoggerFactory { get; set; }
		public static ILogger CreateLogger() => LoggerFactory?.CreateLogger(typeof(WebImpactLogger)) ?? throw new Exception("LoggerFactory is not initialized.");
	}
}
