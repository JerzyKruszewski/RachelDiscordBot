using RachelBot.Core.Configs;
using RachelBot.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.UserAccounts
{
    public class UserAccounts
    {
        private readonly IStorageService _storage;
        private readonly string _folderPath;
        private readonly string _filePath;
        private readonly IList<UserAccount> _accounts;

        public UserAccounts(ulong id, IStorageService storage)
        {
            _storage = storage;
            _folderPath = $"./Guilds/{id}";

            _storage.EnsureDirectoryExist(_folderPath);

            _filePath = $"{_folderPath}/UserAccounts.json";

            if (_storage.FileExist(_filePath))
            {
                _accounts = _storage.RestoreObject<List<UserAccount>>(_filePath);
            }
            else
            {
                _accounts = new List<UserAccount>();
                Save();
            }
        }

        private void Save()
        {
            _storage.StoreObject(_accounts, _filePath);
        }

        public UserAccount GetUserAccount(ulong id)
        {
            UserAccount account = _accounts.SingleOrDefault(u => u.Id == id);

            if (account == null)
            {
                return CreateAccount(id);
            }

            return account;
        }

        private UserAccount CreateAccount(ulong id)
        {
            UserAccount account = new UserAccount()
            {
                Id = id
            };

            _accounts.Add(account);
            Save();

            return account;
        }

        public void AddXP(UserAccount account, ulong exp)
        {
            account.XP += exp;
            Save();
        }

        public void AddPraise(UserAccount account, ulong giverId, string reason)
        {
            account.Praises.Add(Praises.CreatePraise(giverId, Praises.GetNextId(account.Praises), reason));
            Save();
        }

        public void AddWarning(UserAccount account, string reason, GuildConfig config)
        {
            if (config.PointBasedWarns)
            {
                string[] words = reason.Split(' ');
                int value = int.Parse(reason.Split(' ')[0]);
                reason = words[1];

                for (int i = 2; i < words.Length; i++)
                {
                    reason += $" {words[i]}";
                }

                account.Warnings.Add(Warnings.CreateWarning(Warnings.GetNextId(account.Warnings), value, reason, config.WarnDuration));
            }
            else
            {
                account.Warnings.Add(Warnings.CreateWarning(Warnings.GetNextId(account.Warnings), reason, config.WarnDuration));
            }

            Save();
        }

        public void RemoveWarning(UserAccount account, int warnId)
        {
            account.Warnings.Remove(account.Warnings.SingleOrDefault(w => w.Id == warnId));

            Save();
        }

        public void DeleteExpiredWarnings(UserAccount account)
        {
            try
            {
                for (int i = account.Warnings.Count - 1; i >= 0; i--)
                {
                    if (DateTime.Compare(DateTime.Now, account.Warnings[i].ExpireDate) >= 0)
                    {
                        account.Warnings.RemoveAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Save();
        }

        public void AddAchievement(UserAccount account, string achievement)
        {
            account.Archievements.Add(achievement);
            Save();
        }
    }
}
