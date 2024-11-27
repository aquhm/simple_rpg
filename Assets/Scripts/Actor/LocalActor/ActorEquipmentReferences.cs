using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Actor
{
    [Serializable]
    public class EquipmentReference
    {
        public string id;
        public GameObject gameObject;
    }

    [Serializable]
    public class EquipmentPoint
    {
        public string id;
        public Transform point;
    }

    public class ActorEquipmentReferences : MonoBehaviour
    {
        [Header("Equipment Points")]
        [SerializeField] private EquipmentPoint[] _equipPoints;

        [Header("Equipment Objects")]
        [SerializeField] private EquipmentReference[] _equipments;


        public Dictionary<string, Transform> Points { get; private set; }

        public Dictionary<string, GameObject> Equipments { get; private set; }

        private void Awake()
        {
            InitializeCaches();
        }

        private void InitializeCaches()
        {
            Points = new Dictionary<string, Transform>();
            Equipments = new Dictionary<string, GameObject>();

            foreach (var point in _equipPoints)
            {
                if (point.point != null)
                {
                    Points[point.id] = point.point;
                }
            }

            foreach (var equipment in _equipments)
            {
                if (equipment.gameObject != null)
                {
                    Equipments[equipment.id] = equipment.gameObject;
                }
            }
        }

        public Transform GetPoint(string pointId)
        {
            return Points.GetValueOrDefault(pointId);
        }

        public GameObject GetEquipment(string equipmentId)
        {
            return Equipments.GetValueOrDefault(equipmentId);
        }
    }
}
