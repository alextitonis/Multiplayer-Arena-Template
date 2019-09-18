using System;
using System.Linq;

public class Utils
{
    static Random rnd = new Random();

    public static double MinutesToMilliseconds(int minutes)
    {
        return TimeSpan.FromMinutes(minutes).TotalMilliseconds;
    }
    public static double SecondsToMilliseconds(double seconds)
    {
        return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }

    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return "";
    }
}