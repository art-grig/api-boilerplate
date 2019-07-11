using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.Helpers
{
	public class TelemetryInitializer : ITelemetryInitializer
	{
		private readonly string _env;

		public TelemetryInitializer(string env)
		{
			_env = env;
		}

		/// <summary>
		/// Initializes properties of the specified 
		/// <see cref="T:Microsoft.ApplicationInsights.Channel.ITelemetry" /> object.
		/// </summary>
		/// <param name="telemetry">the telemetry channel</param>
		public void Initialize(ITelemetry telemetry)
		{
			telemetry.Context.Properties["Environment"] = _env;
		}
	}
}
