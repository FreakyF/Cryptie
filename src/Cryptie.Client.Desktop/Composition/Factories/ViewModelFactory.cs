using System;
using Cryptie.Client.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Composition.Factories;

public class ViewModelFactory(IServiceProvider services) : IViewModelFactory
{
    public T Create<T>(IScreen hostScreen) where T : ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(services, hostScreen);
    }
}