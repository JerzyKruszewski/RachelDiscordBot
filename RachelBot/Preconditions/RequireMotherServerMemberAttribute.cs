using Discord;
using Discord.Commands;
using RachelBot.Utils;

namespace RachelBot.Preconditions;

public class RequireMotherServerMemberAttribute : PreconditionAttribute
{
    public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        IUser user = context.User;
        IGuild guild = await context.Client.GetGuildAsync(Utility.MotherServerId);

        await guild.DownloadUsersAsync();

        if ((await guild.GetUserAsync(user.Id)) is not null)
        {
            return PreconditionResult.FromSuccess();
        }

        return PreconditionResult.FromError($"This command is available only for users of Bajarz Development server. {Utility.DiscordInviteLink}");
    }
}

