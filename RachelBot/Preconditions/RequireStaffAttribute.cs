using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord.Commands;
using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using RachelBot.Core.StaffRoles;
using RachelBot.Lang;

namespace RachelBot.Preconditions;

public class RequireStaffAttribute : PreconditionAttribute
{
    private readonly StaffPermissionType _minPermType;

    public RequireStaffAttribute(StaffPermissionType minPermType)
    {
        _minPermType = minPermType;
    }

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        SocketGuildUser user = context.User as SocketGuildUser;

        if (user.GuildPermissions.Administrator)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        GuildConfig config = new GuildConfigs(context.Guild.Id, services.GetService<IStorageService>()).GetGuildConfig();

        foreach (StaffRole role in config.StaffRoles)
        {
            ulong roleId = role.Id;
            if (user.Roles.SingleOrDefault(r => r.Id == roleId) is not null && CheckPermissions(role.PermissionType))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
        }

        AlertsHandler alerts = new AlertsHandler(config);

        return Task.FromResult(PreconditionResult.FromError(alerts.GetAlert("INSUFFICIENT_PERMISSIONS")));
    }

    private bool CheckPermissions(StaffPermissionType permissionType)
    {
        return _minPermType switch
        {
            StaffPermissionType.Helper => true,
            StaffPermissionType.JuniorModerator => permissionType.HasFlag(StaffPermissionType.Admin) ||
                                                   permissionType.HasFlag(StaffPermissionType.JuniorAdmin) ||
                                                   permissionType.HasFlag(StaffPermissionType.Moderator) ||
                                                   permissionType.HasFlag(StaffPermissionType.JuniorModerator),
            StaffPermissionType.Moderator => permissionType.HasFlag(StaffPermissionType.Admin) ||
                                             permissionType.HasFlag(StaffPermissionType.JuniorAdmin) ||
                                             permissionType.HasFlag(StaffPermissionType.Moderator),
            StaffPermissionType.JuniorAdmin => permissionType.HasFlag(StaffPermissionType.Admin) ||
                                               permissionType.HasFlag(StaffPermissionType.JuniorAdmin),
            StaffPermissionType.Admin => permissionType.HasFlag(StaffPermissionType.Admin),
            _ => false,
        };
    }
}
