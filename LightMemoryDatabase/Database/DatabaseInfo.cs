using LightMemoryDatabase.Api;
namespace LightMemoryDatabase.Database
{
    internal class DatabaseInfo
    {
        public int Version { get; set; }
        public int CurrentPlainObjectId { get; set; }

        public DatabaseInfo()
        {
            Version = 1;
            CurrentPlainObjectId = 1;
        }

        [Initalize]
        private void Initialize()
        {
            var instance = DependencyRegistry.Resolve<DatabaseInfo>();
            instance.Version = Version;
            instance.CurrentPlainObjectId = CurrentPlainObjectId;
        }
    }
}
