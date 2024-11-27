using Unity.Cinemachine;
using UnityEngine;

namespace Client.Camera
{
    public class FirstPersonCamera : BaseGameCamera
    {
        // 카메라 설정
        private const float LOOK_SENSITIVITY = 2.0f;
        private const float MIN_VERTICAL_ANGLE = -80f;
        private const float MAX_VERTICAL_ANGLE = 80f;
        private const float CAMERA_HEIGHT = 1.6f;
        private Transform _cameraRoot;

        private bool _isAiming;
        private float _rotationX;
        private CinemachineVirtualCamera _virtualCamera;

        public override void Initialize()
        {
            base.Initialize();

            CreateCameraRoot();
            SetupVirtualCamera();

            // 마우스 잠금
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void CreateCameraRoot()
        {
            var rootObject = new GameObject("FirstPersonCameraRoot");
            _cameraRoot = rootObject.transform;
            _cameraRoot.localPosition = new Vector3(0, CAMERA_HEIGHT, 0);
            _cameraRoot.localRotation = Quaternion.identity;
        }

        private void SetupVirtualCamera()
        {
            _virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            if (_virtualCamera == null)
            {
                var vcamObject = new GameObject("FirstPersonVCam");
                _virtualCamera = vcamObject.AddComponent<CinemachineVirtualCamera>();
            }

            _virtualCamera.Follow = _cameraRoot;
            _virtualCamera.LookAt = null; // 1인칭은 LookAt 사용하지 않음

            // 기본 FOV 설정
            _virtualCamera.m_Lens.FieldOfView = 60f;

            // 노이즈 컴포넌트 제거 (선택적으로 사용)
            var noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null) GameObject.Destroy(noise);

            // 컴포저 제거
            var composer = _virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null) GameObject.Destroy(composer);

            // 트랜스포저 설정
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
                    // 카메라가 타겟을 정확히 따라가도록 설정
                transposer.m_FollowOffset = Vector3.zero;
        }

        public override void UpdateCamera()
        {
            if (!_cameraRoot) return;

            // 마우스 입력
            var mouseX = Input.GetAxis("Mouse X") * LOOK_SENSITIVITY;
            var mouseY = Input.GetAxis("Mouse Y") * LOOK_SENSITIVITY;

            // 수직 회전 (상하)
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, MIN_VERTICAL_ANGLE, MAX_VERTICAL_ANGLE);
            _cameraRoot.localRotation = Quaternion.Euler(_rotationX, 0, 0);

            // // 수평 회전 (좌우) - 캐릭터 전체 회전
            // Target.Rotate(Vector3.up * mouseX);

            // 조준 처리
            HandleAiming();
        }

        private void HandleAiming()
        {
            if (Input.GetButtonDown("Fire2")) // 우클릭
            {
                _isAiming = true;
                if (_virtualCamera != null) _virtualCamera.m_Lens.FieldOfView = 40f;
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                _isAiming = false;
                if (_virtualCamera != null) _virtualCamera.m_Lens.FieldOfView = 60f;
            }
        }

        public override void HandleZoom(float zoomDelta)
        {
            if (!_isAiming || _virtualCamera == null) return;

            var currentFOV = _virtualCamera.m_Lens.FieldOfView;
            var newFOV = Mathf.Clamp(currentFOV - zoomDelta * ZOOM_SPEED, MIN_FOV, MAX_FOV);
            _virtualCamera.m_Lens.FieldOfView = newFOV;
        }

        public override Vector3 GetForward()
        {
            if (_cameraRoot == null) return Vector3.forward;
            var forward = _cameraRoot.forward;
            forward.y = 0;
            return forward.normalized;
        }

        public override void Release()
        {
            base.Release();

            // 마우스 잠금 해제
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (_cameraRoot != null) GameObject.Destroy(_cameraRoot.gameObject);

            _virtualCamera = null;
            _cameraRoot = null;
        }
    }
}