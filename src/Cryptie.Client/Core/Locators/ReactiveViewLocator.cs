using System;
using System.Linq;
using ReactiveUI;

namespace Cryptie.Client.Core.Locators;

public class ReactiveViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var vmType = viewModel.GetType();
        var viewType = FindViewType(vmType);
        if (viewType is null)
        {
            throw new InvalidOperationException($"No view found for '{vmType.FullName ?? vmType.Name}'");
        }

        if (Activator.CreateInstance(viewType) is not IViewFor view)
        {
            throw new InvalidOperationException(
                $"Type '{viewType.FullName ?? viewType.Name}' does not implement IViewFor.");
        }

        view.ViewModel = viewModel;
        return view;
    }

    private static Type? FindViewType(Type viewModelType)
    {
        var fullName = viewModelType.FullName;
        if (string.IsNullOrEmpty(fullName))
        {
            return null;
        }

        var viewName = fullName
            .Replace("ViewModels", "Views")
            .Replace("ViewModel", "View");

        return new[] { viewModelType.Assembly }
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .Select(asm => asm.GetType(viewName))
            .OfType<Type>()
            .FirstOrDefault();
    }
}