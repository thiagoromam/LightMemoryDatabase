using System.IO;
using LightMemoryDatabase.Api;

namespace Test.Stream
{
    public class StorageReader : IReader
    {
        private readonly StreamReader _sr;
        public string Line { get; private set; }

        public StorageReader(StreamReader sr)
        {
            _sr = sr;
        }

        public bool NextLine()
        {
            Line = _sr.ReadLine();
            return Line != null;
        }

        public void Close()
        {
            _sr.Close();
        }
    }
}