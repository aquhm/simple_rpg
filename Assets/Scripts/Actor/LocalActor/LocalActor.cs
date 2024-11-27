namespace Client.Actor
{
    public class LocalActor : IActor
    {
        public LocalActor(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public ActorType Type => ActorType.LocalPlayer;
    }
}