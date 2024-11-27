using UnityEngine;

namespace Client.Camera
{
    public abstract class BaseGameCamera : ICameraController
    {
        protected const float MIN_FOV = 40f;
        protected const float MAX_FOV = 60f;
        protected const float ZOOM_SPEED = 20f;
        protected UnityEngine.Camera MainCamera;

        public Transform CameraTransform { get; private set; }

        public virtual void Initialize()
        {
            MainCamera = UnityEngine.Camera.main;
        }

        public abstract void UpdateCamera();


        public virtual Vector3 GetForward()
        {
            if (CameraTransform == null) return Vector3.forward;
            var forward = MainCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        public virtual Vector3 GetRight()
        {
            if (CameraTransform == null) return Vector3.right;
            var right = MainCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        public virtual Quaternion GetRotation()
        {
            return CameraTransform != null ? MainCamera.transform.rotation : Quaternion.identity;
        }

        public virtual void HandleZoom(float zoomDelta)
        {
        }

        public virtual void Release()
        {
            MainCamera = null;
            CameraTransform = null;
        }

        public void Activate()
        {
            CameraTransform?.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            CameraTransform?.gameObject.SetActive(false);
        }


        public virtual void InitializeWithBinding(CinemachineBindingData bindingData)
        {
            MainCamera = UnityEngine.Camera.main;
            CameraTransform = bindingData.CameraTransform;
            OnCameraInitialized(bindingData);
        }

        protected virtual void OnCameraInitialized(CinemachineBindingData bindingData)
        {
            // 각 카메라 타입별 초기화 구현
        }
    }
}