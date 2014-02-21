using System.Threading.Tasks;

namespace LightMemoryDatabase.Api
{
    public interface IStorageService
    {
        Task<bool> DatabaseExists();
        Task<IWriter> GetWriter();
        Task<IReader> GetReader();
    }
}
