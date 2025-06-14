using Cryptie.Client.Core.Mapping;
using Cryptie.Common.Features.Authentication.Exceptions;
using JetBrains.Annotations;

namespace Cryptie.Client.Tests.Core.Mapping;

[TestSubject(typeof(ExceptionMessageMapper))]
public class ExceptionMessageMapperTest
{
    private class TestBadCredentialsException : BadCredentialsException { }

    [Fact]
    public void ValidBadCredentialsException()
    {
        var mapper = new ExceptionMessageMapper();
        var exception = new TestBadCredentialsException();
        
        var result = mapper.Map(exception);
        
        Assert.Equal("Wrong username or password", result);
    }
}