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

namespace HttpService.Tests
{
    public class Tests
    {
       [Fact]
        public async void GetAsyncTest()
        {
            //Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                    services.AddSingleton<IHttpService, HttpService>();
                }
                )
                .Configure(app =>
                {
                    app.UseMiddleware<FakeMiddleware>(TimeSpan.FromMilliseconds(20));
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();


            //Act 
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/delay/");
            var responseMessage = await client.SendAsync(requestMessage);

            //Assert
            //TODO: this test provides absolutely no value as of yet
        }


    }
}
