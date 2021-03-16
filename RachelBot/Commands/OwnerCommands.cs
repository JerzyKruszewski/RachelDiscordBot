using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;

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

        [Command("BotStatus")]
        [RequireOwner]
        public async Task GetStatus()
        {
            int users = 0;

            foreach (SocketGuild guild in Context.Client.Guilds)
            {
                await guild.DownloadUsersAsync();

                users += guild.DownloadedMemberCount;
            }

            await Context.Channel.SendMessageAsync($"Guilds: {Context.Client.Guilds.Count}\nUsers: {users}");
        }

        [Command("DebugGuildConfig")]
        [RequireOwner]
        public async Task DebugConfig()
        {
            IDMChannel channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendFileAsync($"./Guilds/{Context.Guild.Id}/Config.json");
        }
    }
}
