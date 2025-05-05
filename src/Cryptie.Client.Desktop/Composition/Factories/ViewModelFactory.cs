using System;
using Cryptie.Client.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Composition.Factories;

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>(IScreen host) where T : ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, host);
    }
}