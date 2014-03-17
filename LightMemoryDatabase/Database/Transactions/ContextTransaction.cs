using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightMemoryDatabase.Api;

namespace LightMemoryDatabase.Database.Transactions
{
    internal class ContextTransaction : IContextTransaction
    {
        private readonly ICollectionManager _collectionManager;
        private List<PlainObjectState> _states;

        public ContextTransaction(ICollectionManager collectionManager)
        {
            _collectionManager = collectionManager;
        }

        public async Task Open()
        {
            var task = new Task<List<PlainObjectState>>(() =>
            {
                var plainObjects = _collectionManager.Collections.SelectMany(c => c.Cast<IPlainObject>());
                return plainObjects.Select(p => new PlainObjectState(p)).ToList();
            });
            task.Start();
            _states = await task;
        }

        public Task Rollback()
        {
            var task = new Task(() =>
            {
                var statesToRestore = _states.Where(s => s.HasChanges());

                foreach (var state in statesToRestore)
                    state.Restore();
            });
            task.Start();
            return task;
        }
    }
}
