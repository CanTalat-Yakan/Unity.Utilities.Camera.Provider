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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() =>
            PlayerLoopHook.Add<Update>(GetCurrentRenderingCameraInfo);

        private static void GetCurrentRenderingCameraInfo()
        {
#if UNITY_EDITOR
            // Prefer SceneView camera if available and focused
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null && sceneView.hasFocus)
            {
                Distance = sceneView.camera.transform.position.magnitude;
                Height = sceneView.camera.transform.position.y;
                Main = sceneView.camera;
                return;
            }
#else
            // In Builds, use the main camera directly
            if (Camera.main != null && Main == null)
            {
                CameraDistance = Camera.main.transform.position.magnitude;
                CameraHeight = Camera.main.transform.position.y;
                Main = Camera.main;
                return;
            }
#endif
            // Fallback to main camera
            if (Camera.main != null)
            {
                Distance = Camera.main.transform.position.magnitude;
                Height = Camera.main.transform.position.y;
                Main = Camera.main;
                return;
            }
        }

    }
}
