using Unity.Cinemachine;
using UnityEngine;

namespace Client.Camera
{
    public class ThirdPersonCamera : BaseGameCamera
    {
        private CinemachineFreeLook _freeLookCamera;

        public override void Initialize()
        {
            _freeLookCamera = Object.FindFirstObjectByType<CinemachineFreeLook>();
            if (_freeLookCamera != null) SetupFreeLookCamera();
        }

        private void SetupFreeLookCamera()
        {
            _freeLookCamera.m_XAxis.m_MaxSpeed = 300f;
            _freeLookCamera.m_YAxis.m_MaxSpeed = 2f;

            // 오빗 설정
            SetOrbitSettings(_freeLookCamera.m_Orbits[0], 4.5f, 180f); // Top Rig
            SetOrbitSettings(_freeLookCamera.m_Orbits[1], 2.5f, 180f); // Middle Rig
            SetOrbitSettings(_freeLookCamera.m_Orbits[2], 0.5f, 180f); // Bottom Rig
        }

        private void SetOrbitSettings(CinemachineFreeLook.Orbit orbit, float height, float radius)
        {
            orbit.m_Height = height;
            orbit.m_Radius = radius;
        }

        public override void HandleZoom(float zoomDelta)
        {
            if (zoomDelta != 0 && _freeLookCamera != null)
            {
                var currentFOV = _freeLookCamera.m_Lens.FieldOfView;
                var newFOV = Mathf.Clamp(currentFOV - zoomDelta * ZOOM_SPEED, MIN_FOV, MAX_FOV);
                _freeLookCamera.m_Lens.FieldOfView = newFOV;
            }
        }

        public override void UpdateCamera()
        {
            // 추가적인 카메라 업데이트 로직
        }

        public override void Release()
        {
            base.Release();
            _freeLookCamera = null;
        }
    }
}