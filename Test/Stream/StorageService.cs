using System.IO;
using System.Threading.Tasks;
using LightMemoryDatabase.Api;

namespace Test.Stream
{
    public class StorageService : IStorageService
    {
        private const string Connection = "D:\\books_database.txt";

        public Task<bool> DatabaseExists()
        {
            return Task.Factory.StartNew(() => File.Exists(Connection));
        }

        public Task<IWriter> GetWriter()
        {
            return Task<IWriter>.Factory.StartNew(() => new StorageWriter(new StreamWriter(Connection)));
        }

        public Task<IReader> GetReader()
        {
            return Task<IReader>.Factory.StartNew(() => new StorageReader(new StreamReader(Connection)));
        }
    }
}
