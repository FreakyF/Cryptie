using System;

namespace Cryptie.Client.Desktop.Core.Mapping;

public interface IExceptionMessageMapper
{
    string Map(Exception exception);
}