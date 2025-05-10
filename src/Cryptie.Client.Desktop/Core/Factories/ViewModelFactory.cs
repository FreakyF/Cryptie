using System;
using Cryptie.Client.Desktop.Core.Base;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Factories;

public class ViewModelFactory(IServiceProvider services) : IViewModelFactory
{
    public T Create<T>(IScreen hostScreen) where T : ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(services, hostScreen);
    }
}