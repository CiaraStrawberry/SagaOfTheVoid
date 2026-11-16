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
        Report(report, outPath);
    }

    // Browser build (WebGL 2): builds/Web/index.html. Brotli with a built-in fallback, so it runs from any
    // static web server without special headers. Needs the WebGL module for this editor.
    //   Unity -batchmode -quit -projectPath game -executeMethod BuildScript.BuildWeb
    [MenuItem("Saga/Build web (WebGL)")]
    public static void BuildWeb()
    {
        PlayerSettings.companyName = "RowlesCorp";
        PlayerSettings.productName = "Saga of the Void (remake preview)";
        // Brotli is ~15-20% smaller than gzip. With the fallback on, it also runs from servers that don't send
        // the Content-Encoding header (slower first load there); itch.io and most hosts send it.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.WebGL.memoryGrowthMode = WebGLMemoryGrowthMode.Geometric;
        PlayerSettings.WebGL.maximumMemorySize = 4096;
        // Our own page: full-window game, loading screen, full-screen button (Assets/WebGLTemplates/Saga).
        PlayerSettings.WebGL.template = "PROJECT:Saga";
        PlayerSettings.runInBackground = true;

        var root = Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));
        var outPath = Path.Combine(root, "builds", "Web");
        // Build beside the live folder and swap at the end, so a running web server never serves half-written files.
        var stage = Path.Combine(root, "builds", "Web_next");
        if (Directory.Exists(stage)) Directory.Delete(stage, true);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { SceneBuilder.SkirmishPath },
            locationPathName = stage,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        });
        if (report.summary.result == BuildResult.Succeeded)
        {
            if (Directory.Exists(outPath)) Directory.Delete(outPath, true);
            Directory.Move(stage, outPath);
        }
        Report(report, outPath);
    }

    static void Report(BuildReport report, string outPath)
    {
        var s = report.summary;
        Debug.Log(string.Format("BUILD result={0} size={1:N0} MB time={2:N0}s errors={3} warnings={4} -> {5}",
            s.result, s.totalSize / 1048576.0, s.totalTime.TotalSeconds, s.totalErrors, s.totalWarnings, outPath));
        if (s.result != BuildResult.Succeeded)
        {
            foreach (var step in report.steps)
                foreach (var m in step.messages)
                    if (m.type == LogType.Error) Debug.Log("BUILD error: " + m.content);
            EditorApplication.Exit(1);
        }
    }
}
