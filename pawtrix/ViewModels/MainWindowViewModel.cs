using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Meowtrix.Sdk;
using Meowtrix.Sdk.Core.Domain.RoomEvent;
using Meowtrix.Sdk.Core.Infrastructure.Dto.User;
using Tmds.DBus.Protocol;

namespace pawtrix.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage = new LoginWindowViewModel(); // Текущая страница
    
    private readonly Dictionary<string, RoomViewModel> _rooms = new ();
    
    private RoomListViewModel? _roomListViewModel;
    private Dictionary<string, MatrixProfile>  _matrixProfiles = new();
    
    public void NavigateToRooms()
    {
        _roomListViewModel ??= new RoomListViewModel();

        CurrentPage = _roomListViewModel;
    }

    public void NavigateToRoom(string roomid, string roomName)
    {
        if (!_rooms.ContainsKey(roomid)) _rooms.Add(roomid, new RoomViewModel(roomid, roomName));
        CurrentPage = _rooms[roomid];
    }

    public void HandleEvent(object sender, MatrixRoomEventsEventArgs eventArgs)
    {
        foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
        {
            Console.WriteLine("Haiiiii");
            if (roomEvent is not TextMessageEvent textMessageEvent)
                continue;

            string roomId = textMessageEvent.RoomId;
            if (CurrentPage is not RoomViewModel roomViewModel || roomViewModel.RoomId != roomId) continue;
            
            string senderId = textMessageEvent.SenderUserId;
            string text = textMessageEvent.Message;
            DateTimeOffset time = textMessageEvent.Timestamp;

            if (CurrentPage is RoomViewModel room)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    room.AddMessage(new MessageViewModel(senderId, text));
                });
            }
        }
    }
    
    public void RestartApp()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        { 
            desktop.Shutdown();
        }
    }

    public async Task<MatrixProfile> GetProfile(string userId)
    {
        if (_matrixProfiles.TryGetValue(userId, out var profile1)) return profile1;
        
        MatrixProfile profile = await Program.Client.GetUserProfile(userId);
        Console.WriteLine(profile.avatar_url);
        _matrixProfiles.Add(userId, profile);

        Console.WriteLine("Returnprofile");
        return _matrixProfiles[userId];
    }
}