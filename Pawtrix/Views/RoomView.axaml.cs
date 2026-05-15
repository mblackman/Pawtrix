using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Meowtrix.Sdk.Core.Domain.RoomEvent;
using pawtrix.Objects;
using pawtrix.ViewModels;
using Tmds.DBus.Protocol;

namespace pawtrix.Views;

public partial class RoomView: UserControl
{
    
    public RoomView()
    {
        InitializeComponent();
    }

    public void SendClick(object? sender, RoutedEventArgs a)
    {
        if (sender is not Button button) return;

        SendMessage();
    }

    public void Exit(object? sender, RoutedEventArgs a)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
            {
                MainWindow.DataContext: MainWindowViewModel mainVm
            }) return;
        Console.WriteLine("ToChats");
        mainVm.NavigateToRooms();
    }

    private void TextBoxInput(object? sender, KeyEventArgs e)
    {
        Console.WriteLine("hello");
        if (e.Key != Key.Enter) return;
        
        if (e.KeyModifiers == KeyModifiers.Shift)
        {
            Console.WriteLine("newline");
            MessageBox.Text += "\n";
        }
        else
        {
            Console.WriteLine("evil");
            
        }
    }

    private void SendMessage()
    {
        if (DataContext is not RoomViewModel viewModel) return;
            
        var text = MessageBox.Text;
                
        MessageBox.Text = "";
        
        Task.Run(async () =>
        {
            await Program.Client.SendMessageAsync(viewModel.RoomId, text!);
        });
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        Console.WriteLine("Contextchange");

        if (DataContext is not RoomViewModel viewModel) return;
        if (viewModel.Preloaded) return;
        viewModel.Preloaded = true;
        Console.WriteLine("Not preloaded yet");

        _ = Messages();
    }
    
    private async Task Messages()
    {
        Console.WriteLine("Hello");
        if (DataContext is not RoomViewModel viewModel) return;
        
        int preloadedMessages = 0;
        
        Console.WriteLine("Loading history");

        Task<bool> StopCallback(BaseRoomEvent a)
        {
            try
            {
                preloadedMessages += 1;
                if (preloadedMessages > 20)
                {
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception exception)
            {
                return Task.FromException<bool>(exception);
            }
        }
        Console.WriteLine("Found history");

        List<BaseRoomEvent> history = await Program.Client.GetHistory(viewModel.RoomId, StopCallback);
        
        Console.WriteLine("Going over history");
        
        for (var i = history.Count - 1;i>=0; i--)
        {
            var state = history[i];

            if (state is not TextMessageEvent message) continue;
            
            MessageViewModel newMessage = new(message.SenderUserId, message.Message);
            
            viewModel.AddMessage(newMessage);
            
            var scrollViewer = MessageListBox.FindDescendantOfType<ScrollViewer>();
            scrollViewer?.ScrollToEnd();
        }

        while (DataContext is RoomViewModel)
        {
            int num = viewModel.Messages.Count;

            while (num == viewModel.Messages.Count)
            {
                await Task.Delay(200);
            }
            
            Console.WriteLine("new message");
            
            var scrollViewer = MessageListBox.FindDescendantOfType<ScrollViewer>();
            scrollViewer!.Offset = new Vector(0, scrollViewer.Extent.Height);
        }
    }
}