namespace Client.Actor
{
    public enum ActorType
    {
        None,
        LocalPlayer,
        RemotePlayer,
        Enemy,
        NPC,
        Vehicle
    }

    public enum ActorState
    {
        None,
        Idle,
        Moving,
        Jumping,
        Falling,
        Attacking,
        Defending,
        UsingVehicle,
        Dead
    }

    public interface IActor
    {
        string Id { get; }
        ActorType Type { get; }
    }
}