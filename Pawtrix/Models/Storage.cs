namespace pawtrix.Models;

using System;
using System.IO;

public static class Storage
{
    private static readonly string AppDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
        ".pawtrix"
    );
    
    private static readonly string TokenFile = Path.Combine(AppDir, "token.txt");

    public static void SaveToken(string token)
    {
        if (!Directory.Exists(AppDir))
        {
            Directory.CreateDirectory(AppDir);
        }

        File.WriteAllText(TokenFile, token);

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.SetUnixFileMode(TokenFile, 
                UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }

    public static string? LoadToken()
    {
        return File.Exists(TokenFile) ? File.ReadAllText(TokenFile) : null;
    }
    
    public static void ClearToken()
    {
        if (File.Exists(TokenFile))
        {
            File.Delete(TokenFile);
        }
    }
}