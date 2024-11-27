using UniRx;

namespace Client.Actor
{
    public class LocalAnimationEventController : IActorController
    {
        private readonly LocalActorPresenter _actorPresenter;
        private readonly CompositeDisposable _disposables = new();
        private readonly LocalActorModel _localModel;
        private LocalActorEquipmentController _equipmentController;

        public LocalAnimationEventController(LocalActorPresenter actorPresenter)
        {
            _actorPresenter = actorPresenter;
            _localModel = _actorPresenter.LocalModel;
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _equipmentController = _actorPresenter.GetController<LocalActorEquipmentController>();

            MessageBroker.Default
                         .Receive<AnimationEventMessage>()
                         .Where(_ => IsActive)
                         .Subscribe(HandleAnimationEvent)
                         .AddTo(_disposables);

            IsActive = true;
        }

        public void Update()
        {
        }

        public void Release()
        {
            _disposables?.Dispose();
            IsActive = false;
        }

        private void HandleAnimationEvent(AnimationEventMessage message)
        {
            // 이벤트 종류에 따라 적절한 컨트롤러로 전달
            if (message.EventKey.StartsWith("Equipment"))
            {
                switch (message.StringValue)
                {
                    case "ActiveSword":
                        _localModel.EquipmentStateModel.SetEquipmentState(EquipmentType.Sword, EquipmentAction.Active);
                        break;
                    case "ActiveShield":
                        _localModel.EquipmentStateModel.SetEquipmentState(EquipmentType.Shield, EquipmentAction.Active);
                        break;
                    case "DeactiveSword":
                        _localModel.EquipmentStateModel.SetEquipmentState(EquipmentType.Sword, EquipmentAction.Deactive);
                        break;
                    case "DeactiveShield":
                        _localModel.EquipmentStateModel.SetEquipmentState(EquipmentType.Shield, EquipmentAction.Deactive);
                        break;
                }
            }
        }
    }
}
