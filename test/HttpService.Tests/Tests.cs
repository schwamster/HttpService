using System.Net.Http;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using System.Linq;
using System.Collections.Generic;

namespace HttpService.Tests
{
	public class Tests
   {


        [Fact]
        public async void CreateMessageTest_DontPassToken_AddsCorrelationId()
        {
            //Arrange
            var httpContext = new Mock<HttpContext>();
            var authentication = new Mock<AuthenticationManager>();

            httpContext.Setup(x => x.Authentication).Returns(authentication.Object);
            httpContext.Setup(x => x.TraceIdentifier).Returns("someCorrelationId");

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext.Object);

            var httpService = new HttpService(httpContextAccessor.Object);

			//Act
			var msg = await httpService.CreateMessage(HttpMethod.Get, "localhost", false);

            //Assert
			msg.Headers.GetValues("X-Correlation-Id").Should().HaveCount(1);
			msg.Headers.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
			msg.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeFalse();
		}

        [Fact]
        public async void CreateMessageTest_PassToken_AddsNoAuthHeader()
        {
            //Arrange
            var contextReader = new Mock<IContextReader>();
            contextReader.Setup(x => x.GetTokenAsync()).Returns(() => Task.FromResult("sometoken"));
            contextReader.Setup(x => x.GetCorrelationId()).Returns("someCorrelationId");

            var httpService = new HttpService(contextReader.Object);

			//Act
			var msg = await httpService.CreateMessage(HttpMethod.Get, "localhost", true);

			//Assert
			msg.Headers.GetValues("X-Correlation-Id").Should().HaveCount(1);
			msg.Headers.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
			msg.Headers.TryGetValues("Authorization", out IEnumerable<string> tokenValues).Should().BeTrue();
			tokenValues.First().Should().Be("Bearer sometoken");
        }


    }
}

