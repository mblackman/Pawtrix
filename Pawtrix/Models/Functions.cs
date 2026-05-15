using System;
using Avalonia.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;

namespace pawtrix.Models;

public static class Functions
{
    public static async Task<Bitmap?> DownloadImage(Uri url)
    {
        Console.WriteLine("hai");
        try
        {
            Console.WriteLine("hai");
            var response = await Program.HttpClient.GetAsync(url);
            Console.WriteLine("hai");
            var stream = await response.Content.ReadAsStreamAsync();
            Console.WriteLine("hai");
            return new Bitmap(stream);
        }
        catch (Exception)
        {
            return null;
        }
    }
}