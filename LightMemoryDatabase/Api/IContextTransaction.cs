using System.Threading.Tasks;

namespace LightMemoryDatabase.Api
{
    public interface IContextTransaction
    {
        Task Rollback();
    }
}