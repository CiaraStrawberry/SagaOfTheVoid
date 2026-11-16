using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Windows test build of the skirmish.
//   Unity -batchmode -quit -projectPath game -executeMethod BuildScript.BuildWindows
// Output: builds/Skirmish/SagaRemake.exe (repo root, not committed)
public static class BuildScript
{
    [MenuItem("Saga/Build Windows test build")]
    public static void BuildWindows()
    {
        PlayerSettings.companyName = "RowlesCorp";
        PlayerSettings.productName = "Saga of the Void (remake preview)";
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1600;
        PlayerSettings.defaultScreenHeight = 900;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = true;

        var root = Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));
        var outPath = Path.Combine(root, "builds", "Skirmish", "SagaRemake.exe");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { SceneBuilder.SkirmishPath },
            locationPathName = outPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        });
        var s = report.summary;
        Debug.Log(string.Format("BUILD result={0} size={1:N0} MB time={2:N0}s errors={3} warnings={4} -> {5}",
            s.result, s.totalSize / 1048576.0, s.totalTime.TotalSeconds, s.totalErrors, s.totalWarnings, outPath));
        if (s.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
