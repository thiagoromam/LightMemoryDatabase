namespace LightMemoryDatabase.Api
{
    public interface IWriter
    {
        void WriteLine(string value);
        void Close();
    }
}
