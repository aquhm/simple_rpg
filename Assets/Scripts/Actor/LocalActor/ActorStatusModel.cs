using System;
using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public class ActorStatusModel : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public ActorStatusModel(ActorSettings settings)
        {
            Settings = settings;
        }

        public FloatReactiveProperty Health { get; } = new(100f);
        public FloatReactiveProperty Stamina { get; } = new(100f);
        public BoolReactiveProperty IsInvincible { get; } = new();
        public ActorSettings Settings { get; }

        public void Dispose()
        {
            Health?.Dispose();
            Stamina?.Dispose();
            IsInvincible?.Dispose();
            _disposables?.Dispose();
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible.Value)
            {
                return;
            }

            Health.Value = Mathf.Max(0, Health.Value - damage);
        }

        public void Heal(float amount)
        {
            Health.Value = Mathf.Min(100f, Health.Value + amount);
        }

        public void UseStamina(float amount)
        {
            Stamina.Value = Mathf.Max(0, Stamina.Value - amount);
        }

        public void RestoreStamina(float amount)
        {
            Stamina.Value = Mathf.Min(100f, Stamina.Value + amount);
        }

        public void SetInvincible(bool isInvincible, float duration = 0)
        {
            IsInvincible.Value = isInvincible;
            // duration이 있는 경우 타이머 처리 로직 추가 가능
        }
    }
}
