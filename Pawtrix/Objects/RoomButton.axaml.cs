using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using pawtrix.ViewModels;

namespace pawtrix.Objects;

public partial class RoomButton : Button
{
    private readonly string _roomId;

    public RoomButton()
    {
        InitializeComponent();
        _roomId = string.Empty;
    }
    
    public RoomButton(string name, Bitmap? icon, string roomId)
    {
        InitializeComponent();
        
        if (icon != null) Icon.Source = icon;
        Name = name;
        Text.Text = name;
        _roomId = roomId;
    }

    public void OpenRoom(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not Button) return;
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        
        Console.WriteLine("ToChat");
        mainVm.NavigateToRoom(_roomId, Name!);
    }
}
