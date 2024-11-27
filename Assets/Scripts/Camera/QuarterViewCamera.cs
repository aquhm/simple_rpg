using Unity.Cinemachine;
using UnityEngine;

namespace Client.Camera
{
    public class QuarterViewCamera : BaseGameCamera
    {
        private readonly float _rotationAngle = 45f;
        private readonly float _targetHeight = 10f;
        private float _targetDistance = 10f;
        private CinemachineVirtualCamera _virtualCamera;

        public override void Initialize()
        {
            _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            if (_virtualCamera != null) SetupQuarterViewCamera();
        }

        private void SetupQuarterViewCamera()
        {
            if (_virtualCamera != null)
            {
                _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                        new Vector3(0, _targetHeight, -_targetDistance);
                _virtualCamera.transform.rotation = Quaternion.Euler(_rotationAngle, 0, 0);
            }
        }

        public override void UpdateCamera()
        {
            if (_virtualCamera != null)
            {
                // var targetPosition = Target.position;
                // _virtualCamera.transform.position = targetPosition +
                //                                     Quaternion.Euler(_rotationAngle, 0, 0) *
                //                                     new Vector3(0, _targetHeight, -_targetDistance);
            }
        }

        public override void HandleZoom(float zoomDelta)
        {
            _targetDistance = Mathf.Clamp(_targetDistance - zoomDelta * ZOOM_SPEED, 5f, 15f);
            SetupQuarterViewCamera();
        }
    }
}