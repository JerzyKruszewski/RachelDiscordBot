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

namespace RachelBot.Commands
{
    [Group("Staff")]
    public class ModerationCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public ModerationCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("Ban")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task BanUser(SocketGuildUser user, [Remainder]string reason)
        {
            await user.BanAsync(0, reason);
        }

        [Command("Kick")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task KickUser(SocketGuildUser user, [Remainder]string reason)
        {
            await user.KickAsync(reason);
        }

        [Command("Praise")]
        [Alias("Pochwal")]
        [RequireStaff]
        public async Task PraiseUser(SocketGuildUser user, [Remainder]string reason)
        {
            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.AddPraise(accounts.GetUserAccount(user.Id), Context.User.Id, reason);
        }

        [Command("Warn")]
        [Alias("Ostrzeżenie")]
        [RequireStaff]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task WarnUser(SocketGuildUser user, [Remainder] string reason)
        {
            GuildConfig config = new GuildConfigs(Context.Guild.Id, _storage).GetGuildConfig();
            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);

            accounts.DeleteExpiredWarnings(account);
            accounts.AddWarning(account, reason, config);

            if ((account.Warnings.Count >= config.WarnCountTillBan && config.WarnCountTillBan > 0) ||
                (account.Warnings.Sum(w => w.Value) >= config.WarnPointsTillBan && config.WarnPointsTillBan > 0))
            {
                await user.BanAsync(0, $"Too many warnings!\nLast warning: {reason}");
            }
            else if ((account.Warnings.Count >= config.WarnCountTillPunishment && config.WarnCountTillPunishment > 0) ||
                     (account.Warnings.Sum(w => w.Value) >= config.WarnPointsTillPunishment && config.WarnPointsTillPunishment > 0))
            {
                await user.AddRoleAsync(Utility.GetRoleById(Context.Guild, config.PunishmentRoleId));
            }

            await Context.Channel.SendMessageAsync("User warned!");
        }

        [Command("Archievement")]
        [Alias("Osiągnięcie")]
        [RequireStaff]
        public async Task Archievement(SocketGuildUser user, [Remainder]string archievement)
        {
            UserAccounts accounts = new UserAccounts(Context.Guild.Id, _storage);

            accounts.AddAchievement(accounts.GetUserAccount(user.Id), archievement);
        }
    }
}
