using Unity.VisualScripting;
using UnityEngine;

namespace Client.Actor
{
    public class ActorView : MonoBehaviour, IActorView
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator animator;

        [SerializeField] private ActorSettings settings;

        [SerializeField] private ActorEquipmentReferences _equipmentRefs;
        [SerializeField] private AnimationEventHandler _animationEventHandler;

        private void Awake()
        {
            _animationEventHandler ??= gameObject.GetOrAddComponent<AnimationEventHandler>();
            _equipmentRefs ??= gameObject.GetOrAddComponent<ActorEquipmentReferences>();
        }

        public ActorEquipmentReferences EquipmentRefs
        {
            get => _equipmentRefs;
        }

        public ActorSettings Settings
        {
            get => settings;
        }

        public CharacterController CharacterController
        {
            get => characterController;
        }

        public Animator Animator
        {
            get => animator;
        }

        public Transform Transform
        {
            get => transform;
        }
    }
}
