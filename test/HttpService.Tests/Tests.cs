using System.Net.Http;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using System.Linq;
using System.Collections.Generic;
using HttpService.Clients;
using System.Threading;
using System;

namespace HttpService.Tests
{
	public class Tests
   {


        [Fact]
        public async void CreateMessageTest_DontPassToken_AddsCorrelationId()
        {
			//Arrange
			var testHandler = new TestHandler();

            var httpContext = new Mock<HttpContext>();
            var authentication = new Mock<AuthenticationManager>();

            httpContext.Setup(x => x.Authentication).Returns(authentication.Object);
            httpContext.Setup(x => x.TraceIdentifier).Returns("someCorrelationId");

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext.Object);

            var httpService = new HttpService(httpContextAccessor.Object, new HttpClient(new CorrelationHandler(testHandler, httpContextAccessor.Object)));

			//Act
			await httpService.GetAsync("http://example.com/", false);

			//Assert
			testHandler.Request.Headers.GetValues("X-Correlation-Id").Should().HaveCount(1);
			testHandler.Request.Headers.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
			testHandler.Request.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeFalse();
		}

        [Fact]
        public async void CreateMessageTest_PassToken_AddsNoAuthHeader()
        {
			//Arrange
			var testHandler = new TestHandler();
            var contextReader = new Mock<IContextReader>();
            contextReader.Setup(x => x.GetTokenAsync()).Returns(() => Task.FromResult("sometoken"));
            contextReader.Setup(x => x.GetCorrelationId()).Returns("someCorrelationId");

            var httpService = new HttpService(contextReader.Object, new HttpClient(new CorrelationHandler(testHandler, contextReader.Object)));

			//Act
			await httpService.GetAsync("http://example.com/", true);

			//Assert
			testHandler.Request.Headers.GetValues("X-Correlation-Id").Should().HaveCount(1);
			testHandler.Request.Headers.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
			testHandler.Request.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeTrue();
			tokenValues.First().Should().Be("Bearer sometoken");
        }
    }

	public class TestHandler : HttpMessageHandler
	{
		private HttpRequestMessage _request;

		public TestHandler()
		{
		}

		public HttpRequestMessage Request { get => _request; set => _request = value; }

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Request = request;
			return await Task.FromResult(new HttpResponseMessage());
		}


	}
}

