using System.Linq;
using LightMemoryDatabase.Api;
using System.Threading.Tasks;
using LightMemoryDatabase.Database.Sources;
using LightMemoryDatabase.Serialization;

namespace LightMemoryDatabase.Database.IO
{
    internal class DatabaseWriter
    {
        private readonly MemorySourceContext _context;
        private readonly IStorageService _storageService;
        private IWriter _writer;
        private JsonSerializer _serializer;
        private DatabaseInfo _databaseInfo;

        public DatabaseWriter(MemorySourceContext context, DatabaseInfo databaseInfo, IStorageService storageService)
        {
            _databaseInfo = databaseInfo;
            _context = context;
            _storageService = storageService;
        }

        public async Task Write()
        {
            try
            {
                await WriteDatabase();
            }
            catch
            {

            }
            finally
            {
                if (_writer != null)
                    _writer.Close();
            }
        }

        private async Task WriteDatabase()
        {
            _writer = await _storageService.GetWriter();
            _serializer = new JsonSerializer();

            WriteDatabaseInfo();
            WriteClassesInfo();
            WriteCollections();
        }

        private void WriteDatabaseInfo()
        {
            _writer.WriteLine(DatabaseConstants.DatabaseInfoKey);
            _writer.WriteLine(_serializer.Serialize(_databaseInfo));
        }

        private void WriteClassesInfo()
        {
            var classesInfo = _context.CollectionTypes.Select(t => new JsonClassInfo(t)).ToList();
            _serializer.Register(classesInfo);

            _writer.WriteLine(DatabaseConstants.CollectionInfoKey);
            _writer.WriteLine(_serializer.Serialize(classesInfo));
        }

        private void WriteCollections()
        {
            foreach (var collectionType in _context.CollectionTypes)
            {
                _writer.WriteLine(DatabaseConstants.CollectionItemsKey);
                _writer.WriteLine(collectionType.Name);
                _writer.WriteLine(_serializer.Serialize(_context.GetCollectionSource(collectionType)));
            }
        }
    }
}
