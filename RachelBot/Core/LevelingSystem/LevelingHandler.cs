using Discord;
using Discord.WebSocket;
using RachelBot.Core.UserAccounts;
using RachelBot.Services.Storage;
using RachelBot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.LevelingSystem
{
    public class LevelingHandler
    {
        private readonly IStorageService _storage;

        public LevelingHandler(IStorageService storage)
        {
            _storage = storage;
        }

        public async void UserSendMessage(SocketGuildUser user)
        {
            UserAccounts.UserAccounts accounts = new UserAccounts.UserAccounts(user.Guild.Id, _storage);
            UserAccount account = accounts.GetUserAccount(user.Id);
            uint oldLevel = account.LevelNumber;

            accounts.AddXP(account, 50UL);

            uint newLevel = account.LevelNumber;

            if (newLevel > oldLevel)
            {
                HandleRewards(user, account);
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

                if (role == null)
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
                        throw;
                    }
                }
            }
        }
    }
}
