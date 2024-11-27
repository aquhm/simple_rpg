namespace Client.Actor
{
    public interface IActorController
    {
        bool IsActive { get; }
        void Initialize();
        void Update();
        void Release();
    }
}