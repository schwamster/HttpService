using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace HttpService
{

	public class HttpService : IHttpService
	{
		private readonly IContextReader _tokenExtractor;
		private HttpClient _client;

		public HttpService(IHttpContextAccessor accessor) : this(new HttpContextReader(accessor))
		{
		}

		public HttpService(IContextReader tokenExtractor)
		{
			_tokenExtractor = tokenExtractor;
			_client = new HttpClient();
		}

		public async Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken) =>
			await SendAsync(HttpMethod.Get, requestUri, passToken);

		public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, bool passToken) =>
			await SendAsync(HttpMethod.Post, requestUri, passToken, content);

		public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, bool passToken) =>
			await SendAsync(HttpMethod.Put, requestUri, passToken, content);

		public async Task<HttpResponseMessage> DeleteAsync(string requestUri, bool passToken) =>
			await SendAsync(HttpMethod.Delete, requestUri, passToken);

		private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string requestUri, bool passToken, HttpContent content = null)
		{
			var msg = new HttpRequestMessage(method, requestUri);
			if (passToken)
			{
				var token = await _tokenExtractor.GetTokenAsync();
				if (!string.IsNullOrEmpty(token))
				{
					msg.Headers.Add("Authorization", $"Bearer {token}");
				}
			}

			if (content != null)
			{
				msg.Content = content;
			}

			return await _client.SendAsync(msg);
		}
	}


	public interface IContextReader
	{
		Task<string> GetTokenAsync();
	}

	public class HttpContextReader : IContextReader
	{
		private readonly IHttpContextAccessor _accessor;
		public HttpContextReader(IHttpContextAccessor accessor)
		{
			this._accessor = accessor;
		}

		public async Task<string> GetTokenAsync()
		{
			var token = await this._accessor.HttpContext.GetTokenAsync("access_token");
			return token;
		}
	}

}