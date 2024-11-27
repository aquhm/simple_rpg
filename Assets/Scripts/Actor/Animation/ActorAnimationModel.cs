using System;
using UniRx;

namespace Client.Actor.Animation
{
    public class ActorAnimationModel : IDisposable
    {
        private readonly Subject<Unit> _attackTrigger = new();
        private readonly ReactiveProperty<int> _currentLayerWeight = new();
        private readonly Subject<Unit> _getSwordTrigger = new();
        private readonly BoolReactiveProperty _isDefending = new();
        private readonly BoolReactiveProperty _isJumping = new();
        private readonly BoolReactiveProperty _isRunning = new();
        private readonly BoolReactiveProperty _isWalking = new();
        private readonly Subject<Unit> _saveSwordTrigger = new();

        public IReadOnlyReactiveProperty<bool> IsWalking
        {
            get => _isWalking;
        }

        public IReadOnlyReactiveProperty<bool> IsRunning
        {
            get => _isRunning;
        }

        public IReadOnlyReactiveProperty<bool> IsJumping
        {
            get => _isJumping;
        }

        public IReadOnlyReactiveProperty<bool> OnDefending
        {
            get => _isDefending;
        }

        public IObservable<Unit> OnAttack
        {
            get => _attackTrigger;
        }

        public IObservable<Unit> OnGetSword
        {
            get => _getSwordTrigger;
        }

        public IObservable<Unit> OnSaveSword
        {
            get => _saveSwordTrigger;
        }

        public IReadOnlyReactiveProperty<int> CurrentLayerWeight
        {
            get => _currentLayerWeight;
        }

        public void Dispose()
        {
            _isWalking?.Dispose();
            _isRunning?.Dispose();
            _isJumping?.Dispose();
            _isDefending?.Dispose();
            _attackTrigger?.Dispose();
            _getSwordTrigger?.Dispose();
            _saveSwordTrigger?.Dispose();
            _currentLayerWeight?.Dispose();
        }

        public void SetWalking(bool value)
        {
            _isWalking.Value = value;
        }

        public void SetRunning(bool value)
        {
            _isRunning.Value = value;
        }

        public void SetJumping(bool value)
        {
            _isJumping.Value = value;
        }

        public void SetDefending(bool value)
        {
            _isDefending.Value = value;
        }

        public void TriggerAttack()
        {
            _attackTrigger.OnNext(Unit.Default);
        }

        public void TriggerGetSword()
        {
            _getSwordTrigger.OnNext(Unit.Default);
        }

        public void TriggerSaveSword()
        {
            _saveSwordTrigger.OnNext(Unit.Default);
        }

        public void SetLayerWeight(int value)
        {
            _currentLayerWeight.Value = value;
        }
    }
}
