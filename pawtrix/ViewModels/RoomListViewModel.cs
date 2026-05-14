using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using pawtrix.Models;

namespace pawtrix.ViewModels;

public partial class RoomListViewModel : ViewModelBase
{
    public ObservableCollection<Button> Chats { get; } = new();
    
    public string UserName { get; } = Program.Client.UserId;

    public bool Loaded = false;
    
    [RelayCommand]
    private void AddChat()
    {
        
    }
}