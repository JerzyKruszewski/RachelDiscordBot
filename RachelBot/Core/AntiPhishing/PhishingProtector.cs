using RachelBot.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Core.AntiPhishing
{
    public static class PhishingProtector
    {
        private static readonly IStorageService _storage = new JsonStorageService();
        private static readonly PhishingSites _phishingSites = _storage.RestoreObject<PhishingSites>("./Core/AntiPhishing/domain-list.json");

        public static bool IsDangerous(string message)
        {
            message = message.Trim().ToLower();

            foreach (string domain  in _phishingSites.Domains)
            {
                if (message.Contains(domain))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
