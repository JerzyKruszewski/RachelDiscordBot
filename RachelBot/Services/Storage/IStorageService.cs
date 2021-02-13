using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Services.Storage
{
    public interface IStorageService
    {
        public bool FileExist(string filePath);

        public T RestoreObject<T>(string filePath);

        public void StoreObject(object obj, string filePath);
    }
}
