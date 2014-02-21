using System.IO;
using LightMemoryDatabase.Api;

namespace Test.Stream
{
    public class StorageWriter : IWriter
    {
        private readonly StreamWriter _sw;

        public StorageWriter(StreamWriter sw)
        {
            _sw = sw;
        }

        public void WriteLine(string value)
        {
            _sw.WriteLine(value);
        }

        public void Close()
        {
            _sw.Close();
        }
    }
}
