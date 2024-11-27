using UnityEngine;

namespace Client.Camera
{
    public enum CameraType
    {
        ThirdPerson,
        FirstPerson,
        QuarterView,
        TopDown
    }

    public interface ICameraController
    {
        void Initialize();
        void UpdateCamera();
        Vector3 GetForward();
        Vector3 GetRight();
        Quaternion GetRotation();
        void HandleZoom(float zoomDelta);
        void Release();
        void Activate();
        void Deactivate();
    }
}