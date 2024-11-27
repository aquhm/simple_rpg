namespace Client.Core
{
    public interface IModel
    {
        void Initialize();
        void Reset();
        void Release();
    }
}