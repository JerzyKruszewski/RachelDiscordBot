using Discord.Commands;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;

namespace RachelBot.Preconditions
{
    public class RequireStaffAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            GuildConfig config = new GuildConfigs(context.Guild.Id, services.GetService<IStorageService>()).GetGuildConfig();

            foreach (ulong roleId in config.StaffRoleIds)
            {
                if ((context.User as SocketGuildUser).Roles.SingleOrDefault(r => r.Id == roleId) != null)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }

            return Task.FromResult(PreconditionResult.FromError($"You are not server staff"));
        }
    }
}
