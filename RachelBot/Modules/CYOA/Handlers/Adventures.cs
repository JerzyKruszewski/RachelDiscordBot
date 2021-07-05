using RachelBot.Modules.CYOA.Objects;
using RachelBot.Services.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RachelBot.Modules.CYOA.Handlers
{
    public class Adventures
    {
        private static readonly IStorageService _storage;
        private static readonly string _folderPath;
        private static readonly IList<Adventure> _adventures;

        static Adventures()
        {
            _storage = new JsonStorageService();

            _folderPath = "./Modules/CYOA/Stories";

            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }

            _adventures = GetAdventures();
        }

        public static IList<Adventure> GetAdventures()
        {
            IList<Adventure> adventureFiles = new List<Adventure>();

            foreach (string file in Directory.GetFiles(_folderPath))
            {
                if (!file.EndsWith(".json"))
                {
                    continue;
                }

                adventureFiles.Add(_storage.RestoreObject<Adventure>(file));
            }

            return adventureFiles;
        }

        public static void Save(Adventure adventure)
        {
            _storage.StoreObject(adventure, $"{_folderPath}/{adventure.Code}.json");
        }

        public static Adventure GetAdventure(int index)
        {
            return _adventures.ElementAtOrDefault(index);
        }

        public static Adventure GetAdventure(string code)
        {
            return _adventures.SingleOrDefault(a => a.Code == code);
        }

        public static Adventure CreateAdventure(string author, string name, string language, int minimalAge,
                                                string mcName = "", bool? isMCFemale = null)
        {
            if (_adventures.FirstOrDefault(a => a.Author == author && a.Name == name) is not null)
            {
                throw new ArgumentException($"There already is adventure named: {name}");
            }

            Adventure adventure = new Adventure()
            {
                Id = _adventures.Count > 0 ? _adventures.Max(a => a.Id) + 1 : 1,
                Author = author,
                Name = name,
                Language = language,
                MCName = mcName,
                IsMCFemale = isMCFemale,
                MinimalAge = minimalAge,
                Pages = new List<Page>()
            };

            _adventures.Add(adventure);
            Save(adventure);

            return adventure;
        }
    }
}
