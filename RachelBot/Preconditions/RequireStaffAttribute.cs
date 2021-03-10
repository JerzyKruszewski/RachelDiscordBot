using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord.Commands;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;

namespace RachelBot.Preconditions
{
    public class RequireStaffAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            SocketGuildUser user = context.User as SocketGuildUser;

            if (user.GuildPermissions.Administrator)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            GuildConfig config = new GuildConfigs(context.Guild.Id, services.GetService<IStorageService>()).GetGuildConfig();

            foreach (ulong roleId in config.StaffRoleIds)
            {
                if (user.Roles.SingleOrDefault(r => r.Id == roleId) != null)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }

            return Task.FromResult(PreconditionResult.FromError($"You are not server staff"));
        }
    }
}
