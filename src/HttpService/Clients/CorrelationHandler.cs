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
		private readonly IHttpContextAccessor _accessor;

		public CorrelationHandler(HttpMessageHandler innerHandler, IHttpContextAccessor accessor) : base(innerHandler)
		{
			_accessor = accessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request.Headers.TryAddWithoutValidation("X-Correlation-Id", this._accessor.HttpContext.TraceIdentifier);
			return await base.SendAsync(request, cancellationToken);
		}
	}

}