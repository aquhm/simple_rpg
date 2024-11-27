using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Client.Camera
{
    // 게임플레이 시점 타입
    public enum GameViewType
    {
        None,
        FirstPerson, // 1인칭 시점
        ThirdPerson, // 3인칭 시점 (FreeLook)
        QuarterView, // 쿼터뷰 시점
        TopDown, // 탑다운 시점
        TargetGroup, // 다중 타겟 시점
        Cinematic // 시네마틱 컷신용
    }

    [Serializable]
    public struct CinemachineBindingData
    {
        public GameViewType ViewType;
        public CinemachineCamera Camera;
        public Transform CameraTransform;
        public CinemachineTargetGroup TargetGroup;

        public bool IsValid => Camera != null;
    }

    public class CinemachineBindings : MonoBehaviour
    {
        [SerializeField] private CinemachineBindingData[] cameraBindings;
        public IReadOnlyList<CinemachineBindingData> Bindings => cameraBindings;

        public CinemachineBindingData? FindCamera(GameViewType viewType)
        {
            foreach (var binding in cameraBindings)
                if (binding.ViewType == viewType && binding.IsValid)
                    return binding;

            return null;
        }

        public bool HasCamera(GameViewType viewType)
        {
            return FindCamera(viewType).HasValue;
        }
    }
}