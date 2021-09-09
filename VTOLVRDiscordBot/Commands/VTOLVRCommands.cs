using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Steamworks;

namespace VTOLVRDiscordBot.Commands
{
    public class VTOLVRCommands : BaseCommandModule
    {
        public Callback<LobbyMatchList_t> lobbyList;
        public CommandContext ctx;

        [Command("vtol")]
        public async Task VTOL(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("fetching lobbies...").ConfigureAwait(false);

            this.ctx = ctx;
            if (lobbyList == null)
            {
                lobbyList = Callback<LobbyMatchList_t>.Create(OnSteamLobbyList);
                Console.WriteLine("Setup callback!");
            }

            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
            SteamMatchmaking.RequestLobbyList();
            Console.WriteLine("Requested Lobbies!");
        }

        [Command("runcallback")]
        public async Task RunCallback(CommandContext ctx)
        {
            SteamAPI.RunCallbacks();//this should not be a sererate chat command, this should be automatically called one every second at least
        }

        public void OnSteamLobbyList(LobbyMatchList_t callback)
        {
            Console.WriteLine("Recieved Lobbies!");
            string message = "Lobbies: ";
            for (int i = 0; i < callback.m_nLobbiesMatching; i++)
            {
                CSteamID lobby = SteamMatchmaking.GetLobbyByIndex(i);

                string lobbyName = SteamMatchmaking.GetLobbyData(lobby, "lName");
                if (lobbyName != "")
                {//make sure the lobby isnt garbage
                    string playerName = SteamMatchmaking.GetLobbyData(lobby, "oName");
                    string mapName = SteamMatchmaking.GetLobbyData(lobby, "scn");
                    int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobby);
                    int maxPlayers = int.Parse(SteamMatchmaking.GetLobbyData(lobby, "maxP"));

                    Console.WriteLine($"{playerName}'s {lobbyName} lobby in {mapName} has {playerCount}/{maxPlayers} players.");

                    message += "\n";
                    message += $"{playerName}'s {lobbyName} lobby in {mapName} has {playerCount}/{maxPlayers} players.";
                }
            }
            VTOLReply(ctx, message);
        }

        public async Task VTOLReply(CommandContext ctx, string message)
        {

            lobbyList = Callback<LobbyMatchList_t>.Create(OnSteamLobbyList);

            await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}
