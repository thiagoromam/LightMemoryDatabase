using System.Threading.Tasks;
using LightMemoryDatabase.Api;

namespace Test.Database
{
    public class DataContext
    {
        private readonly IContextService _memoryContext;

        public Task<IPlainObjectCollection<Book>> Books
        {
            get { return _memoryContext.GetCollection<Book>(); }
        }
        public Task<IPlainObjectCollection<Author>> Authors
        {
            get { return _memoryContext.GetCollection<Author>(); }
        }
        public Task<IPlainObjectCollection<BooksSerie>> BooksSeries
        {
            get { return _memoryContext.GetCollection<BooksSerie>(); }
        }

        public DataContext(IContextService memoryContext)
        {
            _memoryContext = memoryContext;
        }

        public async void SaveDatabase()
        {
            await _memoryContext.SaveDatabase();
        }
    }
}
