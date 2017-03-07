using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using HttpService;

namespace HttpService.Tests
{
    public class Tests
    {
       [Fact]
        public async void CallMiddleware_WithValidOptions_ThrowsNoException()
        {
            //Arrange
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseHttpServiceMiddleware(new HttpServiceMiddlewareOptions());
                    app.UseMiddleware<FakeMiddleware>(TimeSpan.FromMilliseconds(5));
                });

            var server = new TestServer(builder);

            //Act 
            var requestMessage = new HttpRequestMessage(new HttpMethod("GET"), "/delay/");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            //Assert
            //define your own assertation here. You could for example use the fakemiddleware to output some response like
            // await context.Response.WriteAsync("something"); 
            // and then check the result:
            // Assert.Contains("something", responseMessage.Content.ReadAsStringAsync().Result);
        }
    }
}
