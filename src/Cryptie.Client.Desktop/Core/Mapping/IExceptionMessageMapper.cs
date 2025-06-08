using System;



public interface IExceptionMessageMapper
{
    string Map(Exception exception);
}