using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace HttpService
{
	/// <summary>
	/// A service for performing well formed HTTP requests.
	///
	/// Optionaly correctly passes authorization tokens along.
	/// </summary>
	public class HttpService : IHttpService
	{
		private readonly IContextReader _tokenExtractor;
		private HttpClient _client;

		/// <summary>
		/// Create a service object wrapping <code>accessor</code> with a defualt IContextReader.
		/// </summary>
		/// <param name="accessor"></param>
		/// <param name="client"></param>
		public HttpService(IHttpContextAccessor accessor, HttpClient client = null) : this(new HttpContextReader(accessor), client)
		{
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="tokenExtractor"></param>
		/// <param name="client"></param>
		public HttpService(IContextReader tokenExtractor, HttpClient client = null)
		{
			_tokenExtractor = tokenExtractor;
			_client = client ?? new HttpClient();
		}

		/// <summary>
		/// Perform a HTTP get request with this service.
		/// </summary>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <returns>The response of the request.</returns>
		public async Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken) =>
			await SendAsync(HttpMethod.Get, requestUri, passToken);

		/// <summary>
		/// Perform a HTTP post request with this service.
		/// </summary>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="content">The content of the request.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <returns>The response of the request.</returns>
		public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, bool passToken) =>
			await SendAsync(HttpMethod.Post, requestUri, passToken, content);

		/// <summary>
		/// Perform a HTTP Put request with this service.
		/// </summary>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="content">The content of the request.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <returns>The response of the request.</returns>
		public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, bool passToken) =>
			await SendAsync(HttpMethod.Put, requestUri, passToken, content);

		/// <summary>
		/// Perform a HTTP delete request with this service.
		/// </summary>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <returns>The response of the request.</returns>
		public async Task<HttpResponseMessage> DeleteAsync(string requestUri, bool passToken) =>
			await SendAsync(HttpMethod.Delete, requestUri, passToken);

		/// <summary>
		/// Perform a HTTP request.
		/// </summary>
		/// <param name="method">Determines what HTPP method to use for the request. </param>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <param name="content">The content of the request. (Should only be non-null when performing a put or post request.)</param>
		/// <returns>The response of the request.</returns>
		private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri, bool passToken, HttpContent content = null)
		{
			HttpRequestMessage msg = await CreateMessage(method, requestUri, passToken, content);

			return await _client.SendAsync(msg);
		}

		/// <summary>
		/// Creates a HTTP request message.
		///
		/// Adds the header <code>X-Correlation-Id</code> to the request. A <code>Authorization</code> header is also added if <code>passToken = true</code>.
		/// </summary>
		/// <param name="method">Determines what HTTP method to use for the request. </param>
		/// <param name="requestUri">The URI to make the request to.</param>
		/// <param name="passToken">Indicate if the authorization token used to authorize with in this application should be passed along the request.</param>
		/// <param name="content">The content of the request. (Should only be non-null when performing a put or post request.)</param>
		/// <returns></returns>
		internal async Task<HttpRequestMessage> CreateMessage(HttpMethod method, string requestUri, bool passToken, HttpContent content = null)
		{
			var msg = new HttpRequestMessage(method, requestUri);
			
			if (passToken)
			{
				var token = await _tokenExtractor.GetTokenAsync();
				if (!string.IsNullOrEmpty(token))
				{
					msg.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
				}
			}

			if (content != null)
			{
				msg.Content = content;
			}

			return msg;
		}
	}

	public interface IContextReader
	{
		Task<string> GetTokenAsync();

		string GetCorrelationId();
	}

	public class HttpContextReader : IContextReader
	{
		private readonly IHttpContextAccessor _accessor;

		public HttpContextReader(IHttpContextAccessor accessor)
		{
			this._accessor = accessor;
		}

		public string GetCorrelationId()
		{
			return this._accessor.HttpContext.TraceIdentifier;
		}

		public async Task<string> GetTokenAsync()
		{
			var token = await this._accessor.HttpContext.GetTokenAsync("access_token");
			return token;
		}
	}
}