using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System.Collections.Generic;
using RachelBot.Utils;

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

        [Command("BotStatus", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task GetStatus()
        {
            List<ulong> userList = new List<ulong>();

            foreach (SocketGuild guild in Context.Client.Guilds)
            {
                await guild.DownloadUsersAsync();

                foreach (SocketGuildUser user in guild.Users)
                {
                    if (!user.IsBot && !userList.Contains(user.Id))
                    {
                        userList.Add(user.Id);
                    }
                }
            }

            await Context.Channel.SendMessageAsync($"Guilds: {Context.Client.Guilds.Count}\nUsers: {userList.Count}");
        }

        [Command("CheckOwnerships")]
        [RequireOwner]
        public async Task CheckOwnerships()
        {
            Dictionary<ulong, int> ownerships = new Dictionary<ulong, int>();

            foreach (SocketGuild guild in Context.Client.Guilds)
            {
                if (ownerships.ContainsKey(guild.OwnerId))
                {
                    ownerships[guild.OwnerId]++;
                }
                else
                {
                    ownerships.Add(guild.OwnerId, 1);
                }
            }

            string message = "";

            foreach (KeyValuePair<ulong, int> ownership in ownerships)
            {
                message += $"{ownership.Key}:{ownership.Value}";
            }

            await Context.Channel.SendMessageAsync(message);
        }

        [Command("DebugGuildConfig")]
        [RequireOwner]
        public async Task DebugConfig()
        {
            IDMChannel channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendFileAsync($"./Guilds/{Context.Guild.Id}/Config.json");
        }

        [Command("CommunicateWithOwners", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task CommunicateWithOwners([Remainder]string message)
        {
            IDMChannel channel;

            foreach (SocketGuild guild in Context.Client.Guilds)
            {
                try
                {
                    channel = await guild.Owner.GetOrCreateDMChannelAsync();
                    await channel.SendMessageAsync($"{message}\n\n{Utility.OwnerSignature}");
                    await Task.Delay(1000);
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            await Context.Channel.SendMessageAsync("Done");
        }
    }
}
