using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace UnityEssentials
{
    public static class CameraProvider
    {
        public static Camera Main { get; private set; }
        public static float Distance { get; private set; }
        public static float Height { get; private set; }

        [RuntimeInitializeOnLoadMethod()]
        public static void Initialize() =>
            PlayerLoopHook.Add<Update>(GetCurrentRenderingCameraInfo);

        public static void SetCameraInfo(Camera camera)
        {
            Distance = camera.transform.position.magnitude;
            Height = camera.transform.position.y;
            Main = camera;
        }

        private static void GetCurrentRenderingCameraInfo()
        {
#if UNITY_EDITOR
            // Prefer SceneView camera if available and focused
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView?.camera != null && sceneView.hasFocus)
            {
                SetCameraInfo(sceneView.camera);
                return;
            }
#endif
            // In Builds, use the main camera directly
            // Fallback to main camera
            if (Camera.main != null)
            {
                SetCameraInfo(Camera.main);
                return;
            }
        }
    }
}