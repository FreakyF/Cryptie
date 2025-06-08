using System;
using System.Linq;
using Cryptie.Client.Desktop.Core.Base;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Factories;

public class ViewModelFactory(IServiceProvider services) : IViewModelFactory
{
    public T Create<T>(IScreen hostScreen, params object[] args) where T : ViewModelBase
    {
        var allArgs = new object[] { hostScreen }.Concat(args).ToArray();
        return ActivatorUtilities.CreateInstance<T>(services, allArgs);
    }
}