namespace LightMemoryDatabase.Api
{
    public interface IReader
    {
        string Line { get; }
        bool NextLine();
        void Close();
    }
}
