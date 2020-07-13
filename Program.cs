using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Console = Colorful.Console;
using Discord;
using Discord.Gateway;
using Discord.Commands;
using Leaf.xNet;

namespace EmojiDumper
{
    internal class Program
    {
        private static DiscordSocketClient client = new DiscordSocketClient();
        public static void Main(string[] args)
        {
            Console.Write("> [TOKEN]: ", Color.Yellow);
            var token = Console.ReadLine();
            client.OnLoggedIn += (socketClient, eventArgs) => Console.WriteLine($"Logged in as {eventArgs.User.Username}!");
            client.Token = token;
            client.Login(client.Token);
            client.CreateCommandHandler("e!");
            Thread.Sleep(-1);
        }
        [Command("dump", "Dumps all emojis in a guild")]
        private class  Dump : ICommand
        {
            public void Execute(DiscordSocketClient client, DiscordMessage message)
            {
                message.Delete();
                var path = $"{Environment.CurrentDirectory}/{message.Guild.Id}";
                if (Directory.Exists(path))
                    Directory.Delete(path);
                Directory.CreateDirectory(path);
                var req = new HttpRequest {Cookies = new CookieStorage(), Authorization = client.Token, KeepAlive = true, UserAgent = Http.RandomUserAgent(), IgnoreProtocolErrors = true};
                foreach (var emoji in message.Guild.GetEmojis())
                {
                    if (!emoji.Available) continue;
                    var p = emoji.Animated ? ".gif" : ".png";
                    try
                    {
                        req.Get(emoji.Icon.Url).ToFile($"{path}/{emoji.Name}{p}");
                        Console.WriteLine($"Dumped {emoji.Name}{p}!", Color.Green);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Couldn't dump {emoji.Name}{p}!", Color.Red);
                    }
                }
                Console.WriteLine("> [DONE]: Dumped all emojis in guild!", Color.Fuchsia);
            }
        }
    }
}