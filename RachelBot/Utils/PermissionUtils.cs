using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RachelBot.Utils
{
    public class PermissionUtils
    {
        public static bool CanBanMembers(SocketCommandContext context)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            return (permissions.BanMembers || permissions.Administrator); 
        }

        public static bool CanGiveRoles(SocketCommandContext context)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            return (permissions.ManageRoles || permissions.Administrator);
        }

        public static async void TryBanUserAsync(SocketCommandContext context, IUser user, string reason)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            if (permissions.BanMembers || permissions.Administrator)
            {
                await context.Guild.AddBanAsync(user, 0 , reason);
            }
        }

        public static async void TryGiveUserRoleAsync(SocketCommandContext context, SocketGuildUser user, IRole role)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            if (permissions.ManageRoles || permissions.Administrator)
            {
                await user.AddRoleAsync(role);
            }
        }

        public static async void TryRemoveMessageAsync(SocketCommandContext context)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            if (permissions.ManageMessages || permissions.Administrator)
            {
                await context.Message.DeleteAsync();
            }
        }

        public static async void TryRemoveMessageAsync(SocketCommandContext context, SocketMessage message)
        {
            GuildPermissions permissions = context.Guild.CurrentUser.GuildPermissions;

            if (permissions.ManageMessages || permissions.Administrator)
            {
                await message.DeleteAsync();
            }
        }
    }
}
