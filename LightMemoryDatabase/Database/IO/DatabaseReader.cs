using LightMemoryDatabase.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightMemoryDatabase.Database.References;
using LightMemoryDatabase.Database.Sources;
using LightMemoryDatabase.Serialization;

namespace LightMemoryDatabase.Database.IO
{
    internal class DatabaseReader
    {
        private readonly MemorySourceContext _context;
        private readonly IStorageService _storageService;
        private IReader _reader;
        private readonly JsonDeserializer _deserializer;
        private IEnumerable<JsonClassInfo> _classesInfo;
        public bool LoadedCorrectly { get; private set; }

        public DatabaseReader(MemorySourceContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
            _deserializer = new JsonDeserializer(new ReferenceManager(context));
        }

        public async Task Read()
        {
            try
            {
                if (await _storageService.DatabaseExists())
                    await ReadDatabase();

                LoadedCorrectly = true;
            }
            catch
            {
                LoadedCorrectly = false;
            }
            finally
            {
                if (_reader != null)
                    _reader.Close();
            }
        }

        private async Task ReadDatabase()
        {
            _reader = await _storageService.GetReader();

            while (_reader.NextLine())
            {
                Action method;
                switch (_reader.Line)
                {
                    case DatabaseConstants.DatabaseInfoKey:
                        method = ReadDatabaseInfo;
                        break;

                    case DatabaseConstants.CollectionInfoKey:
                        method = ReadCollectionInfo;
                        break;

                    case DatabaseConstants.CollectionItemsKey:
                        method = ReadCollection;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                _reader.NextLine();
                method();
            }
        }

        private void ReadDatabaseInfo()
        {
            _deserializer.Deserialize<DatabaseInfo>(_reader.Line);
        }

        private void ReadCollectionInfo()
        {
            _classesInfo = _deserializer.Deserialize<IEnumerable<JsonClassInfo>>(_reader.Line);
            _deserializer.Register(_classesInfo);
        }

        private void ReadCollection()
        {
            var typeName = _reader.Line;
            _reader.NextLine();

            var classType = _deserializer.GetClassInfo(typeName).ClassType;
            var collectionType = typeof(IEnumerable<>).MakeGenericType(classType);
            var enumerable = (IEnumerable<IPlainObject>)_deserializer.Deserialize(collectionType, _reader.Line);

            _context.AddCollectionSource(classType, enumerable);
        }
    }
}
