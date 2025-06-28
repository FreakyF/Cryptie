using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Cryptie.Client.Features.Chats.ViewModels;

namespace Cryptie.Client.Features.Chats.Views;

public partial class ChatsView : ReactiveUserControl<ChatsViewModel>
{
    private ScrollViewer? _scrollViewer;
    private ChatsViewModel? _vm;

    public ChatsView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _scrollViewer = this.FindControl<ScrollViewer>("MessagesScrollViewer");
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_vm != null)
        {
            ((INotifyCollectionChanged)_vm.Messages).CollectionChanged -= Messages_CollectionChanged;
        }

        _vm = DataContext as ChatsViewModel;

        if (_vm != null)
        {
            ((INotifyCollectionChanged)_vm.Messages).CollectionChanged += Messages_CollectionChanged;
        }
    }

    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            Dispatcher.UIThread.Post(() => { _scrollViewer?.ScrollToEnd(); }, DispatcherPriority.Background);
        }
    }
}