using System;

namespace Cryptie.Client.Desktop.Mappers;

public interface IExceptionMessageMapper
{
    string Map(Exception exception);
}