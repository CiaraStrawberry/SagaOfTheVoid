using System.Linq;
using Saga.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Creates Assets/Scenes/Skirmish.unity: a camera with the SkirmishController (it builds the rest at start).
//   Unity -batchmode -quit -projectPath game -executeMethod SceneBuilder.BuildSkirmish
public static class SceneBuilder
{
    public const string SkirmishPath = "Assets/Scenes/Skirmish.unity";

    [MenuItem("Saga/Rebuild Skirmish scene")]
    public static void BuildSkirmish()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(UniversalAdditionalCameraData));
        go.tag = "MainCamera";
        go.AddComponent<SkirmishController>();
        EditorSceneManager.SaveScene(scene, SkirmishPath);

        var list = EditorBuildSettings.scenes.Where(s => s.path != SkirmishPath).ToList();
        list.Insert(0, new EditorBuildSettingsScene(SkirmishPath, true));
        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("SCENE built " + SkirmishPath);
    }
}
