using System.Threading.Tasks;

namespace LightMemoryDatabase.Api
{
    public interface IContextService
    {
        Task<IPlainObjectCollection<T>> GetCollection<T>() where T : class, IPlainObject;
        Task SaveDatabase();
    }
}
