namespace Client.Service
{
    public interface IService
    {
        bool IsActive { get; }
        void Initialize();
        void DoUpdate(float deltaTime);
        void Release();
    }
}