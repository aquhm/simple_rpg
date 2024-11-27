using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client.Actor
{
    public class ActorInputController : IActorController
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly ActorInputModel _inputModel;
        private InputActionMap _actionMap;
        private InputActionAsset _inputActions;

        public ActorInputController()
        {
            _inputModel = new ActorInputModel();
        }

        public IReadOnlyReactiveProperty<Vector3> Move => _inputModel.Move;
        public IReadOnlyReactiveProperty<bool> Jump => _inputModel.Jump;
        public IReadOnlyReactiveProperty<bool> IsSprinting => _inputModel.IsSprinting;
        public IReadOnlyReactiveProperty<bool> IsAttacking => _inputModel.IsAttacking;
        public IReadOnlyReactiveProperty<bool> IsDefending => _inputModel.IsDefending;

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            LoadInputActions();
            SetupInputBindings();
            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive) _inputModel.ResetAllInputs();
        }

        public void Release()
        {
            _disposables.Dispose();
            _inputModel?.Dispose();
            _actionMap?.Disable();

            IsActive = false;
        }

        private void LoadInputActions()
        {
            _inputActions = Resources.Load<InputActionAsset>("PlayerInputActions");
            if (_inputActions == null)
            {
                Debug.LogError("Failed to load PlayerInputActions!");
                return;
            }

            _actionMap = _inputActions.FindActionMap("Player");
            _actionMap?.Enable();
        }

        private void SetupInputBindings()
        {
            if (_actionMap == null) return;

            SetupMovementInput();
            SetupJumpInput();
            SetupSprintInput();
            SetupCombatInput();
        }

        private void SetupMovementInput()
        {
            var moveAction = _actionMap.FindAction("Move");
            if (moveAction != null)
            {
                Observable.FromEvent<InputAction.CallbackContext>(
                                                                  h => moveAction.performed += h,
                                                                  h => moveAction.performed -= h)
                        .Select(ctx =>
                        {
                            var input = ctx.ReadValue<Vector2>();
                            return new Vector3(input.x, 0f, input.y);
                        })
                        .Subscribe(value => _inputModel.SetMoveInput(value))
                        .AddTo(_disposables);

                moveAction.canceled += _ => _inputModel.SetMoveInput(Vector3.zero);
            }
        }

        private void SetupJumpInput()
        {
            var jumpAction = _actionMap.FindAction("Jump");
            if (jumpAction != null)
            {
                Observable.FromEvent<InputAction.CallbackContext>(
                                                                  h => jumpAction.started += h,
                                                                  h => jumpAction.started -= h)
                        .Subscribe(_ => _inputModel.SetJumpInput(true))
                        .AddTo(_disposables);

                jumpAction.canceled += _ => _inputModel.SetJumpInput(false);
            }
        }

        private void SetupSprintInput()
        {
            var sprintAction = _actionMap.FindAction("Sprint");
            if (sprintAction != null)
                Observable.FromEvent<InputAction.CallbackContext>(
                                                                  h => sprintAction.performed += h,
                                                                  h => sprintAction.performed -= h)
                        .Select(ctx => ctx.ReadValueAsButton())
                        .Subscribe(value => _inputModel.SetSprintInput(value))
                        .AddTo(_disposables);
        }

        private void SetupCombatInput()
        {
            // Attack
            var attackAction = _actionMap.FindAction("Attack");
            if (attackAction != null)
            {
                Observable.FromEvent<InputAction.CallbackContext>(
                                                                  h => attackAction.started += h,
                                                                  h => attackAction.started -= h)
                        .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                        .Subscribe(_ => _inputModel.SetAttackInput(true))
                        .AddTo(_disposables);

                attackAction.canceled += _ => _inputModel.SetAttackInput(false);
            }

            // Defense
            var defendAction = _actionMap.FindAction("Defense");
            if (defendAction != null)
                Observable.FromEvent<InputAction.CallbackContext>(
                                                                  h => defendAction.performed += h,
                                                                  h => defendAction.performed -= h)
                        .Select(ctx => ctx.ReadValueAsButton())
                        .Subscribe(value => _inputModel.SetDefendInput(value))
                        .AddTo(_disposables);
        }
    }
}