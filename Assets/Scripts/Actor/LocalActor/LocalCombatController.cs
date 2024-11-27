using Client.Core;
using Client.Service;
using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public class LocalCombatController : IActorController
    {
        private readonly float _comboTimeWindow = 1.5f; // 콤보 입력 허용 시간
        private readonly CompositeDisposable _disposables = new();
        private readonly LocalActorPresenter _localActorPresenter;
        private readonly LocalActorModel _model;
        private readonly IActorView _view;
        private bool _canAttack = true;
        private InputService _inputService;
        private float _lastAttackTime;

        public LocalCombatController(LocalActorPresenter actorPresenter, IActorView view)
        {
            _localActorPresenter = actorPresenter;
            _model = actorPresenter.LocalModel;
            _view = view;
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _inputService = GameApplication.Instance.Services.InputService;

            SetupInputBindings();
            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            UpdateCombatState();
            CheckComboTimeout();
        }

        public void Release()
        {
            _disposables?.Dispose();
            IsActive = false;
        }

        private void SetupInputBindings()
        {
            // 전투 모드 전환
            _inputService.IsDefending
                         .Where(_ => IsActive)
                         .Subscribe(OnCombatMode)
                         .AddTo(_disposables);

            // 공격 입력
            _inputService.IsAttacking
                         .Where(_ => IsActive && _model.StateModel.IsInCombatMode.Value)
                         .Subscribe(OnAttackInput)
                         .AddTo(_disposables);
        }

        private void OnCombatMode(bool isPressed)
        {
            if (isPressed)
            {
                _model.StateModel.SetInCombatMode(!_model.StateModel.IsInCombatMode.Value);
                var combatState = _model.StateModel.IsInCombatMode.Value ? CombatState.Normal : CombatState.None;
                _model.StateModel.SetCurrentCombatState(combatState);
            }
        }

        private void OnAttackInput(bool isPressed)
        {
            if (!isPressed || !_canAttack)
            {
                return;
            }

            var currentTime = Time.time;
            if (currentTime - _lastAttackTime <= _comboTimeWindow)
            {
                ProgressCombo();
            }
            else
            {
                StartNewCombo();
            }

            _lastAttackTime = currentTime;
        }

        private void StartNewCombo()
        {
            _canAttack = false;
            _model.StateModel.SetAttackState(ComboState.Combo1);
            _model.StateModel.SetCurrentCombatState(CombatState.Attacking);
        }

        private void ProgressCombo()
        {
            switch (_model.StateModel.CurrentAttackState.Value)
            {
                case ComboState.Combo1:
                    _model.StateModel.SetAttackState(ComboState.Combo2);
                    break;
                case ComboState.Combo2:
                    _model.StateModel.SetAttackState(ComboState.Combo3);
                    break;
                case ComboState.Combo3:
                    _model.StateModel.SetAttackState(ComboState.ComboFinisher);
                    break;
                default:
                    _model.StateModel.SetAttackState(ComboState.Combo1);
                    break;
            }

            _model.StateModel.SetCurrentCombatState(CombatState.Attacking);
            _canAttack = false;
        }

        private void CheckComboTimeout()
        {
            if (_model.StateModel.CurrentAttackState.Value != ComboState.None && Time.time - _lastAttackTime > _comboTimeWindow)
            {
                ResetCombo();
            }
        }

        private void ResetCombo()
        {
            var combatState = _model.StateModel.IsInCombatMode.Value ? CombatState.Normal : CombatState.None;
            _canAttack = true;

            _model.StateModel.SetCurrentCombatState(combatState);
            _model.StateModel.SetAttackState(ComboState.None);
        }

        private void UpdateCombatState()
        {
            // 현재는 특별한 업데이트 로직이 필요 없음
            // 필요한 경우 여기에 추가
        }
    }
}
