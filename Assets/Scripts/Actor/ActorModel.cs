using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public class ActorModel : IActorModel
    {
        private readonly CompositeDisposable _disposables = new();

        public ActorModel(ActorSettings settings)
        {
            Settings = settings;
        }

        public ReactiveProperty<ActorState> State { get; } = new(ActorState.Idle);
        public ReactiveProperty<Vector3> Position { get; } = new(Vector3.zero);
        public ReactiveProperty<Vector3> Movement { get; } = new();
        public FloatReactiveProperty Health { get; } = new(100f);
        public FloatReactiveProperty Stamina { get; } = new(100f);
        public BoolReactiveProperty IsInvincible { get; } = new();
        public BoolReactiveProperty IsGrounded { get; } = new();
        public ActorSettings Settings { get; }

        public void UpdateState(ActorState newState)
        {
            State.Value = newState;
        }

        public void UpdatePosition(Vector3 position)
        {
            Position.Value = position;
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible.Value) return;
            Health.Value = Mathf.Max(0, Health.Value - damage);

            if (Health.Value <= 0)
                UpdateState(ActorState.Dead);
        }

        public void Heal(float amount)
        {
            Health.Value = amount;
        }

        public void UseStamina(float amount)
        {
            Stamina.Value = amount;
        }

        public void RestoreStamina(float amount)
        {
            Stamina.Value = amount;
        }

        public void SetInvincible(bool isInvincible, float duration = 0)
        {
            IsInvincible.Value = isInvincible;
        }

        public void SetIsGrounded(bool isGrounded)
        {
            IsGrounded.Value = isGrounded;
        }
    }
}