using Client.Core;
using Client.Service;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Client.Actor
{
    public class LocalMovementController : IActorController
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly float _groundedGravity = -0.5f;
        private readonly float _jumpAnimationDelay = 1f; // 연속 점프 방지를 위한 딜레이

        private readonly LocalActorPresenter _localActorPresenter;

        // 점프 & 중력 관련
        private readonly float _maxJumpHeight = 2.0f;
        private readonly float _maxJumpTime = 0.75f;

        private readonly LocalActorModel _model;
        private readonly IActorView _view;
        private CameraService _cameraService;

        private Vector3 _currentMovement;
        private float _currentRotationVelocity;
        private Vector3 _currentRunMovement;
        private float _gravity = -9.8f;
        private float _initialJumpVelocity;

        private InputService _inputService;
        private bool _isJumpAnimating;
        private bool _isJumping;
        private bool _isJumpPressed;


        // 이동 관련 변수들
        private bool _isMovementPressed;
        private bool _isMoving;
        private bool _isRunPressed;
        private float _lastJumpTime;

        public LocalMovementController(LocalActorPresenter actorPresenter,
                IActorView view)
        {
            _localActorPresenter = actorPresenter;
            _model = actorPresenter.LocalModel;
            _view = view;
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _inputService = GameApplication.Instance.Services.InputService;
            _cameraService = GameApplication.Instance.Services.GetService<CameraService>();

            SetUpJumpVariables();
            SetupMovementBindings();
            Observable.EveryFixedUpdate().Subscribe(_ => FixedUpdate()).AddTo(_disposables);

            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            HandleRotation();
            HandleMovement();
            HandleGravity();
            HandleJump();
            UpdateMovementState();

            //Debug.Log($"=========> _currentMovement.y = {_currentMovement.y} _currentRunMovement.y = {_currentRunMovement.y}");
        }

        public void Release()
        {
            _lastJumpTime = 0f;
            _disposables?.Dispose();
            IsActive = false;
        }

        private void SetUpJumpVariables()
        {
            _gravity = _view.Settings.Gravity;

            var timeToApex = _maxJumpTime / 2;
            _gravity = -2 * _maxJumpHeight / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = 2 * _maxJumpHeight / timeToApex;
        }

        private void SetupMovementBindings()
        {
            _inputService.Move.Where(_ => IsActive).Subscribe(OnMoveInput).AddTo(_disposables);
            _inputService.IsSprinting.Where(_ => IsActive).Subscribe(OnSprintingInput).AddTo(_disposables);
            _inputService.Jump.Subscribe(OnJumpInput).AddTo(_disposables);

            SetupJumpAnimation();
        }

        private void OnSprintingInput(bool pressed)
        {
            Debug.Log($"OnSprintingInput pressed = {pressed}");
            _isRunPressed = pressed;
        }

        private void OnJumpInput(bool pressed)
        {
            _isJumpPressed = pressed;
        }

        private void SetupJumpAnimation()
        {
            var trigger = _view.Animator?.GetBehaviour<ObservableStateMachineTrigger>();
            if (trigger is null)
            {
                return;
            }

            trigger.OnStateEnterAsObservable().Subscribe(onStateInfo =>
            {
                if (onStateInfo.StateInfo.IsName("Normal.Jump"))
                {
                    _isJumpAnimating = true;
                }
            }).AddTo(_disposables);

            trigger.OnStateExitAsObservable().Subscribe(onStateInfo =>
            {
                if (onStateInfo.StateInfo.IsName("Normal.Jump"))
                {
                    _isJumpAnimating = false;
                }
            }).AddTo(_disposables);
        }

        private void FixedUpdate()
        {
        }

        private void HandleRotation()
        {
            if (_isMoving)
            {
                var moveDirection = _model.StateModel.Movement.Value;
                var cameraAngle = _cameraService?.MainCamra?.transform.eulerAngles.y ?? 0f;
                var targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraAngle;

                var smoothAngle = Mathf.SmoothDampAngle(_view.Transform.eulerAngles.y,
                        targetAngle,
                        ref _currentRotationVelocity,
                        _model.StatusModel.Settings.RotationSmoothTime);

                var moveVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _view.Transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                _currentRunMovement.x = moveVector.x;
                _currentRunMovement.z = moveVector.z;
                _currentMovement.x = moveVector.x;
                _currentMovement.z = moveVector.z;
            }
        }

        private void HandleMovement()
        {
            _currentMovement.x *= _model.StatusModel.Settings.MoveSpeed;
            _currentMovement.z *= _model.StatusModel.Settings.MoveSpeed;

            _currentRunMovement.x *= _model.StatusModel.Settings.MoveSpeed * _model.StatusModel.Settings.RunSpeedMultiplier;
            _currentRunMovement.z *= _model.StatusModel.Settings.MoveSpeed * _model.StatusModel.Settings.RunSpeedMultiplier;

            var movement = _isRunPressed ? _currentRunMovement : _currentMovement;
            _view.CharacterController.Move(movement * Time.deltaTime);

            _model.StateModel.SetPosition(_view.Transform.position);
        }

        private void HandleGravity()
        {
            var isFalling = _currentMovement.y < 0.0f || !_isJumpPressed;

            if (_view.CharacterController.isGrounded)
            {
                if (_isJumpAnimating)
                {
                    _isJumpAnimating = false;
                }

                _currentMovement.y = _groundedGravity;
                _currentRunMovement.y = _groundedGravity;
            }
            else if (isFalling)
            {
                var previousYVelocity = _currentMovement.y;
                var newYVelocity = _currentMovement.y + _gravity * Time.deltaTime;
                var nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;

                _currentMovement.y = nextYVelocity;
                _currentRunMovement.y = nextYVelocity;
            }
            else
            {
                var previousYVelocity = _currentMovement.y;
                var newYVelocity = _currentMovement.y + _gravity * Time.deltaTime;
                var nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;

                _currentMovement.y = nextYVelocity;
                _currentRunMovement.y = nextYVelocity;
            }

            //Debug.Log($"isGrounded = {_view.CharacterController.isGrounded} isFalling = {isFalling} _currentMovement.y = {_currentMovement.y}");
        }

        private void HandleJump()
        {
            //Debug.Log($"_isJumping = {_isJumping} _view.CharacterController.isGrounded = {_view.CharacterController.isGrounded} _isJumpPressed = {_isJumpPressed}");
            if (!_isJumping && _view.CharacterController.isGrounded && _isJumpPressed)
            {
                // 마지막 점프로부터 일정 시간이 지났는지 확인
                if (Time.time - _lastJumpTime < _jumpAnimationDelay)
                {
                    return;
                }

                _lastJumpTime = Time.time;

                _isJumping = true;
                _isJumpAnimating = true;
                _currentMovement.y = _initialJumpVelocity * 0.5f;
                _currentRunMovement.y = _initialJumpVelocity * 0.5f;
            }
            else if (!_isJumpPressed && _isJumping && _view.CharacterController.isGrounded)
            {
                _isJumping = false;
            }
        }

        private void OnMoveInput(Vector3 moveInput)
        {
            _currentMovement = moveInput;
            _currentRunMovement = moveInput;
            _isMoving = moveInput.magnitude >= _model.StatusModel.Settings.MovementThreshold;
            _isMovementPressed = _isMoving;

            _model.StateModel.SetMovement(moveInput);
        }

        // private void CheckGroundedState()
        // {
        //     if (_view.GroundCheckInfo?.groundCheckers == null)
        //     {
        //         return;
        //     }
        //
        //     var isGrounded = false;
        //     foreach (var checker in _view.GroundCheckInfo.groundCheckers)
        //     {
        //         if (Physics.CheckSphere(checker.position,
        //                     _view.Settings.GroundCheckRadius,
        //                     _view.Settings.GroundMask))
        //         {
        //             isGrounded = true;
        //             break;
        //         }
        //     }
        //
        //     _model.StateModel.SetIsGrounded(isGrounded);
        // }

        private void UpdateMovementState()
        {
            var newState = MovementState.Idle;

            if (_isJumpAnimating)
            {
                newState = MovementState.Jumping;
            }
            else if (_isMovementPressed && _isMoving)
            {
                newState = _isRunPressed ? MovementState.Running : MovementState.Walking;
            }

            // else if (!_view.CharacterController.isGrounded && _currentMovement.y < 0)
            // {
            //     newState = MovementState.Falling;
            // }
            // else if (_view.CharacterController.isGrounded && _model.StateModel.CurrentMovementState.Value is MovementState.Jumping or MovementState.Falling)
            // {
            //     newState = MovementState.Landing;
            // }

            if (_model.StateModel.CurrentMovementState.Value != newState)
            {
                _model.StateModel.SetCurrentMovementState(newState);
            }
        }
    }
}
