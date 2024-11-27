using System;

namespace Client.Actor
{
    public class LocalActorModel : IActorModel, IDisposable
    {
        public LocalActorModel(ActorSettings settings)
        {
            StateModel = new ActorStateModel();
            StatusModel = new ActorStatusModel(settings);
            EquipmentStateModel = new ActorEquipmentStateModel();
        }

        public ActorStateModel StateModel { get; }
        public ActorStatusModel StatusModel { get; }
        public ActorEquipmentStateModel EquipmentStateModel { get; }


        public void Dispose()
        {
            StateModel?.Dispose();
            StatusModel?.Dispose();
            EquipmentStateModel?.Dispose();
        }
    }
}
