using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using RachelBot.Core.UserAccounts;
using RachelBot.Services.Storage;
using RachelBot.Utils;

namespace RachelBot.Core.LevelingSystem
{
    public class LevelingHandler
    {
        private readonly IStorageService _storage;

        public LevelingHandler(IStorageService storage)
        {
            _storage = storage;
        }

        public void UserSendMessage(SocketUser user, SocketGuild guild)
        {
            UserAccounts.UserAccounts accounts = new UserAccounts.UserAccounts(guild.Id, _storage);

            UserAccount account = accounts.GetUserAccount(user.Id);

            uint oldLevel = account.LevelNumber;

            accounts.AddXP(account, 50UL);

            SocketGuildUser socketGuildUser = Utility.GetGuildUserById(guild, user.Id);

            if (socketGuildUser is null)
            {
                return;
            }

            uint newLevel = account.LevelNumber;

            if (newLevel > oldLevel)
            {
                HandleRewards(socketGuildUser, account);
            }
        }

        private async void HandleRewards(SocketGuildUser socketGuildUser, UserAccount account)
        {
            foreach (LevelRoleReward reward in new LevelRoleRewards(socketGuildUser.Guild.Id, _storage).GetLevelRoleRewards())
            {
                if (account.LevelNumber < reward.RequiredLevel)
                {
                    continue;
                }

                IRole role = Utility.GetRoleById(socketGuildUser.Guild, reward.RoleId);

                if (role is null)
                {
                    continue;
                }

                if (!socketGuildUser.Roles.Contains(role))
                {
                    try
                    {
                        await socketGuildUser.AddRoleAsync(role);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Program.LogToFile($"ERROR: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }
    }
}
