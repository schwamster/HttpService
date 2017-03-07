using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using HttpService;
using FluentAssertions;

namespace HttpService.Tests
{
    public class Tests
    {
       [Fact]
        public async void GetAsyncTest()
        {
            //Arrange
            var httpService = new HttpService(null);

            //Act 
            //do something

            //Assert
            true.Should().Be(true);
        }
    }
}
