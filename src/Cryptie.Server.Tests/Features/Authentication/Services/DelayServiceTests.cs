using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cryptie.Server.Features.Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.Authentication.Services
{
    public class DelayServiceTests
    {
        [Fact]
        public async Task FakeDelay_ReturnsResultFromFunc()
        {
            
            var service = new DelayService();
            var expectedResult = new OkResult();
            Func<IActionResult> func = () => expectedResult;

            
            var result = await service.FakeDelay(func);

            
            Assert.Same(expectedResult, result);
        }

        [Fact]
        public async Task FakeDelay_DelaysIfFuncIsFast()
        {
            
            var service = new DelayService();
            var func = new Func<IActionResult>(() => new OkResult());
            var stopwatch = Stopwatch.StartNew();

            
            await service.FakeDelay(func);
            stopwatch.Stop();

            
            Assert.True(stopwatch.ElapsedMilliseconds >= 95); 
        }
        
    }
}

