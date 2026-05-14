namespace pawtrix.ViewModels;

public class LoginWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Pawtrix! \nPut in your homeserver, login and password below.";
    public string Homeserver { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}