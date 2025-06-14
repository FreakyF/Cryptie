using Cryptie.Client.Core.Mapping;
using Cryptie.Common.Features.Authentication.Exceptions;

namespace Cryptie.Client.Tests.Core.Mapping;

public class ExceptionMessageMapperTest
{
    [Fact]
    public void ValidBadCredentialsException()
    {
        var mapper = new ExceptionMessageMapper();
        var exception = new TestBadCredentialsException();

        var result = mapper.Map(exception);

        Assert.Equal("Wrong username or password", result);
    }

    private class TestBadCredentialsException : BadCredentialsException
    {
    }
}