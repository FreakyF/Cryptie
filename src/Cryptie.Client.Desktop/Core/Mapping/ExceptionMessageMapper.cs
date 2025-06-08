using System;
using Cryptie.Common.Features.Authentication.Exceptions;

namespace Cryptie.Client.Desktop.Core.Mapping;

public class ExceptionMessageMapper : IExceptionMessageMapper
{
    public string Map(Exception exception)
    {
        return exception switch
        {
            BadCredentialsException => "Wrong username or password",
            _ => string.Empty
        };
    }
}