namespace RachelBot.Services.Storage;

public interface IStorageService
{
    public void EnsureDirectoryExist(string folderPath);

    public bool FileExist(string filePath);

    public T RestoreObject<T>(string filePath);

    public void StoreObject(object obj, string filePath);
}
