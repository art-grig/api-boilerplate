using WI.Foundation.Helpers.Rest.Models.Interfaces;

namespace WI.Foundation.Helpers.Rest.Interfaces
{
	public interface IRestRequestBuilder
    {
		/// <summary>
		/// Allowes to use same headers, including Authorization, between requests prepared with this instance of builder
		/// </summary>
		IHttpRequest WithSharedClient();
		/// <summary>
		/// Allowes to use same headers, including Authorization, between requests prepared with this instance of builder
		/// </summary>
		IHttpRequest WithSharedClient(string baseAddress);
		/// <summary>
		/// Prepares an isolated request that doesn't depend on headers set in other requests created through this builder
		/// </summary>
		IHttpRequest WithIsolatedSettings();
		/// <summary>
		/// Prepares an isolated request that doesn't depend on headers set in other requests created through this builder
		/// </summary>
		IHttpRequest WithIsolatedSettings(string baseAddress);
	}
}
