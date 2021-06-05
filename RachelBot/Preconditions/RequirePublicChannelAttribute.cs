using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace RachelBot.Preconditions
{
    public class RequirePublicChannelAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is not IPrivateChannel)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError($"This command is reserved for public channels"));
            }
        }
    }
}
