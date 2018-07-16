using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttpService.Clients
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class CorrelationHandler : DelegatingHandler
	{
		private readonly IContextReader _tokenExtractor;

		public CorrelationHandler(HttpMessageHandler innerHandler, IHttpContextAccessor accessor) : this(innerHandler, new HttpContextReader(accessor))
		{}

		public CorrelationHandler(HttpMessageHandler innerHandler, IContextReader tokenExtractor) : base(innerHandler)
		{
			_tokenExtractor = tokenExtractor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request.Headers.TryAddWithoutValidation("X-Correlation-Id", this._tokenExtractor.GetCorrelationId());
			return await base.SendAsync(request, cancellationToken);
		}
	}

}