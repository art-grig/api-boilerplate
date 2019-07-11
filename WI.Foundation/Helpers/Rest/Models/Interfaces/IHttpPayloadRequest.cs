using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WI.Foundation.Helpers.Rest.Models.Interfaces
{
	public interface IHttpPayloadRequest
    {
		/// <summary>
		/// Adds HTTP header to this request and all requests made with that client if it is shared.
		/// </summary>
		IAnyRequest WithHeader(string name, string value);
		/// <summary>
		/// Adds HTTP header "Authorization: Bearer {jwt}" to this requests and all shared requests.
		/// </summary>
		IAnyRequest WithBearer(string jwt);
		/// <summary>
		/// Adds GET parameter to that request.
		/// </summary>
		IAnyRequest WithQueryParameter(string name, object value);
		/// <summary>
		/// Adds GET parameters from the properties of that model.
		/// </summary>
		IAnyRequest WithQueryParameters(object model);

		/// <summary>
		/// Adds JSON body serialized from the payload parameter.
		/// Only PUT and POST requests are available after that.
		/// </summary>
		IHttpPayloadRequest WithPayload(object payload, JsonSerializerSettings settings = null);
		/// <summary>
		/// Adds body from pre-serialized payload parameter.
		/// Only PUT and POST requests are available after that.
		/// </summary>
		IHttpPayloadRequest WithSerializedBody(string body);
		/// <summary>
		/// Adds Form parameter (FormUrlEncodedContent).
		/// Only PUT and POST requests are available after that.
		/// </summary>
		IHttpPayloadRequest WithFormParameter(string name, object value);
		/// <summary>
		/// Adds all properties and values from the model as form parameters (FormUrlEncoded).
		/// Only PUT and POST requests are available after that.
		/// </summary>
		IHttpPayloadRequest WithFormParameters(object model);
		
	    /// <summary>
	    /// Sends POST request asynchronously.
	    /// </summary>
	    Task<HttpResponseMessage> PostAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
		    CancellationToken ct = default(CancellationToken));
		/// <summary>
		/// Sends POST request asynchronously and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> PostAsync<T>() where T : new();
		/// <summary>
		/// Sends POST request asynchronously with cancellation token and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> PostAsync<T>(CancellationToken cancellationToken) where T : new();
		
	    /// <summary>
	    /// Sends PUT request asynchronously.
	    /// </summary>
	    Task<HttpResponseMessage> PutAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
		    CancellationToken ct = default(CancellationToken));
		/// <summary>
		/// Sends PUT request asynchronously and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> PutAsync<T>() where T : new();
		/// <summary>
		/// Sends PUT request asynchronously with cancellation token and deserializes the result JSON to the type T.
		/// </summary>
		Task<T> PutAsync<T>(CancellationToken cancellationToken) where T : new();


		IParallelRequest InParallel(Func<IAnyRequest, IEnumerable<IAnyRequest>> multiplicator);
		IHttpPayloadParallelRequest InParallel(Func<IHttpPayloadRequest, IEnumerable<IHttpPayloadRequest>> multiplicator);
		IHttpPayloadParallelRequest InParallel(Func<IAnyRequest, IEnumerable<IHttpPayloadRequest>> multiplicator);
	}
}
