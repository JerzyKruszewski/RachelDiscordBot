using System.Threading.Tasks;
using Discord.Commands;
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
    }
}
