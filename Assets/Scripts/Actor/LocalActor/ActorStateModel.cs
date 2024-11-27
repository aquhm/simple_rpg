using System;
using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public enum MovementState
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Landing
    }

    public enum CombatState
    {
        None, // 비전투 상태
        Normal, // 전투 기본 자세
        Defending,
        Attacking
    }

    public enum ComboState
    {
        None,
        Combo1,
        Combo2,
        Combo3,
        ComboFinisher
    }

    public class ActorStateModel : IDisposable
    {
        public ReactiveProperty<MovementState> CurrentMovementState { get; } = new();
        public ReactiveProperty<CombatState> CurrentCombatState { get; } = new();
        public ReactiveProperty<ComboState> CurrentAttackState { get; } = new();
        public ReactiveProperty<bool> IsGrounded { get; } = new();
        public ReactiveProperty<Vector3> Position { get; } = new();
        public ReactiveProperty<Vector3> Movement { get; } = new();

        // 전투 모드 상태 추가
        public ReactiveProperty<bool> IsInCombatMode { get; } = new(false);

        public void Dispose()
        {
            CurrentMovementState?.Dispose();
            CurrentCombatState?.Dispose();
            CurrentAttackState?.Dispose();
            IsGrounded?.Dispose();
            Position?.Dispose();
            Movement?.Dispose();
            IsInCombatMode?.Dispose();
        }

        public void SetIsGrounded(bool value)
        {
            IsGrounded.Value = value;
        }

        public void SetCurrentMovementState(MovementState value)
        {
            CurrentMovementState.Value = value;
        }

        public void SetCurrentCombatState(CombatState value)
        {
            CurrentCombatState.Value = value;
        }

        public void SetPosition(Vector3 value)
        {
            Position.Value = value;
        }

        public void SetMovement(Vector3 value)
        {
            Movement.Value = value;
        }

        public void SetAttackState(ComboState value)
        {
            CurrentAttackState.Value = value;
        }

        public void SetInCombatMode(bool value)
        {
            if (IsInCombatMode.Value != value)
            {
                IsInCombatMode.Value = value;
                // 전투 모드 전환 시 기본 전투 상태로 설정
                CurrentCombatState.Value = value ? CombatState.Normal : CombatState.None;
            }
        }
    }
}
