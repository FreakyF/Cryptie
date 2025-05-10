using System;
using System.Net.Http;
using Cryptie.Common.Features.Authentication.Exceptions;

namespace Cryptie.Client.Desktop.Mappers;

public class ExceptionMessageMapper : IExceptionMessageMapper
{
    public string Map(Exception exception)
    {
        return exception switch
        {
            BadCredentialsException => "Wrong username or password",
            HttpRequestException http when (int?)http.StatusCode == 400 => http.Message,
            OperationCanceledException => string.Empty,
            _ => exception.Message
        };
    }
}