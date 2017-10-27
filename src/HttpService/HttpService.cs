using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;

namespace HttpService
{

    public class HttpService : IHttpService
    {
        private readonly IContextReader _tokenExtractor;

        public HttpService(IHttpContextAccessor accessor)
        {
            this._tokenExtractor = new HttpContextReader(accessor);
        }

        public HttpService(IContextReader tokenExtractor)
        {
            _tokenExtractor = tokenExtractor;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken = false)
        {
            using (var client = new HttpClient())
            {
                await EnhanceClientAsync(client, passToken);

                var response = await client.GetAsync(requestUri);

                return response;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, bool passToken = false)
        {
            using (var client = new HttpClient())
            {
                await EnhanceClientAsync(client, passToken);

                var response = await client.PostAsync(requestUri, content);

                return response;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, bool passToken)
        {
            using (var client = new HttpClient())
            {
                await EnhanceClientAsync(client, passToken);

                var response = await client.PutAsync(requestUri, content);

                return response;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, bool passToken)
        {
            using (var client = new HttpClient())
            {
                await EnhanceClientAsync(client, passToken);

                var response = await client.DeleteAsync(requestUri);

                return response;
            }
        }

        internal async Task EnhanceClientAsync(HttpClient client, bool passToken)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Correlation-Id", this._tokenExtractor.GetCorrelationId());
            if (passToken)
            {
                var token = await _tokenExtractor.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                }
            }
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