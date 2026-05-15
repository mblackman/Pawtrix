using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using pawtrix.ViewModels;
using pawtrix.Models;
using pawtrix.Objects;

namespace pawtrix.Views;

public partial class RoomListView: UserControl
{
    public RoomListView()
    {
        Console.WriteLine("Creatingnewroomlist");
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not RoomListViewModel viewModel) return;
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        if (viewModel.Loaded) return;
        viewModel.Loaded = true;
        
        Program.Client.OnMatrixRoomEventsReceived += mainVm.HandleEvent!;
        
        Program.Client.Start();

        Task.Run(async () =>
        {
            while (Program.Client.JoinedRooms.Length == 0)
            {
                await Task.Delay(100);
                Console.WriteLine("Waiting for rooms.");
            }
            
            
            if (Program.Client.Token != null)
            {
                Program.HttpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", Program.Client.Token);
            }
            foreach (var room in Program.Client.JoinedRooms)
            {
                Console.WriteLine(room);
                Console.WriteLine(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state");
                string a = await Program.HttpClient.GetStringAsync(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state");
                JsonElement states = JsonDocument.Parse(a).RootElement;
                Console.WriteLine(states);
                Uri? icon = null;
                //Console.WriteLine(states);
                //Console.WriteLine(parsed.RootElement.GetProperty("name"));
                //Console.WriteLine(a);
                string? roomName;
                try
                {
                    Console.WriteLine(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state/m.room.name");
                    string b = await Program.HttpClient.GetStringAsync(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id +
                                                               "/state/m.room.name");
                    Console.WriteLine(b);
                    roomName = JsonDocument.Parse(b).RootElement.GetProperty("name").ToString();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Caught");
                    roomName = "";
                    foreach (JsonElement roomState in states.EnumerateArray())
                    {
                        Console.WriteLine(roomState);
                        if (roomState.GetProperty("type").GetString() == "m.room.member")
                        {
                            string? userid = roomState.GetProperty("sender").GetString();
                            if (userid == Program.Client.UserId ||
                                roomState.GetProperty("content").GetProperty("membership").ToString() != "join") continue;
                            roomName += userid + " ";
                        }
                    }
                }
                foreach (JsonElement roomState in states.EnumerateArray())
                {
                    Console.WriteLine(roomState);
                    if (roomState.GetProperty("type").GetString() == "m.room.avatar")
                    {
                        Uri mxc = new(roomState.GetProperty("content").GetProperty("url").GetString()!);
                        icon = new(Program.Client.BaseAddress + "_matrix/client/v1/media/download/" + mxc.Host + mxc.PathAndQuery);
                        Console.WriteLine("FOUNDICON");
                        Console.WriteLine(icon);
                        Console.WriteLine(roomState);
                    }
                }
                
                Console.WriteLine("Eveveveve");
                
                Bitmap? setIcon = null;
                if (icon != null)
                {
                    setIcon = await Functions.DownloadImage(icon);
                }
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    RoomButton newButton = new(roomName, setIcon, room.Id);
                    
                    viewModel.Chats.Add(newButton);
                });

                Console.WriteLine("Added1");
            }
        });
    }

    private void Logout(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        Storage.ClearToken();
        mainVm.RestartApp();
    }
}