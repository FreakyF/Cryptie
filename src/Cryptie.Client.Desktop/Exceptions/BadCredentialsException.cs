using System;

namespace Cryptie.Client.Desktop.Exceptions;

public abstract class BadCredentialsException() : Exception("Wrong username or password");