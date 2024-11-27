// using System.Collections.Generic;
// using System.Linq;
// using UniRx;
//
// namespace Client.Actor
// {
//     public class ActorPresenter : IActorPresenter
//     {
//         private readonly List<IActorController> _controllers = new();
//         private readonly CompositeDisposable _disposables = new();
//         private readonly IActorModel _model;
//         private readonly IActorView _view;
//
//         public ActorPresenter(IActorView view, IActorModel model)
//         {
//             _view = view;
//             _model = model;
//         }
//
//         public void Initialize()
//         {
//             InitializeControllers();
//             SetupBindings();
//         }
//
//         public void Update()
//         {
//             foreach (var controller in _controllers.Where(controller => controller.IsActive)) controller.Update();
//         }
//
//         public void Release()
//         {
//             _disposables.Dispose();
//             foreach (var controller in _controllers) controller.Release();
//             _controllers.Clear();
//         }
//
//         private void InitializeControllers()
//         {
//             // InputController를 먼저 생성하고
//             var inputController = new ActorInputController();
//             inputController.Initialize();
//
//             // MovementController에 InputController 전달
//             var movementController = new ActorMovementController(_model, _view, inputController);
//
//             _controllers.Add(inputController);
//             _controllers.Add(movementController);
//
//             // MovementController 초기화
//             movementController.Initialize();
//         }
//
//         private void SetupBindings()
//         {
//             // View와 Model 바인딩
//             _model.Position
//                     .Subscribe(pos => _view.Transform.position = pos)
//                     .AddTo(_disposables);
//
//             _model.State
//                     .Subscribe(UpdateVisuals)
//                     .AddTo(_disposables);
//
//             // 추가적인 Model 타입별 바인딩
//             if (_model is LocalActorModel localModel)
//                 SetupLocalActorBindings(localModel);
//             else if (_model is RemoteActorModel remoteModel) SetupRemoteActorBindings(remoteModel);
//         }
//
//         private void SetupLocalActorBindings(LocalActorModel model)
//         {
//             model.Health
//                     .Subscribe(UpdateHealthUI)
//                     .AddTo(_disposables);
//
//             model.Stamina
//                     .Subscribe(UpdateStaminaUI)
//                     .AddTo(_disposables);
//         }
//
//         private void SetupRemoteActorBindings(RemoteActorModel model)
//         {
//             // 원격 플레이어 관련 바인딩
//             model.OnNetworkStateReceived
//                     .Subscribe(UpdateNetworkState)
//                     .AddTo(_disposables);
//         }
//
//         private void UpdateVisuals(ActorState state)
//         {
//             switch (state)
//             {
//                 case ActorState.Moving:
//                     _view.SetAnimation("IsMoving", true);
//                     break;
//                 case ActorState.Dead:
//                     _view.SetAnimation("IsDead", true);
//                     break;
//                 // 다른 상태들 처리
//             }
//         }
//
//         private void UpdateHealthUI(float health)
//         {
//             
//         }
//
//         private void UpdateStaminaUI(float stamina)
//         {
//             
//         }
//
//         private void UpdateNetworkState(NetworkState state)
//         {
//             // 네트워크 상태에 따른 시각적 업데이트
//         }
//
//         private void EnableController<T>() where T : IActorController
//         {
//             _controllers.OfType<T>().FirstOrDefault()?.Initialize();
//         }
//
//         private void DisableController<T>() where T : IActorController
//         {
//             _controllers.OfType<T>().FirstOrDefault()?.Release();
//         }
//     }
// }

