using Client.Core;
using UniRx;
using UnityEngine;

namespace Client.Model
{
    public interface IInputStateModel : IModel
    {
        IReadOnlyReactiveProperty<Vector3> Move { get; }
        IReadOnlyReactiveProperty<bool> Jump { get; }
        IReadOnlyReactiveProperty<bool> IsSprinting { get; }
        IReadOnlyReactiveProperty<bool> IsAttacking { get; }
        IReadOnlyReactiveProperty<bool> IsDefending { get; }

        void SetMoveInput(Vector3 value);
        void SetJumpInput(bool value);
        void SetSprintInput(bool value);
        void SetAttackInput(bool value);
        void SetDefendInput(bool value);
    }

    public class InputStateModel : IInputStateModel
    {
        private readonly ReactiveProperty<bool> _attackInput = new();
        private readonly ReactiveProperty<bool> _defendInput = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly ReactiveProperty<bool> _jumpInput = new();
        private readonly ReactiveProperty<Vector3> _moveInput = new();
        private readonly ReactiveProperty<bool> _sprintInput = new();

        public IReadOnlyReactiveProperty<Vector3> Move
        {
            get => _moveInput;
        }

        public IReadOnlyReactiveProperty<bool> Jump
        {
            get => _jumpInput;
        }

        public IReadOnlyReactiveProperty<bool> IsSprinting
        {
            get => _sprintInput;
        }

        public IReadOnlyReactiveProperty<bool> IsAttacking
        {
            get => _attackInput;
        }

        public IReadOnlyReactiveProperty<bool> IsDefending
        {
            get => _defendInput;
        }

        public void Initialize()
        {
        }

        public void Reset()
        {
            _moveInput.Value = Vector3.zero;
            _jumpInput.Value = false;
            _sprintInput.Value = false;
            _attackInput.Value = false;
            _defendInput.Value = false;
        }

        public void Release()
        {
            _disposables.Dispose();
            _moveInput?.Dispose();
            _jumpInput?.Dispose();
            _sprintInput?.Dispose();
            _attackInput?.Dispose();
            _defendInput?.Dispose();
        }

        public void SetMoveInput(Vector3 value)
        {
            _moveInput.Value = value;
        }

        public void SetJumpInput(bool value)
        {
            _jumpInput.SetValueAndForceNotify(value);
        }

        public void SetSprintInput(bool value)
        {
            _sprintInput.Value = value;
        }

        public void SetAttackInput(bool value)
        {
            _attackInput.Value = value;
        }

        public void SetDefendInput(bool value)
        {
            _defendInput.Value = value;
        }
    }
}
