using System;

namespace Cryptie.Client.Core.Mapping;

public interface IExceptionMessageMapper
{
    string Map(Exception exception);
}