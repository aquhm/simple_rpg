using System;
using System.Collections.Generic;
using UniRx;

namespace Client.Actor
{
    public enum EquipmentType
    {
        Sword,
        Shield
    }

    public enum EquipmentAction
    {
        None,
        Active,
        Deactive
    }

    public struct EquipmentStateChange
    {
        public EquipmentType Type { get; }
        public EquipmentAction Action { get; }

        public EquipmentStateChange(EquipmentType type, EquipmentAction action)
        {
            Type = type;
            Action = action;
        }
    }

    public class ActorEquipmentStateModel : IDisposable
    {
        private readonly Dictionary<EquipmentType, ReactiveProperty<EquipmentAction>> _equipmentStates = new();

        private readonly Subject<EquipmentStateChange[]> _stateChangeSubject = new();

        public ActorEquipmentStateModel()
        {
            foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                _equipmentStates[type] = new ReactiveProperty<EquipmentAction>(EquipmentAction.None);
            }
        }

        public IObservable<EquipmentStateChange[]> OnStateChanged
        {
            get => _stateChangeSubject;
        }

        public void Dispose()
        {
            foreach (var state in _equipmentStates.Values)
            {
                state?.Dispose();
            }

            _stateChangeSubject?.Dispose();
        }

        public void SetEquipmentState(EquipmentType type, EquipmentAction action)
        {
            if (_equipmentStates.TryGetValue(type, out var state))
            {
                state.Value = action;
                _stateChangeSubject.OnNext(new[] { new EquipmentStateChange(type, action) });
            }
        }

        public void SetMultipleEquipmentStates(params (EquipmentType type, EquipmentAction action)[] changes)
        {
            var stateChanges = new List<EquipmentStateChange>();

            foreach (var (type, action) in changes)
            {
                if (_equipmentStates.TryGetValue(type, out var state))
                {
                    state.Value = action;
                    stateChanges.Add(new EquipmentStateChange(type, action));
                }
            }

            if (stateChanges.Count > 0)
            {
                _stateChangeSubject.OnNext(stateChanges.ToArray());
            }
        }

        public EquipmentAction GetEquipmentState(EquipmentType type)
        {
            return _equipmentStates.TryGetValue(type, out var state) ? state.Value : EquipmentAction.None;
        }
    }
}
