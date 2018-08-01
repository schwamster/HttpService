using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HttpService.Handlers
{
	/// <summary>
	/// A delegating handler that adds the HTTP context tracing identifer as a header to the request.
	/// 
	/// Adds the tracing identifier as a <code>X-Correlation-Id</code> header.
	/// </summary>
	public class CorrelationHandler : DelegatingHandler
	{
		private readonly IContextReader _tokenExtractor;

		public CorrelationHandler(HttpMessageHandler innerHandler, IHttpContextAccessor accessor) : this(innerHandler, new HttpContextReader(accessor))
		{ }

		public CorrelationHandler(HttpMessageHandler innerHandler, IContextReader tokenExtractor) : base(innerHandler)
		{
			_tokenExtractor = tokenExtractor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request.Headers.TryAddWithoutValidation("X-Correlation-Id", _tokenExtractor.GetCorrelationId());
			return await base.SendAsync(request, cancellationToken);
		}
	}

}