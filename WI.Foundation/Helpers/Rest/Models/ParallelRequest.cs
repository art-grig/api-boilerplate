using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WI.Foundation.Helpers.Rest.Models.Interfaces;

namespace WI.Foundation.Helpers.Rest.Models
{
	public class ParallelRequest : IParallelRequest
	{
		protected IEnumerable<ExternalHttpRequest> _requests;

		public ParallelRequest(IEnumerable<IAnyRequest> requests)
		{
			_requests = requests.Cast<ExternalHttpRequest>();
		}

		public ParallelRequest(IEnumerable<IHttpPayloadRequest> requests)
		{
			_requests = requests.Cast<ExternalHttpRequest>();
		}

		public Task<HttpResponseMessage[]> DeleteAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return Task.WhenAll(_requests.Select(x => x.DeleteAsync(completionOption, ct)));
		}

		public Task<T[]> DeleteAsync<T>() where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.DeleteAsync<T>()));
		}

		public Task<T[]> DeleteAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.DeleteAsync<T>(cancellationToken)));
		}

		public Task<HttpResponseMessage[]> GetAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return Task.WhenAll(_requests.Select(x => x.GetAsync(completionOption, ct)));
		}

		public Task<T[]> GetAsync<T>() where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.GetAsync<T>()));
		}

		public Task<T[]> GetAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.GetAsync<T>(cancellationToken)));
		}

		public Task<HttpResponseMessage[]> PostAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return Task.WhenAll(_requests.Select(x => x.PostAsync(completionOption, ct)));
		}

		public Task<T[]> PostAsync<T>() where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.PostAsync<T>()));
		}

		public Task<T[]> PostAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.PostAsync<T>(cancellationToken)));
		}

		public Task<HttpResponseMessage[]> PutAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return Task.WhenAll(_requests.Select(x => x.PutAsync(completionOption, ct)));
		}

		public Task<T[]> PutAsync<T>() where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.PutAsync<T>()));
		}

		public Task<T[]> PutAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			return Task.WhenAll(_requests.Select(x => x.PutAsync<T>(cancellationToken)));
		}

		public IParallelRequest WithBearer(string jwt)
		{
			_requests = _requests.Select(x => x.WithBearer(jwt) as ExternalHttpRequest);
			return this;
		}

		public IHttpPayloadParallelRequest WithFormParameter(string name, object value)
		{
			_requests = _requests.Select(x => x.WithFormParameter(name, value) as ExternalHttpRequest);
			return this;
		}

		public IHttpPayloadParallelRequest WithFormParameters(object model)
		{
			_requests = _requests.Select(x => x.WithFormParameters(model) as ExternalHttpRequest);
			return this;
		}

		public IParallelRequest WithHeader(string name, string value)
		{
			_requests = _requests.Select(x => x.WithHeader(name, value) as ExternalHttpRequest);
			return this;
		}

		public IHttpPayloadParallelRequest WithPayload(object payload)
		{
			_requests = _requests.Select(x => x.WithPayload(payload) as ExternalHttpRequest);
			return this;
		}

		public IParallelRequest WithQueryParameter(string name, object value)
		{
			_requests = _requests.Select(x => x.WithQueryParameter(name, value) as ExternalHttpRequest);
			return this;
		}

		public IParallelRequest WithQueryParameters(object model)
		{
			_requests = _requests.Select(x => x.WithQueryParameters(model) as ExternalHttpRequest);
			return this;
		}

		public IHttpPayloadParallelRequest WithSerializedBody(string body)
		{
			_requests = _requests.Select(x => x.WithSerializedBody(body) as ExternalHttpRequest);
			return this;
		}
	}
}
