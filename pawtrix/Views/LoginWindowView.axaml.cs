using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using pawtrix.Models;
using pawtrix.ViewModels;

namespace pawtrix.Views;

public partial class LoginWindowView : UserControl
{
    public LoginWindowView()
    {
        InitializeComponent();
        Console.WriteLine("Hello");

        Task.Run(async () =>
        {
            Console.WriteLine("Async");
            string? data = Storage.LoadToken();

            if (data != null)
            {
                Console.WriteLine("Found data");
                JsonElement root = JsonDocument.Parse(data).RootElement;
                Uri homeserver = new Uri(root.GetProperty("BaseAddress").GetString()!);
                string token = root.GetProperty("Token").GetString()!;
                string login = root.GetProperty("UserId").GetString()!;
                
                Console.WriteLine("Found stuff, logging in");

                await Program.Client.LoginAsync(homeserver, token, login);
                
                Dispatcher.UIThread.Post(() =>
                {
                    if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
                        {
                            MainWindow.DataContext: MainWindowViewModel mainVm
                        }) return;
                    Console.WriteLine("ToChat");
                    mainVm.NavigateToRooms();
                });
            }
        });
    }

    private void Button_Login(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }
        
        button.IsEnabled = false;
        button.Content = "Logging in...";

        if (DataContext is LoginWindowViewModel viewModel)
        {
            var homeserver = new Uri("https://"+viewModel.Homeserver);
            Console.WriteLine(homeserver);   
            try
            {
                string responseBody =
                    Program.HttpClient.GetStringAsync(homeserver + "_matrix/federation/v1/version").Result;
                Console.WriteLine(responseBody);

                homeserver = new("https://" + viewModel.Homeserver);
            }
            catch (AggregateException)
            {
                try
                {
                    Console.WriteLine(homeserver + ".well-known/matrix/server");   
                    string responseBody =
                        Program.HttpClient.GetStringAsync(homeserver + ".well-known/matrix/server").Result;
                    Console.WriteLine(responseBody);

                    var parsed = JsonDocument.Parse(responseBody);

                    parsed.RootElement.TryGetProperty("m.server", out var server);
                    if (server.GetString() == null) return;
                    Console.WriteLine(server.GetString());

                    // Still in the same try scope so if it errors out it'll catch the exception too
                    Program.HttpClient.GetStringAsync("https://" + server.GetString()).Wait();

                    homeserver = new("https://" + server.GetString());
                }
                catch (AggregateException)
                {
                    Console.WriteLine("Homeserver invalid!");
                    return;
                }
            }
            
            var login = viewModel.Login;
            var password = viewModel.Password;

            Program.Client.LoginAsync(homeserver, login, password, "SillyTestDeviceId").ContinueWith(a =>
            {
                Console.WriteLine(a);
                Console.WriteLine(Program.Client.IsLoggedIn);
            
                if (Program.Client.IsLoggedIn)
                {
                    Console.WriteLine(Program.Client.IsLoggedIn);
                    Console.WriteLine("Bruhh");
                    Console.WriteLine();
                    Console.WriteLine("Smhhh");

                    var userData = new { Program.Client.BaseAddress, Program.Client.UserId, Program.Client.Token };
                    string serialized = JsonSerializer.Serialize(userData);
                    
                    Storage.SaveToken(serialized);
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime
                            {
                                MainWindow.DataContext: MainWindowViewModel mainVm
                            }) return;
                        Console.WriteLine("ToChat");
                        mainVm.NavigateToRooms();
                    });
                }
                else
                {
                    button.Content = "Failed to log in";
                    button.IsEnabled = true;
                }
            });
        }
        else
        {
            Console.WriteLine("Broken");
        }

    }
}