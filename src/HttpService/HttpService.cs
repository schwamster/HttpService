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
        private readonly IHttpContextAccessor _accessor;

        public HttpService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, bool passToken = false)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Correlation-Id", this._accessor.HttpContext.TraceIdentifier);
                if (passToken)
                {
                    var token = await this._accessor.HttpContext.Authentication.GetTokenAsync("access_token");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                }

                var response = await client.GetAsync(requestUri);

                return response;
            }
        }
    }

}