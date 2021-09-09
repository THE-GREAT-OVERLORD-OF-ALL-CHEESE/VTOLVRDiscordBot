using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Steamworks;

namespace VTOLVRDiscordBot
{
    class Bot
    {

        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var config = new DiscordConfiguration
            {
                Token = "",//put your bot token here, someone find a cleverer solution so noone accidently leaks tokens
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(config);
            //Client.Ready += OnClientReady;

            var commands = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "!!" },
                EnableMentionPrefix = true,
                EnableDms = false
            };

            Commands = Client.UseCommandsNext(commands);
            Commands.RegisterCommands<Commands.VTOLVRCommands>();


            if (SteamAPI.Init())
            {
                Console.WriteLine("Connected to steam!");
                Console.WriteLine(SteamFriends.GetPersonaName());
            }

            Console.WriteLine("Setup commands...");

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
