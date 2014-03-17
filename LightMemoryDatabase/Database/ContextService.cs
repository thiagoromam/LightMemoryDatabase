using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightMemoryDatabase.Api;
using LightMemoryDatabase.Database.IO;
using LightMemoryDatabase.Database.Sources;
using LightMemoryDatabase.Database.Transactions;

namespace LightMemoryDatabase.Database
{
    internal sealed class ContextService : IContextService, ICollectionManager
    {
        private readonly MemorySourceContext _context;
        private readonly DatabaseReader _databaseReader;
        private readonly DatabaseWriter _databaseWriter;
        private readonly Dictionary<Type, IPlainObjectCollection> _collections;
        private bool _databaseLoaded;
        private bool _loadingDatabase;

        public IEnumerable<IPlainObjectCollection> Collections
        {
            get { return _collections.Values; }
        }

        public ContextService(MemorySourceContext context, DatabaseReader databaseReader, DatabaseWriter databaseWriter)
        {
            _context = context;
            _databaseReader = databaseReader;
            _databaseWriter = databaseWriter;
            _collections = new Dictionary<Type, IPlainObjectCollection>();
        }

        private async Task LoadDatabase()
        {
            if (!_loadingDatabase && !_databaseLoaded)
            {
                _loadingDatabase = true;
                await _databaseReader.Read();
                _databaseLoaded = true;
                _loadingDatabase = false;
            }

            while (_loadingDatabase) { }
        }
        
        public async Task<IPlainObjectCollection<T>> GetCollection<T>() where T : class, IPlainObject
        {
            if (!_databaseLoaded)
                await LoadDatabase();

            var type = typeof(T);
            lock (_collections)
            {
                if (!_collections.ContainsKey(type))
                    _collections.Add(type, new PlainObjectCollection<T>(_context.GetCollectionSource(type)));
            }

            return (IPlainObjectCollection<T>)_collections[type];
        }
        
        public async Task SaveDatabase()
        {
            if (_databaseReader.LoadedCorrectly)
                await _databaseWriter.Write();
        }

        public async Task<IContextTransaction> OpenTransaction()
        {
            var transaction = new ContextTransaction(this);
            await transaction.Open();

            return transaction;
        }
    }
}
