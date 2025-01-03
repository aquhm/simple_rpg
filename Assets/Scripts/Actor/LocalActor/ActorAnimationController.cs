﻿using System.Collections.Generic;
using Client.Core;
using Client.Model;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Client.Actor.Animation
{
    public class ActorAnimationController : IActorController
    {
        // 애니메이션 레이어 상수
        private const int DEFAULT_LAYER = 0;
        private const int COMBAT_LAYER = 1;
        private const float MOVEMENT_THRESHOLD = 0.1f;

        // 애니메이션 해시값 캐싱
        private static readonly int Walking = Animator.StringToHash("walking");
        private static readonly int Running = Animator.StringToHash("running");
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int ComboIndex = Animator.StringToHash("comboIndex");
        private static readonly int InCombat = Animator.StringToHash("inCombat");


        private readonly IActorPresenter _actorPresenter;
        private readonly Animator _animator;
        private readonly Avatar _avatar;
        private readonly RuntimeAnimatorController _combatStateController;
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, int> _hashCache = new();
        private readonly InputStateModel _inputStateModel;

        private readonly RuntimeAnimatorController _normalStateController;
        private readonly ActorStateModel _stateModel;
        private readonly CompositeDisposable _triggerDisposables = new();

        private MovementState _previousState = MovementState.Idle;


        public ActorAnimationController(IActorPresenter actorPresenter, Animator animator)
        {
            _actorPresenter = actorPresenter;
            _animator = animator;
            _stateModel = (_actorPresenter as LocalActorPresenter)?.LocalModel.StateModel;
            _inputStateModel = GameApplication.Instance.Models.InputStateModel;
            _normalStateController = GameApplication.Instance.NormalStateController;
            _combatStateController = GameApplication.Instance.CombatStateController;
            _avatar = GameApplication.Instance.CharacterAvatar;

            // 초기 애니메이터 컨트롤러 설정
            if (_stateModel != null)
            {
                SetAnimatorMode(_stateModel.IsInCombatMode.Value);
            }
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            SetupStateBindings();
            SetupCombatBindings();
            SetupAnimationCallbacks();
            IsActive = true;
        }

        public void Update()
        {
            if (!IsActive)
            {
            }
        }

        public void Release()
        {
            _triggerDisposables?.Dispose();
            _disposables?.Dispose();
            IsActive = false;
        }

        private void SetupStateBindings()
        {
            _stateModel.CurrentMovementState
                       .Subscribe(currentState =>
                       {
                           OnMovementStateChanged(_previousState, currentState);
                           _previousState = currentState;
                       })
                       .AddTo(_disposables);

            // 전투 모드 상태 변경 감지
            _stateModel.IsInCombatMode
                       .Subscribe(SetAnimatorMode)
                       .AddTo(_disposables);

            // 전투 상태 변경 감지
            _stateModel.CurrentCombatState
                       .Subscribe(OnCombatStateChanged)
                       .AddTo(_disposables);
        }

        private void SetupCombatBindings()
        {
            // 콤보 공격 상태 감지
            _stateModel.CurrentAttackState
                       .Where(state => state != ComboState.None)
                       .Subscribe(comboState =>
                       {
                           SetTrigger(Attack);
                           SetInteger(ComboIndex, (int)comboState);
                       })
                       .AddTo(_disposables);
        }


        private void SetupAnimationCallbacks()
        {
            _triggerDisposables.Clear();
            var trigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (trigger == null)
            {
                return;
            }

            trigger.OnStateEnterAsObservable().Subscribe(state =>
            {
                OnStateEnter(state.StateInfo);
            }).AddTo(_triggerDisposables);

            trigger.OnStateExitAsObservable().Subscribe(state =>
            {
                OnStateExit(state.StateInfo);
            }).AddTo(_triggerDisposables);
        }

        private void OnStateEnter(AnimatorStateInfo stateInfo)
        {
            switch (stateInfo)
            {
                case { } when stateInfo.IsName("Normal.Jump"):
                    OnJumpAnimationEnter();
                    break;
            }
        }

        private void OnStateExit(AnimatorStateInfo stateInfo)
        {
            switch (stateInfo)
            {
                case { } when stateInfo.IsName("Normal.Jump"):
                    OnJumpAnimationExit();
                    break;
            }
        }

        private void SetAnimatorMode(bool isInCombatMode)
        {
            var parameterValues = SaveCurrentParameters();

            _animator.runtimeAnimatorController = isInCombatMode ?
                    _combatStateController :
                    _normalStateController;

            _animator.avatar = _avatar;

            RestoreParameters(parameterValues);
            SetupAnimationCallbacks();

            if (isInCombatMode == false)
            {
                SetTrigger("saveSword");
            }
        }

        private AnimatorParameterCache SaveCurrentParameters()
        {
            var cache = new AnimatorParameterCache();

            foreach (var parameter in _animator.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        cache.BoolParameters[parameter.nameHash] = _animator.GetBool(parameter.nameHash);
                        break;
                    case AnimatorControllerParameterType.Int:
                        cache.IntParameters[parameter.nameHash] = _animator.GetInteger(parameter.nameHash);
                        break;
                    case AnimatorControllerParameterType.Float:
                        cache.FloatParameters[parameter.nameHash] = _animator.GetFloat(parameter.nameHash);
                        break;
                }
            }

            return cache;
        }

        private void RestoreParameters(AnimatorParameterCache cache)
        {
            foreach (var parameter in _animator.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        if (cache.BoolParameters.TryGetValue(parameter.nameHash, out var boolValue))
                        {
                            _animator.SetBool(parameter.nameHash, boolValue);
                        }

                        break;
                    case AnimatorControllerParameterType.Int:
                        if (cache.IntParameters.TryGetValue(parameter.nameHash, out var intValue))
                        {
                            _animator.SetInteger(parameter.nameHash, intValue);
                        }

                        break;
                    case AnimatorControllerParameterType.Float:
                        if (cache.FloatParameters.TryGetValue(parameter.nameHash, out var floatValue))
                        {
                            _animator.SetFloat(parameter.nameHash, floatValue);
                        }

                        break;
                }
            }
        }

        private void OnCombatStateChanged(CombatState state)
        {
            switch (state)
            {
                case CombatState.None:
                    SetBool(InCombat, false);
                    break;
                case CombatState.Normal:
                    SetBool(InCombat, true);
                    break;
                case CombatState.Defending:
                    break;
                case CombatState.Attacking:
                    break;
            }
        }


        private void OnMovementStateChanged(MovementState previousState, MovementState currentState)
        {
            // 기본 이동 관련 파라미터 초기화
            SetBool(Walking, false);
            SetBool(Running, false);
            SetBool(Jump, false);
            //SetBool("falling", false);
            //SetBool("landing", false);

            // 현재 상태에 따른 파라미터 설정
            switch (currentState)
            {
                case MovementState.Walking:
                    SetBool(Walking, true);
                    break;
                case MovementState.Running:
                    SetBool(Running, true);
                    break;
                case MovementState.Jumping:
                    SetBool(Jump, true);
                    break;
                // case MovementState.Falling:
                //     SetBool("falling", true);
                //     break;
                // case MovementState.Landing:
                //     SetBool("landing", true);
                //     break;
            }

            // // 기본 이동 관련 파라미터 처리
            // SetBool("walking", currentState == MovementState.Walking);
            // SetBool("running", currentState == MovementState.Running);
            // SetBool("falling", currentState == MovementState.Falling);
            //
            // // 점프 시작 시에만 트리거 발동
            // if (previousState != MovementState.Jumping && currentState == MovementState.Jumping)
            // {
            //     SetTrigger("jumpStart");
            //     SetBool("isJumping", true);
            // }
            //
            // // 착지 처리
            // if ((previousState == MovementState.Jumping || previousState == MovementState.Falling)
            //     && currentState == MovementState.Landing)
            // {
            //     SetTrigger("land");
            //     SetBool("isJumping", false);
            // }
        }

        private void OnJumpAnimationEnter()
        {
            // 점프 시작 시 필요한 처리
        }

        private void OnJumpAnimationExit()
        {
            // 점프 종료 시 필요한 처리
        }

#region Helpers

        // 추가적인 헬퍼 메서드가 필요한 경우
        private bool IsValidHash(int hashId)
        {
            foreach (var parameter in _animator.parameters)
            {
                if (parameter.nameHash == hashId)
                {
                    return true;
                }
            }

            return false;
        }

#endregion

        private class AnimatorParameterCache
        {
            public Dictionary<int, bool> BoolParameters { get; } = new();
            public Dictionary<int, int> IntParameters { get; } = new();
            public Dictionary<int, float> FloatParameters { get; } = new();
        }

#region Animation Parameter Methods

        private int GetHash(string name)
        {
            if (!_hashCache.ContainsKey(name))
            {
                _hashCache[name] = Animator.StringToHash(name);
            }

            return _hashCache[name];
        }

        private bool GetBool(string name)
        {
            if (!IsActive)
            {
                return false;
            }

            return _animator.GetBool(GetHash(name));
        }

        private void SetBool(string name, bool value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetBool(GetHash(name), value);
        }

        private float GetFloat(string name)
        {
            if (!IsActive)
            {
                return 0f;
            }

            return _animator.GetFloat(GetHash(name));
        }

        private void SetFloat(string name, float value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetFloat(GetHash(name), value);
        }

        private int GetInteger(string name)
        {
            if (!IsActive)
            {
                return 0;
            }

            return _animator.GetInteger(GetHash(name));
        }

        private void SetInteger(string name, int value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetInteger(GetHash(name), value);
        }

        private void SetTrigger(string name)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetTrigger(GetHash(name));
        }

        private void ResetTrigger(string name)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.ResetTrigger(GetHash(name));
        }

        private bool GetBoolFromHash(int hash)
        {
            if (!IsActive)
            {
                return false;
            }

            return _animator.GetBool(hash);
        }

        private void SetBoolFromHash(int hash, bool value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetBool(hash, value);
        }

#endregion

#region Hash-based Parameter Methods

        private bool GetBool(int hashId)
        {
            if (!IsActive)
            {
                return false;
            }

            return _animator.GetBool(hashId);
        }

        private void SetBool(int hashId, bool value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetBool(hashId, value);
        }

        private float GetFloat(int hashId)
        {
            if (!IsActive)
            {
                return 0f;
            }

            return _animator.GetFloat(hashId);
        }

        private void SetFloat(int hashId, float value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetFloat(hashId, value);
        }

        private int GetInteger(int hashId)
        {
            if (!IsActive)
            {
                return 0;
            }

            return _animator.GetInteger(hashId);
        }

        private void SetInteger(int hashId, int value)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetInteger(hashId, value);
        }

        private void SetTrigger(int hashId)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetTrigger(hashId);
        }

        private void ResetTrigger(int hashId)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.ResetTrigger(hashId);
        }

        private void SetLayerWeight(int layerIndex, float weight)
        {
            if (!IsActive)
            {
                return;
            }

            _animator.SetLayerWeight(layerIndex, weight);
        }

#endregion
    }
}
