using Client.Actor.Animation;
using Client.Camera;
using Client.Core;
using Client.Service;

namespace Client.Actor
{
    public class LocalActorPresenter : BaseActorPresenter
    {
        private readonly CameraService _cameraService;

        public LocalActorPresenter(IActorView view, LocalActorModel model) : base(view, model)
        {
            LocalModel = model;
            _cameraService = GameApplication.Instance.Services.GetService<CameraService>();
        }

        public LocalActorModel LocalModel { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            _cameraService.SetCameraType(GameViewType.ThirdPerson);
        }

        protected override void InitializeControllers()
        {
            RegisterController(new LocalMovementController(this, _view));
            RegisterController(new LocalCombatController(this, _view));
            RegisterController(new ActorAnimationController(this, _view.Animator));
            RegisterController(new LocalActorEquipmentController(this, _view));
            RegisterController(new LocalAnimationEventController(this));

            foreach (var controller in _controllers.Values)
            {
                controller.Initialize();
            }
        }

        private IActorController RegisterController(IActorController actorController)
        {
            var type = actorController.GetType();
            if (_controllers.ContainsKey(type) == false)
            {
                _controllers[type] = actorController;
            }

            return actorController;
        }
    }
}
