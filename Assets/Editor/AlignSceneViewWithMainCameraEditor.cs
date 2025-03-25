using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AlignSceneViewWithMainCamera: EditorWindow
{
    private const string _TOOL_PATH = "Tools/Sjouke/";
    private const string _BUTTON_TEXT = "Align Scene View with MainCamera";

    // [MenuItem(_TOOL_PATH + "Enable")]
    // public static void Enable()
    // {
    // SceneView.duringSceneGui += OnSceneGUI;
    // }

    // [MenuItem(_TOOL_PATH + "Disable")]
    // public static void Disable()
    // {
    //     SceneView.duringSceneGui -= OnSceneGUI;
    // }

    [MenuItem(_TOOL_PATH + _BUTTON_TEXT)]
    private static void AlignWithMainCam()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
            SceneView.lastActiveSceneView.AlignViewToObject(mainCamera.transform);
        else
            Debug.LogWarning($"Cannot align SceneView camera when there's no camera with tag set to \"MainCamera\"!");

        // var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        // // Camera[] sceneViewCameras = SceneView.GetAllSceneCameras();

        // for (int index = 0; index < cameras.Length; index++)
        // {
        //     if (cameras[index].CompareTag("MainCamera"))
        //     {
        //         SceneView.lastActiveSceneView.AlignViewToObject(cameras[index].transform);
        //         return;
        //     }
        // }

        // Debug.LogWarning($"Cannot align SceneView camera when there's no camera with tag set to \"MainCamera\"!");
    }

    // private static void OnSceneGUI(SceneView sceneview)
    // {
    //     Handles.BeginGUI();

    //     if (GUILayout.Button(_BUTTON_TEXT))
    //     {
    //         if (Camera.main != null)
    //         {
    //             Transform mainCamTransform = Camera.main.transform;

    //             sceneview.camera.transform.SetPositionAndRotation(
    //                 mainCamTransform.position, 
    //                 mainCamTransform.rotation
    //             );
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"[{_BUTTON_TEXT}] pressed, but there's no Camera with tag set to \"MainCamera\"!");
    //         }
    //     }

    //     Handles.EndGUI();
    // }

    // private static void OnScene
}
