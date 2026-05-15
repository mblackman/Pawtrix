using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Meowtrix.Sdk.Core.Infrastructure.Dto.User;

namespace pawtrix.ViewModels;

public class MessageViewModel
{
    public string Sender { get; set; } = null!;
    public string Text { get; set; } = null!;

    public MessageViewModel(string sender, string text)
    {
        Console.WriteLine("new message");
        Sender = sender;
        Text = text;

        _ = SetData();
    }
    
    
    private async Task SetData()
    {
        Console.WriteLine("ran async");
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
            {
                MainWindow.DataContext: MainWindowViewModel mainVm
            }) return;
        Console.WriteLine("getting profile");
        MatrixProfile profile = await mainVm.GetProfile(Sender);

        Console.WriteLine("set profile");
        Sender = profile.displayname;
    }
}