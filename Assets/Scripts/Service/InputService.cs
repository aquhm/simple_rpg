using System;
using Client.Core;
using Client.Model;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Client.Service
{
    public class InputService : IService
    {
        private readonly CompositeDisposable _disposables = new();

        private readonly PlayerInputActions _playerInputActions = new();
        private InputActionAsset _inputActions;
        private InputStateModel _inputStateModel;

        public IReadOnlyReactiveProperty<Vector3> Move
        {
            get => _inputStateModel.Move;
        }

        public IReadOnlyReactiveProperty<bool> Jump
        {
            get => _inputStateModel.Jump;
        }

        public IReadOnlyReactiveProperty<bool> IsSprinting
        {
            get => _inputStateModel.IsSprinting;
        }

        public IReadOnlyReactiveProperty<bool> IsAttacking
        {
            get => _inputStateModel.IsAttacking;
        }

        public IReadOnlyReactiveProperty<bool> IsDefending
        {
            get => _inputStateModel.IsDefending;
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _inputStateModel = GameApplication.Instance.Models.InputStateModel;

            SetupInputActions();
        }

        public void DoUpdate(float deltaTime)
        {
        }

        public void Release()
        {
            if (_playerInputActions != null)
            {
                _playerInputActions.Disable();
            }

            _disposables.Clear();
            _inputStateModel?.Release();
        }

        private bool SetupInputActions()
        {
            try
            {
                SetupMovementStreams();
                SetupCombatStreams();
                //SetupInteractionStreams();

                _playerInputActions.Enable();
                IsActive = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to setup input actions: {e.Message}");
                IsActive = false;
            }

            return IsActive;
        }

        private void SetupMovementStreams()
        {
            // Move Stream (WASD + Arrow keys)
            _playerInputActions.Player.Move.started += context =>
            {
                var input = context.ReadValue<Vector2>();
                _inputStateModel.SetMoveInput(new Vector3(input.x, 0f, input.y));
            };
            _playerInputActions.Player.Move.performed += context =>
            {
                var input = context.ReadValue<Vector2>();
                _inputStateModel.SetMoveInput(new Vector3(input.x, 0f, input.y));
            };
            _playerInputActions.Player.Move.canceled += context => { _inputStateModel.SetMoveInput(Vector3.zero); };

            // Sprint Stream (Shift key)
            _playerInputActions.Player.Sprint.performed += context => { _inputStateModel.SetSprintInput(context.ReadValueAsButton()); };
            _playerInputActions.Player.Sprint.canceled += context => { _inputStateModel.SetSprintInput(context.ReadValueAsButton()); };

            // Jump Stream (Space key)
            _playerInputActions.Player.Jump.performed += context => { _inputStateModel.SetJumpInput(context.ReadValueAsButton()); };
            _playerInputActions.Player.Jump.canceled += context => { _inputStateModel.SetJumpInput(context.ReadValueAsButton()); };
        }

        private void SetupCombatStreams()
        {
            // Attack Stream (Left Mouse Button)

            _playerInputActions.Player.Attack.started += context => { _inputStateModel.SetAttackInput(context.ReadValueAsButton()); };
            _playerInputActions.Player.Attack.canceled += context => { _inputStateModel.SetAttackInput(context.ReadValueAsButton()); };

            _playerInputActions.Player.Defense.started += context => { _inputStateModel.SetDefendInput(context.ReadValueAsButton()); };
            _playerInputActions.Player.Defense.canceled += context => { _inputStateModel.SetDefendInput(context.ReadValueAsButton()); };
        }
    }
}
