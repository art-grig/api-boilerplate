using System;
using System.Net.Http;
using WI.Foundation.Helpers.Rest.Interfaces;
using WI.Foundation.Helpers.Rest.Models;
using WI.Foundation.Helpers.Rest.Models.Interfaces;

namespace WI.Foundation.Helpers.Rest
{
	public class RestRequestBuilder : IRestRequestBuilder
    {
		protected HttpClient _client = null;
		protected Func<HttpClient> _httpClientFactory = () => new HttpClient();

		public RestRequestBuilder()
		{ }

		/// <summary>
		/// Accepts HttpClient factory delegate what allows to mock http requests made through this builder
		/// </summary>
		public RestRequestBuilder(Func<HttpClient> httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		/// <inheritdoc />
		public IHttpRequest WithSharedClient()
		{
			if (_client == null)
				_client = _httpClientFactory();

			var request = new ExternalHttpRequest(_client);
			return request;
		}

		/// <inheritdoc />
		public IHttpRequest WithSharedClient(string baseAddress)
		{
			if (_client == null)
				_client = _httpClientFactory();

			var request = new ExternalHttpRequest(_client, baseAddress);

			return request;
		}

		/// <inheritdoc />
		public IHttpRequest WithIsolatedSettings()
		{
			return new ExternalHttpRequest(_httpClientFactory());
		}

		/// <inheritdoc />
		public IHttpRequest WithIsolatedSettings(string baseAddress)
		{
			return new ExternalHttpRequest(_httpClientFactory(), baseAddress);
		}
	}
}
