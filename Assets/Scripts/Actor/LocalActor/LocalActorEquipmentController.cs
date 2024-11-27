using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Client.Actor
{
    public class LocalActorEquipmentController : IActorController
    {
        private readonly LocalActorPresenter _actorPresenter;
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, Transform> _equipmentPoints = new();
        private readonly Dictionary<string, GameObject> _equipments = new();
        private readonly ActorEquipmentStateModel _equipmentStateModel;
        private readonly IActorView _view;

        public LocalActorEquipmentController(LocalActorPresenter actorPresenter, IActorView view)
        {
            _actorPresenter = actorPresenter;
            _view = view;
            _equipmentStateModel = actorPresenter.LocalModel.EquipmentStateModel;
        }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            InitializeEquipments();
            InitializeRx();

            IsActive = true;
        }

        public void Update()
        {
        }

        public void Release()
        {
            _disposables?.Dispose();
            IsActive = false;
        }

        private void InitializeRx()
        {
            _equipmentStateModel.OnStateChanged
                                .Subscribe(OnEquipmentStatesChanged)
                                .AddTo(_disposables);
        }

        private void OnEquipmentStatesChanged(EquipmentStateChange[] changes)
        {
            foreach (var change in changes)
            {
                switch (change.Type)
                {
                    case EquipmentType.Sword:
                        HandleSwordState(change.Action);
                        break;

                    case EquipmentType.Shield:
                        HandleShieldState(change.Action);
                        break;
                }
            }
        }

        private void HandleSwordState(EquipmentAction action)
        {
            switch (action)
            {
                case EquipmentAction.Active:
                    ShowEquipment("SwordHand");
                    HideEquipment("SwordBack");
                    break;

                case EquipmentAction.Deactive:
                    ShowEquipment("SwordBack");
                    HideEquipment("SwordHand");
                    break;
            }
        }

        private void HandleShieldState(EquipmentAction action)
        {
            switch (action)
            {
                case EquipmentAction.Active:
                    ShowEquipment("ShieldHand");
                    HideEquipment("ShieldBack");
                    break;

                case EquipmentAction.Deactive:
                    ShowEquipment("ShieldBack");
                    HideEquipment("ShieldHand");
                    break;
            }
        }

        public void ShowEquipment(string equipmentId)
        {
            if (_equipments.TryGetValue(equipmentId, out var equipment))
            {
                equipment.SetActive(true);
            }
        }

        public void HideEquipment(string equipmentId)
        {
            if (_equipments.TryGetValue(equipmentId, out var equipment))
            {
                equipment.SetActive(false);
            }
        }

        public void MoveEquipment(string command)
        {
            var parts = command.Split('.');
            if (parts.Length != 2)
            {
                return;
            }

            var equipmentId = parts[0];
            var pointId = parts[1];

            if (_equipments.TryGetValue(equipmentId, out var equipment) &&
                _equipmentPoints.TryGetValue(pointId, out var point))
            {
                equipment.transform.SetParent(point, false);
                equipment.transform.localPosition = Vector3.zero;
                equipment.transform.localRotation = Quaternion.identity;
            }
        }

        private void InitializeEquipments()
        {
            if (IsActive)
            {
                return;
            }

            var equipmentRefs = _view.EquipmentRefs;
            if (equipmentRefs == null)
            {
                return;
            }

            foreach (var equipment in equipmentRefs.Equipments)
            {
                _equipments[equipment.Key] = equipment.Value;
            }

            foreach (var point in equipmentRefs.Points)
            {
                _equipmentPoints[point.Key] = point.Value;
            }
        }
    }
}
