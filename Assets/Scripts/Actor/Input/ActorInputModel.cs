// Input 상태를 관리하는 Model 클래스

using System;
using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public class ActorInputModel : IDisposable
    {
        private readonly ReactiveProperty<bool> _attackInput = new();
        private readonly ReactiveProperty<bool> _defendInput = new();

        private readonly ReactiveProperty<bool> _jumpInput = new();

        // Input States
        private readonly ReactiveProperty<Vector3> _moveInput = new();
        private readonly ReactiveProperty<bool> _sprintInput = new();

        // Public ReadOnly Properties
        public IReadOnlyReactiveProperty<Vector3> Move => _moveInput;
        public IReadOnlyReactiveProperty<bool> Jump => _jumpInput;
        public IReadOnlyReactiveProperty<bool> IsSprinting => _sprintInput;
        public IReadOnlyReactiveProperty<bool> IsAttacking => _attackInput;
        public IReadOnlyReactiveProperty<bool> IsDefending => _defendInput;

        public void Dispose()
        {
            _moveInput?.Dispose();
            _jumpInput?.Dispose();
            _sprintInput?.Dispose();
            _attackInput?.Dispose();
            _defendInput?.Dispose();
        }

        // State Setters
        public void SetMoveInput(Vector3 value)
        {
            _moveInput.Value = value;
        }

        public void SetJumpInput(bool value)
        {
            _jumpInput.Value = value;
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

        public void ResetAllInputs()
        {
            _moveInput.Value = Vector3.zero;
            _jumpInput.Value = false;
            _sprintInput.Value = false;
            _attackInput.Value = false;
            _defendInput.Value = false;
        }
    }
}