using UnityEngine;

namespace Client.Actor
{
    [CreateAssetMenu(fileName = "ActorSettings", menuName = "Game/ActorSettings")]
    public class ActorSettings : ScriptableObject
    {
        [Header("Movement Settings")] public float MoveSpeed = 1.5f;

        public float RunSpeedMultiplier = 3f;
        public float RotationSpeed = 2f;
        public float RotationSmoothTime = 0.1f;
        public float MovementThreshold = 0.1f;

        [Header("Jump Settings")] public float Gravity = -19.62f; // PlayerController의 값으로 수정

        public float JumpPower = 4.3f; // JumpForce를 JumpPower로 이름 변경 및 값 수정

        [Header("Ground Check")] public float GroundCheckRadius = 0.3f; // Physics.CheckSphere의 radius 값

        public LayerMask GroundMask; // 지면 체크를 위한 레이어 마스크

        [Header("Animation Layers")] public float DefenseLayerWeight = 1f; // 방어 애니메이션 레이어 가중치
    }
}
