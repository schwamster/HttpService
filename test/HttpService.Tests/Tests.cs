using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HttpService.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Moq;
using Xunit;

namespace HttpService.Tests
{
	public class Tests
   {
		
		[Fact]
		public async void SendGetMessagePassingTokenTest()
		{
			//Arrange
			var testHandler = new TestHandler();
			var extractor = new Mock<IContextReader>();
			extractor.Setup(x => x.GetTokenAsync()).Returns(Task.FromResult("sometoken"));

			var service = new HttpService(extractor.Object, new HttpClient(testHandler));

			//Act
			service.GetAsync("http://example.com", true);

			//Assert
			{   //Assert msg is using get
				testHandler.Request.Method.Method.Should().Be(HttpMethod.Get.Method);
			}
			{   //Assert auth token was passeed
				testHandler.Request.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeTrue();
				tokenValues.Any(tokenValue => tokenValue == "Bearer sometoken").Should().BeTrue();
			}
		}

		[Fact]
		public async void SendGetMessageNotPassingTokenTest()
		{
			//Arrange
			var testHandler = new TestHandler();
			var extractor = new Mock<IContextReader>();
			extractor.Setup(x => x.GetTokenAsync()).Returns(Task.FromResult("sometoken"));

			var service = new HttpService(extractor.Object, new HttpClient(testHandler));

			//Act
			service.GetAsync("http://example.com", false);

			//Assert
			{   //Assert msg is using get
				testHandler.Request.Method.Method.Should().Be(HttpMethod.Get.Method);
			}
			{   //Assert auth token was passeed
				testHandler.Request.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeFalse();
			}
		}

		[Fact]
		public async void AddTraceHeaderTest()
		{
			//Arrange
			var testHandler = new TestHandler();
			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext.TraceIdentifier).Returns("someCorrelationId");

			var client = new HttpClient(new CorrelationHandler(testHandler, accessor.Object));

			//Act
			await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://example.com"));

			//Assert
			testHandler.Request.Headers.TryGetValues("X-Correlation-Id", out IEnumerable<string> tokenValues).Should().BeTrue();
			tokenValues.Should().Contain("someCorrelationId");
		}
	}

	public class TestHandler : HttpMessageHandler
	{
		public TestHandler()
		{
		}

		public HttpRequestMessage Request { get; set; }

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Request = request;
			return await Task.FromResult(new HttpResponseMessage());
		}


	}
}

