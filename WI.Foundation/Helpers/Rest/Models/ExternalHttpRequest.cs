using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using WI.Foundation.Exceptions;
using WI.Foundation.Helpers.Rest.Models.Interfaces;
using WI.Foundation.ViewModels;

namespace WI.Foundation.Helpers.Rest.Models
{
	public class ExternalHttpRequest : IHttpRequest, IAnyRequest, IHttpPayloadRequest
	{
		private readonly HttpClient _client;

		protected string _baseUrl;
		protected string _endpoint = "/";
		protected List<KeyValuePair<string, string>> _headersData;
		protected List<KeyValuePair<string, string>> _formData;
		protected List<KeyValuePair<string, string>> _queryData;
		protected string _contentData;
		protected bool _successOnly = true;

		public ExternalHttpRequest()
		{
		}

		public ExternalHttpRequest(string baseUrl) : this()
		{
			_baseUrl = baseUrl;
		}

		public ExternalHttpRequest(HttpClient client)
		{
			_client = client ?? new HttpClient();
		}

		public ExternalHttpRequest(HttpClient client, string baseUrl)
		{
			_client = client ?? new HttpClient();
			_baseUrl = baseUrl;
		}

		#region Protected properties

		protected ExternalHttpRequest CloneAndModify(Action<ExternalHttpRequest> modifyAction)
		{
			var copy = new ExternalHttpRequest(this._client)
			{
				_baseUrl = this._baseUrl,
				_endpoint = this._endpoint,
				_headersData = this._headersData != null ? new List<KeyValuePair<string, string>>(this._headersData) : null,
				_formData = this._formData != null ? new List<KeyValuePair<string, string>>(this._formData) : null,
				_queryData = this._queryData != null ? new List<KeyValuePair<string, string>>(this._queryData) : null,
				_contentData = this._contentData,
				_successOnly = this._successOnly
			};

			modifyAction(copy);

			return copy;
		}

		protected List<KeyValuePair<string, string>> HeadersData
			=> (_headersData ?? (_headersData = new List<KeyValuePair<string, string>>()));

		protected List<KeyValuePair<string, string>> FormData 
			=> (_formData ?? (_formData = new List<KeyValuePair<string, string>>()));

		protected List<KeyValuePair<string, string>> QueryData
			=> (_queryData ?? (_queryData = new List<KeyValuePair<string, string>>()));

		#endregion

		#region Base methods

		/// <inheritdoc />
		public virtual IAnyRequest From(string relativeAddress)
		{
			return CloneAndModify(x =>
			{
				x._endpoint = relativeAddress;
			});
		}

		/// <inheritdoc />
		public virtual IAnyRequest To(string relativeAddress)
			=> From(relativeAddress);

		#endregion

		#region Adding information to the request

		/// <inheritdoc />
		public virtual IAnyRequest WithHeader(string name, string value)
		{
			return CloneAndModify(x =>
			{
				x.HeadersData.Add(new KeyValuePair<string, string>(name, value));
			});
		}

		/// <inheritdoc />
		public virtual IAnyRequest WithBearer(string jwt)
		{
			return CloneAndModify(x =>
			{
				x.HeadersData.Add(new KeyValuePair<string, string>("Authorization", $"Bearer {jwt}"));
			});
		}

		/// <inheritdoc />
		public virtual IAnyRequest WithQueryParameter(string name, object value)
		{
			return CloneAndModify(x =>
			{
				x.QueryData.Add(new KeyValuePair<string, string>(name, value.ToString()));
			});
		}

		/// <inheritdoc />
		public virtual IAnyRequest WithQueryParameters(object model)
		{
			return CloneAndModify(x =>
			{
				x.QueryData.AddRange(ParseObjectToParams(model));
			});
		}

		/// <inheritdoc />
		public virtual IHttpPayloadRequest WithPayload(object payload, JsonSerializerSettings settings = null)
		{
			return CloneAndModify(x =>
			{
				if (settings == null)
				{
					settings = new JsonSerializerSettings
					{
						ContractResolver = new CamelCasePropertyNamesContractResolver(),
					};
				}

				var json = JsonConvert.SerializeObject(payload, settings);

				x._contentData = json;
			});
		}

		/// <inheritdoc />
		public virtual IHttpPayloadRequest WithSerializedBody(string body)
		{
			return CloneAndModify(x =>
			{
				x._contentData = body;
			});
		}

		/// <inheritdoc />
		public virtual IHttpPayloadRequest WithFormParameter(string name, object value)
		{
			return CloneAndModify(x =>
			{
				x.FormData.Add(new KeyValuePair<string, string>(name, value.ToString()));
			});
		}

		/// <inheritdoc />
		public virtual IHttpPayloadRequest WithFormParameters(object model)
		{
			return CloneAndModify(x =>
			{
				x.FormData.AddRange(ParseObjectToParams(model));
			});
		}

		protected async Task<HttpResponseMessage> SendAsync(HttpMethod method,
			CancellationToken cancellationToken, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
		{
			var urlString = (_baseUrl?.TrimEnd('/') ?? string.Empty) + (_endpoint ?? string.Empty) + GetQueryParameters();

			var httpRequestMessage = new HttpRequestMessage
			{
				Method = method,
				RequestUri = new Uri(urlString),
			};

			foreach (var header in HeadersData)
			{
				httpRequestMessage.Headers.Add(header.Key, header.Value);
			}

			HttpResponseMessage response;

			switch (method.ToString().ToUpper())
			{
				case "GET":
					response = await _client.SendAsync(httpRequestMessage, completionOption, cancellationToken);
					break;

				case "PUT":
				case "POST":
				case "DELETE":
					if (!string.IsNullOrEmpty(_contentData))
						httpRequestMessage.Content = new StringContent(_contentData, Encoding.UTF8, "application/json");
					else
					if (_formData != null)
						httpRequestMessage.Content = new FormUrlEncodedContent(_formData);

					response = await _client.SendAsync(httpRequestMessage, completionOption, cancellationToken);
					break;

				default:
					throw new WebImpactInvalidOperationException($"{method} is not implemented");
			}

			return response;
		}

		#endregion

		#region Parameters

		/// <inheritdoc />
		public virtual IHttpRequest ForAnyResponseStatus()
		{
			return CloneAndModify(x =>
			{
				x._successOnly = false;
			});
		}

		/// <inheritdoc />
		public virtual HttpClient ShareClient() => _client;

		#endregion

		#region Parallelize

		public IParallelRequest InParallel(Func<IHttpRequest, IEnumerable<IAnyRequest>> multiplicator)
		{
			return new ParallelRequest(multiplicator(this));
		}

		public IParallelRequest InParallel(Func<IHttpRequest, IEnumerable<IHttpPayloadRequest>> multiplicator)
		{
			return new ParallelRequest(multiplicator(this));
		}

		public IParallelRequest InParallel(Func<IAnyRequest, IEnumerable<IAnyRequest>> multiplicator)
		{
			return new ParallelRequest(multiplicator(this));
		}

		public IHttpPayloadParallelRequest InParallel(Func<IHttpPayloadRequest, IEnumerable<IHttpPayloadRequest>> multiplicator)
		{
			return new ParallelRequest(multiplicator(this));
		}

		public IHttpPayloadParallelRequest InParallel(Func<IAnyRequest, IEnumerable<IHttpPayloadRequest>> multiplicator)
		{
			return new ParallelRequest(multiplicator(this));
		}

		#endregion

		#region Execute and materialize

		/// <inheritdoc />
		public Task<HttpResponseMessage> GetAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return SendAsync(HttpMethod.Get, ct, completionOption);
		}

		/// <inheritdoc />
		public Task<T> GetAsync<T>() where T : new()
			=> GetAsync<T>(CancellationToken.None);

		/// <inheritdoc />
		public async Task<T> GetAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			var response = await GetAsync(ct: cancellationToken);
			var result = await ParseResponseMessage<T>(response);
			return result;
		}

		/// <inheritdoc />
		public Task<HttpResponseMessage> PostAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return SendAsync(HttpMethod.Post, ct, completionOption);
		}

		/// <inheritdoc />
		public Task<T> PostAsync<T>() where T : new()
			=> PostAsync<T>(CancellationToken.None);

		/// <inheritdoc />
		public async Task<T> PostAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			var response = await PostAsync(ct: cancellationToken);
			var result = await ParseResponseMessage<T>(response);
			return result;
		}

		/// <inheritdoc />
		public Task<HttpResponseMessage> PutAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return SendAsync(HttpMethod.Put, ct, completionOption);
		}

		/// <inheritdoc />
		public Task<T> PutAsync<T>() where T : new()
			=> PutAsync<T>(CancellationToken.None);

		/// <inheritdoc />
		public async Task<T> PutAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			var response = await PutAsync(ct: cancellationToken);
			var result = await ParseResponseMessage<T>(response);
			return result;
		}

		/// <inheritdoc />
		public Task<HttpResponseMessage> DeleteAsync(HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
			CancellationToken ct = default(CancellationToken))
		{
			return SendAsync(HttpMethod.Delete, ct, completionOption);
		}

		/// <inheritdoc />
		public Task<T> DeleteAsync<T>() where T : new()
			=> DeleteAsync<T>(CancellationToken.None);

		/// <inheritdoc />
		public async Task<T> DeleteAsync<T>(CancellationToken cancellationToken) where T : new()
		{
			var response = await DeleteAsync(ct: cancellationToken);
			var result = await ParseResponseMessage<T>(response);
			return result;
		}

		#endregion

		#region Protected methods to prepare request and parse response

		protected async Task EnsureSuccess(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode && _successOnly)
			{
				string userMessage = string.Empty;
				string responseContent = string.Empty;
				object debugInfo = null;

				try
				{
					responseContent = await response.Content.ReadAsStringAsync();
					var webImpactResponse = JsonConvert.DeserializeObject<BaseResponseVm>(responseContent);
					userMessage = webImpactResponse.UserMessage;
					debugInfo = webImpactResponse;
				}
				catch // if response is not WebImpact response model
				{
					debugInfo = responseContent;
				}

				var systemMessage = $"External data request to {_endpoint} failed with the reason: {response.ReasonPhrase}";
				if (!string.IsNullOrEmpty(userMessage))
					systemMessage += $": {userMessage}";
				else
					userMessage = response.ReasonPhrase;
				throw new WebImpactExternalResourceException(userMessage, systemMessage, debugInfo, response.RequestMessage);
			}
		}

		protected async Task<T> ParseResponseMessage<T>(HttpResponseMessage response)
		{
			await EnsureSuccess(response);

			var content = await response.Content.ReadAsStringAsync();
			var obj = JsonConvert.DeserializeObject<T>(content);

			return obj;
		}

		protected IEnumerable<KeyValuePair<string, string>> ParseObjectToParams(object obj)
		{
			var properties = obj.GetType().GetProperties();
			foreach (var property in properties)
			{
				yield return new KeyValuePair<string, string>(
					property.Name,
					property.GetValue(obj).ToString()
					);
			}
		}

		protected string GetQueryParameters()
		{
			string result = string.Join("&", QueryData.Select(x => $"{x.Key}={x.Value}"));
			if (!string.IsNullOrEmpty(result))
				return "?" + result;
			else
				return string.Empty;
		}

		#endregion
	}
}
