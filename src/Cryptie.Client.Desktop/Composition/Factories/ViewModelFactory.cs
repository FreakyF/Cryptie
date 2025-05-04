using System;
using Cryptie.Client.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptie.Client.Desktop.Composition.Factories;

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>() where T : ViewModelBase
        => serviceProvider.GetRequiredService<T>();
}