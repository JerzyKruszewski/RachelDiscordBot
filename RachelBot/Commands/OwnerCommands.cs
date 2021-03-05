using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Preconditions;
using RachelBot.Core.UserAccounts;
using RachelBot.Utils;
using RachelBot.Core.LevelingSystem;
using RachelBot.Lang;

namespace RachelBot.Commands
{
    public class OwnerCommands : InteractiveBase<SocketCommandContext>
    {
        [Command("Guilds")]
        [RequireOwner]
        public async Task GetGuilds()
        {
            await Context.Channel.SendMessageAsync($"Guilds: {Context.Client.Guilds.Count}");
        }
    }
}
