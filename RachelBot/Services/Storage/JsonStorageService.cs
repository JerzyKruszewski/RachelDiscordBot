using Newtonsoft.Json;

namespace RachelBot.Services.Storage;

public class JsonStorageService : IStorageService
{
    public void EnsureDirectoryExist(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
    
    public bool FileExist(string filepath)
    {
        return File.Exists(filepath);
    }

    public T? RestoreObject<T>(string filepath)
    {
        string json = File.ReadAllText(filepath);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public void StoreObject(object obj, string filepath)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

        File.WriteAllText(filepath, json);
    }
}
