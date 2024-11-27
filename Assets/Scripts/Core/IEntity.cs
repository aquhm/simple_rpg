namespace Client.Core
{
    public interface IEntity
    {
        string Id { get; }
        void Initialize();
        void Update();
        void Release();
    }
}