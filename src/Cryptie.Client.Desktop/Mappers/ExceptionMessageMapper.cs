using System;
using System.Net.Http;
using Cryptie.Client.Desktop.Exceptions;

namespace Cryptie.Client.Desktop.Mappers;

public class ExceptionMessageMapper : IExceptionMessageMapper
{
    public string Map(Exception exception) =>
        exception switch
        {
            BadCredentialsException => "Wrong username or password",
            HttpRequestException http when (int?)http.StatusCode == 400 => http.Message,
            OperationCanceledException => string.Empty,
            _ => exception.Message
        };
}