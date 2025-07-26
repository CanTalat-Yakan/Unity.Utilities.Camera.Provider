using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace UnityEssentials
{
    public static class CameraProvider
    {
        public static Camera Active { get; private set; }
        public static float Distance { get; private set; }
        public static float Height { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize() =>
            PlayerLoopHook.Add<Update>(GetCurrentRenderingCameraInfo);

        public static void SetCameraInfo(Camera camera)
        {
            if (camera == null)
                return;

            Distance = camera.transform.position.magnitude;
            Height = camera.transform.position.y;
            Active = camera;
        }

        private static void GetCurrentRenderingCameraInfo()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetCameraInfo(GetSceneViewCamera());
                return;
            }
#endif
            SetCameraInfo(GetActiveCamera());
        }

        private static Camera GetSceneViewCamera()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
                if (sceneView.hasFocus)
                    return sceneView.camera;
            return null;
        }

        private static Camera GetActiveCamera()
        {
            if (Camera.main != null)
                return Camera.main;
            if (Camera.current != null)
                return Camera.current;
            foreach (var camera in Camera.allCameras)
                if (camera.targetTexture != null)
                    return camera;
            if (Camera.allCamerasCount > 0)
                return Camera.allCameras[0];
            return null;
        }
    }
}