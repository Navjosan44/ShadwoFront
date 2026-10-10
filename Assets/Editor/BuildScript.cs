using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildScript
{
    public static void BuildAndroidAPK()
    {
        string outputPath = Path.Combine("Builds", "Shadowfront.apk");
        if (File.Exists(outputPath))
            File.Delete(outputPath);

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Main.unity" },
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(opts);
        BuildSummary summary = report.summary;
        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("BUILD SUCCESS: " + summary.totalSize + " bytes");
        }
        else
        {
            UnityEngine.Debug.LogError("BUILD FAILED: " + summary.result);
        }
    }
}
