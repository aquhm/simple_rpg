using System.Collections.Generic;
using Client.Camera;
using UnityEngine;

namespace Client.Service
{
    public class CameraService : IService
    {
        private readonly Dictionary<GameViewType, ICameraController> _cameras = new();
        private CinemachineBindings _bindings;
        private ICameraController _currentCamera;

        public UnityEngine.Camera MainCamra
        {
            get => UnityEngine.Camera.main;
        }

        public GameViewType CurrentViewType { get; private set; }

        public bool IsActive { get; private set; }

        public void Initialize()
        {
            _bindings = GameObject.FindFirstObjectByType<CinemachineBindings>();
            if (_bindings == null)
            {
                Debug.LogError("Scene에 CinemachineBindings가 없습니다!");
                return;
            }

            InitializeCameras();
            IsActive = true;
        }

        public void DoUpdate(float deltaTime)
        {
            _currentCamera?.UpdateCamera();
            HandleZoomInput();
        }

        public void Release()
        {
            if (_cameras != null)
            {
                foreach (var camera in _cameras.Values)
                {
                    camera.Release();
                }

                _cameras.Clear();
            }

            _currentCamera = null;
        }

        private void InitializeCameras()
        {
            foreach (var binding in _bindings.Bindings)
            {
                if (!_cameras.ContainsKey(binding.ViewType))
                {
                    var controller = CreateCameraController(binding.ViewType);
                    if (controller != null)
                    {
                        _cameras[binding.ViewType] = controller;
                    }
                }
            }
        }

        private ICameraController CreateCameraController(GameViewType viewType)
        {
            return viewType switch
            {
                    GameViewType.FirstPerson => new FirstPersonCamera(),
                    GameViewType.ThirdPerson => new ThirdPersonCamera(),
                    GameViewType.QuarterView => new QuarterViewCamera(),
                    _ => null
            };
        }

        public void SetCameraType(GameViewType viewType, bool immediate = false)
        {
            if (!_cameras.TryGetValue(viewType, out var controller))
            {
                Debug.LogWarning($"Camera controller for {viewType} not found!");
                return;
            }

            var bindingData = _bindings.FindCamera(viewType);
            if (!bindingData.HasValue)
            {
                Debug.LogWarning($"Camera binding for {viewType} not found in scene!");
                return;
            }

            _currentCamera?.Deactivate();

            _currentCamera = controller;
            CurrentViewType = viewType;

            if (controller is BaseGameCamera baseCamera)
            {
                baseCamera.InitializeWithBinding(bindingData.Value);
            }

            _currentCamera.Activate();

            // 즉시 전환이 아닌 경우 블렌딩 처리 가능
            if (!immediate)
            {
                // 카메라 블렌딩 로직 추가 가능
            }
        }

        private void HandleZoomInput()
        {
            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0)
            {
                _currentCamera?.HandleZoom(scrollDelta);
            }
        }

        public Vector3 GetCameraForward()
        {
            return _currentCamera?.GetForward() ?? Vector3.forward;
        }

        public Vector3 GetCameraRight()
        {
            return _currentCamera?.GetRight() ?? Vector3.right;
        }

        public Quaternion GetCameraRotation()
        {
            return _currentCamera?.GetRotation() ?? Quaternion.identity;
        }
    }
}
