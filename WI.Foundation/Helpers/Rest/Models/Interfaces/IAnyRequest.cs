using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WI.Foundation.Helpers.Rest.Models.Interfaces
{
	public interface IAnyRequest : IHttpPayloadRequest
    {
		/// <summary>
		/// Sends GET request asynchronously.
		/// </summary>
		Task<HttpResponseMessage> GetAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken));
		/// <summary>
		/// Sends GET request asynchronously and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> GetAsync<T>() where T : new();
		/// <summary>
		/// Sends GET request asynchronously with cancellation token and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> GetAsync<T>(CancellationToken cancellationToken) where T : new();
		
	    /// <summary>
	    /// Sends DELETE request asynchronously.
	    /// </summary>
	    Task<HttpResponseMessage> DeleteAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
		    CancellationToken ct = default(CancellationToken));
		/// <summary>
		/// Sends DELETE request asynchronously and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> DeleteAsync<T>() where T : new();
		/// <summary>
		/// Sends DELETE request asynchronously with cancellation token and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> DeleteAsync<T>(CancellationToken cancellationToken) where T : new();
	}
}
