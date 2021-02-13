using System.IO;
using Newtonsoft.Json;

namespace RachelBot.Services.Storage
{
    public class JsonStorageService : IStorageService
    {
        public T RestoreObject<T>(string filepath)
        {
            string json = File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void StoreObject(object obj, string filepath)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            File.WriteAllText(filepath, json);
        }

        public bool FileExist(string filepath)
        {
            return File.Exists(filepath);
        }
    }
}
