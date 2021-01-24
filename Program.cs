using System;
using System.IO;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.Gateway;
using Leaf.xNet;
using Serilog;
using Serilog.Core;

namespace EmojiDumper
{
    internal static class Program
    {
        private static DiscordSocketClient client = new();
        private static Logger log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        public static void Main(string[] args)
        {
            Console.Write("Discord Token: ");
            var token = Console.ReadLine();
            client.OnLoggedIn += (_, eArgs) => log.Information($"Successfully logged in as {eArgs.User}!");
            client.OnLoggedOut += (_, eArgs) => log.Information($"Logged out of User, reason: {eArgs.Reason}!");
            client.CreateCommandHandler("e!");
            client.Login(token);
        }

        [Command("dump", "Dumps all emojis in a guild.")]
        public class DumpCommand : CommandBase
        {
            public override void Execute()
            {
                Message.Delete();
                var path = $"{Environment.CurrentDirectory}/{Message.Guild.Id}";
                Directory.CreateDirectory(path);
                var req = new HttpRequest {Cookies = new CookieStorage(), Authorization = client.Token, KeepAlive = true, UserAgent = Http.RandomUserAgent(), IgnoreProtocolErrors = true, ConnectTimeout = 10000};
                Message.Guild.GetEmojis().ToList().ForEach(delegate(DiscordEmoji emoji)
                {
                    req.Get(emoji.Icon.Url).ToFile(emoji.Animated ? $"{path}/{emoji.Name}.gif" : $"{path}/{emoji.Name}.png");
                    Log.Information($"Dumped Emoji: {emoji.Name} - Animated: {emoji.Animated}");
                });
                log.Information($"Done, downloaded all emojis to {path}!");
            }
        }
    }
}