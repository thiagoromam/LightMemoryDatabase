using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightMemoryDatabase.Api;
using System.Diagnostics;

namespace LightMemoryDatabase.Database
{
    internal sealed class MemoryContextService : IContextService
    {
        private UpdateManager _updateManager;
        private DatabaseReader _databaseReader;
        private DatabaseWriter _databaseWriter;
        private Dictionary<Type, IItemCollection> _collections;
        private bool _databaseLoaded;

        public MemoryContextService(UpdateManager updateManager, DatabaseReader databaseReader, DatabaseWriter databaseWriter)
        {
            _updateManager = updateManager;
            _databaseReader = databaseReader;
            _databaseWriter = databaseWriter;
            _collections = new Dictionary<Type, IItemCollection>();
        }

        private async Task LoadDatabase()
        {
            _collections = await _databaseReader.Read(_updateManager);
            _databaseLoaded = true;
        }

        public async Task<IItemCollection<T>> GetCollection<T>() where T : class, IItem
        {
            if (!_databaseLoaded)
                await LoadDatabase();

            var type = typeof(T);
            if (!_collections.ContainsKey(type))
                _collections.Add(type, new ItemCollection<T>(new List<T>()));

            return (IItemCollection<T>)_collections[type];
        }

        public async Task SaveDatabase()
        {
            if (_databaseReader.LoadedCorrectly)
                await _databaseWriter.Write(_collections);
        }

        public void AddUptade(IDatabaseUpdate update)
        {
            _updateManager.AddUpdate(update);
        }
    }
}
