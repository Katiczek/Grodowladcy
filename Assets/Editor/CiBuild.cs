using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Grodowladcy.Editor
{
    /// <summary>
    /// Android APK build for /gh-release.
    /// - Batchmode: -executeMethod Grodowladcy.Editor.CiBuild.BuildAndroidApk
    /// - Open Editor: PowerShell drops Temp/gh-release-build.request; watcher builds and writes .result
    /// - Menu: Grodowladcy/Build Android APK (gh-release)
    /// </summary>
    public static class CiBuild
    {
        public const string ApkPath = "Builds/Android/Grodowladcy.apk";
        public const string RequestFile = "Temp/gh-release-build.request";
        public const string ResultFile = "Temp/gh-release-build.result";

        /// <summary>CLI / batchmode entry. Exits the Editor process.</summary>
        public static void BuildAndroidApk()
        {
            var ok = TryBuildAndroidApk(out var error);
            if (!ok)
            {
                Debug.LogError($"CiBuild: {error}");
                EditorApplication.Exit(1);
                return;
            }

            Debug.Log($"CiBuild: APK ok → {ApkPath}");
            EditorApplication.Exit(0);
        }

        [InitializeOnLoadMethod]
        private static void WatchGhReleaseBuildRequests()
        {
            EditorApplication.update -= PollBuildRequest;
            EditorApplication.update += PollBuildRequest;
        }

        private static void PollBuildRequest()
        {
            if (!File.Exists(RequestFile))
                return;

            try
            {
                File.Delete(RequestFile);
            }
            catch
            {
                return;
            }

            Debug.Log("CiBuild: gh-release request detected — building Android APK in open Editor...");
            var ok = TryBuildAndroidApk(out var error);
            try
            {
                var dir = Path.GetDirectoryName(ResultFile);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(ResultFile, ok ? "0" : "1");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"CiBuild: failed to write result file: {ex.Message}");
            }

            if (!ok)
                Debug.LogError($"CiBuild: {error}");
            else
                Debug.Log($"CiBuild: APK ok → {ApkPath}");
        }

        public static bool TryBuildAndroidApk(out string error)
        {
            error = null;
            EditorUserBuildSettings.buildAppBundle = false;

            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                error = "No enabled scenes in Build Settings.";
                return false;
            }

            var dir = Path.GetDirectoryName(ApkPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(ResultFile))
            {
                try { File.Delete(ResultFile); } catch { /* ignore */ }
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = ApkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                error = $"Android APK failed: {summary.result}";
                return false;
            }

            return true;
        }
    }
}
