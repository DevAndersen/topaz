using DevAndersen.Topaz.Client;
using DevAndersen.Topaz.Common;
using DevAndersen.Topaz.ServerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevAndersen.Topaz.DemoServer
{
    class Program
    {
        static void Main()
        {
            Server server = new Server();

            server.RegisterCommands(
                // Example: "Let's go ahead and stop the server."
                new Command("stop.*server", (match) =>
                {
                    server.Stop();
                }),
                new Command("ping", (match) =>
                {
                    server.MessageSender.SendExit("Pong.");
                    return false;
                }),
                new Command("errortest", (match) =>
                {
                    server.MessageSender.SendError("This is an error message.");
                    return false;
                }),
                // Example: "Match my volume and brightness."
                new Command("match.*volume.*brightness", (match) =>
                {
                    double brightness = server.MessageSender.GetBrightness();
                    server.MessageSender.SetVolume(brightness);
                }),
                // Example: "Search Google" or "Search YouTube for me at the zoo"
                new Command(@"search\s(?:(?:(?'service'.+)(?:\s(?:for)\s(?'query'.+)))|(?'service'.+))", (match) =>
                {
                    Group serviceGroup = match.Groups["service"];
                    Group queryGroup = match.Groups["query"];

                    string GetQuery() => queryGroup.Success switch
                    {
                        true => queryGroup.Value,
                        false => server.MessageSender.GetText($"Search {serviceGroup.Value} for:")
                    };

                    string GetUrl() => serviceGroup.Value.ToLower() switch
                    {
                        "google" => $"http://www.google.com/search?q={GetQuery()}",
                        "wikipedia" => $"https://en.wikipedia.org/wiki/Special:Search/{GetQuery()}",
                        "youtube" => $"https://www.youtube.com/results?search_query={GetQuery()}",
                        _ => string.Empty
                    };

                    string url = GetUrl().Replace(' ', '+'); ;
                    if (string.IsNullOrEmpty(url))
                    {
                        server.MessageSender.SendError($"Service '{serviceGroup.Value}' is not supported.");
                        return false;
                    }

                    Process.Start(new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    });
                    return true;
                }),
                // Example: "Set my screen brightness to 45 percent, please."
                new Command(@"set.*brightness.*?(\d+)", (match) =>
                {
                    if (double.TryParse(match.Groups[1].Value, out double brightness))
                    {
                        if (brightness > 100)
                        {
                            server.MessageSender.SendError("Cannot set brightness to a value above 100%.");
                            return false;
                        }
                        else if (brightness < 0)
                        {
                            server.MessageSender.SendError("Cannot set brightness to a value below 0%.");
                            return false;
                        }
                        server.MessageSender.SetBrightness(brightness / 100);
                        return true;
                    }
                    server.MessageSender.SendError("Unable to parse brightness level.");
                    return false;
                }),
                // Example: "Please lock my computer."
                new Command("lock.*(pc|computer|machine)", (match) =>
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
                        return true;
                    }
                    server.MessageSender.SendError("Computer platform not supported for remote sesssion locking.");
                    return false;
                })
            );

            server.Start();
        }
    }
}
