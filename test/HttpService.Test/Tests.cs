using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using HttpService;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
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
        public async void EnhanceClientTest_DontPassToken_AddsCorrelationId()
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
            var client = new HttpClient();
            await httpService.EnhanceClientAsync(client, false);

            //Assert
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").Should().HaveCount(1);
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
            IEnumerable<string> tokenValues = null;
            client.DefaultRequestHeaders.TryGetValues("Authorization", out tokenValues).Should().BeFalse();
        }

        [Fact]
        public async void EnhanceClientTest_PassToken_ButNoTokenPresent_AddsNoAuthHeader()
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
            var client = new HttpClient();
            await httpService.EnhanceClientAsync(client, true);

            //Assert
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").Should().HaveCount(1);
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
            IEnumerable<string> tokenValues = null;
            client.DefaultRequestHeaders.TryGetValues("Authorization", out tokenValues).Should().BeFalse();
        }

        [Fact]
        public async void EnhanceClientTest_PassToken_AddsNoAuthHeader()
        {
            //Arrange
            var contextReader = new Mock<IContextReader>();
            contextReader.Setup(x => x.GetTokenAsync()).Returns(() => Task.FromResult("sometoken"));
            contextReader.Setup(x => x.GetCorrelationId()).Returns("someCorrelationId");

            var httpService = new HttpService(contextReader.Object);

            //Act
            var client = new HttpClient();
            await httpService.EnhanceClientAsync(client, true);

            //Assert
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").Should().HaveCount(1);
            client.DefaultRequestHeaders.GetValues("X-Correlation-Id").First().Should().Be("someCorrelationId");
            IEnumerable<string> tokenValues = null;
            client.DefaultRequestHeaders.TryGetValues("Authorization", out tokenValues).Should().BeTrue();
            tokenValues.First().Should().Be("Bearer sometoken");
        }


    }
}
