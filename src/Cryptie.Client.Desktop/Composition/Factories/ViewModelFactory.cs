using System;
using Cryptie.Client.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptie.Client.Desktop.Composition.Factories;

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>(MainWindowViewModel parent) where T : ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, parent);
    }
}