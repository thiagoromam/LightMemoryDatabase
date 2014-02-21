using LightMemoryDatabase.Api;

namespace Test.Database
{
    public class Author : IPlainObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
