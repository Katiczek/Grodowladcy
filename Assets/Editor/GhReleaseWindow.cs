using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Grodowladcy.Editor
{
    /// <summary>Interactive Unity UI for /gh-release (version + notes + APK + optional gh).</summary>
    public sealed class GhReleaseWindow : EditorWindow
    {
        string _currentVersion = "";
        string _proposedVersion = "";
        string _version = "";
        string _notes = "";
        string _status = "";
        bool _publishToGitHub = true;
        Vector2 _notesScroll;

        [MenuItem("Grodowladcy/gh-release...")]
        public static void Open()
        {
            var window = GetWindow<GhReleaseWindow>(true, "gh-release", true);
            window.minSize = new Vector2(420, 320);
            window.RefreshVersions();
            window.Show();
        }

        [MenuItem("Grodowladcy/Build Android APK only")]
        public static void BuildApkOnly()
        {
            if (!CiBuild.TryBuildAndroidApk(out var error))
                EditorUtility.DisplayDialog("Build APK", error, "OK");
            else
                EditorUtility.DisplayDialog("Build APK", $"APK OK:\n{CiBuild.ApkPath}", "OK");
        }

        void OnEnable() => RefreshVersions();

        void RefreshVersions()
        {
            _currentVersion = ReadBundleVersion();
            _proposedVersion = ProposeMinorBump(_currentVersion);
            if (string.IsNullOrWhiteSpace(_version))
                _version = _proposedVersion;
            _status = $"Current: {_currentVersion}  →  proposed minor: {_proposedVersion}";
        }

        void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Grodowładcy — Android playtest release", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Leave Version as proposed (minor bump) or type your own X.Y.Z. Notes become the GitHub Release body.",
                MessageType.Info);

            EditorGUILayout.LabelField(_status, EditorStyles.miniLabel);
            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            _version = EditorGUILayout.TextField("Version", _version);
            if (GUILayout.Button("Use proposed", GUILayout.Width(110)))
                _version = _proposedVersion;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Release notes");
            _notesScroll = EditorGUILayout.BeginScrollView(_notesScroll, GUILayout.Height(100));
            _notes = EditorGUILayout.TextArea(_notes, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            _publishToGitHub = EditorGUILayout.ToggleLeft("Publish with gh release create", _publishToGitHub);

            EditorGUILayout.Space(12);
            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_version)))
            {
                if (GUILayout.Button(_publishToGitHub ? "Build APK + GitHub Release" : "Build APK only", GUILayout.Height(36)))
                    Run();
            }
        }

        void Run()
        {
            var version = _version.Trim().TrimStart('v', 'V');
            if (!Regex.IsMatch(version, @"^\d+\.\d+\.\d+$"))
            {
                EditorUtility.DisplayDialog("gh-release", "Version must look like X.Y.Z", "OK");
                return;
            }

            if (_publishToGitHub && string.IsNullOrWhiteSpace(_notes))
            {
                EditorUtility.DisplayDialog("gh-release", "Release notes are required when publishing to GitHub.", "OK");
                return;
            }

            if (!WriteBundleVersion(version))
            {
                EditorUtility.DisplayDialog("gh-release", "Could not update bundleVersion in ProjectSettings.", "OK");
                return;
            }

            PlayerSettings.bundleVersion = version;
            _status = $"Building {version}...";
            Repaint();

            if (!CiBuild.TryBuildAndroidApk(out var error))
            {
                EditorUtility.DisplayDialog("gh-release", error, "OK");
                RefreshVersions();
                return;
            }

            if (!_publishToGitHub)
            {
                EditorUtility.DisplayDialog("gh-release", $"APK OK:\n{CiBuild.ApkPath}", "OK");
                RefreshVersions();
                return;
            }

            try
            {
                var url = CreateGitHubRelease(version, _notes.Trim());
                EditorUtility.DisplayDialog("gh-release", $"Published Grodowładcy v{version}\n\n{url}", "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog(
                    "gh-release",
                    $"APK built, but gh release failed:\n{ex.Message}\n\nAPK: {CiBuild.ApkPath}",
                    "OK");
            }

            RefreshVersions();
        }

        static string CreateGitHubRelease(string version, string notes)
        {
            var root = Directory.GetParent(Application.dataPath)?.FullName
                       ?? throw new InvalidOperationException("Cannot resolve project root.");
            var apk = Path.Combine(root, CiBuild.ApkPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(apk))
                throw new FileNotFoundException("APK missing after build.", apk);

            var tag = $"v{version}";
            var title = $"Grodowładcy v{version}";
            var notesFile = Path.Combine(Path.GetTempPath(), $"gh-release-notes-{version}.md");
            File.WriteAllText(notesFile, notes);

            var psi = new ProcessStartInfo
            {
                FileName = "gh",
                Arguments = $"release create \"{tag}\" \"{apk}\" --title \"{title}\" --notes-file \"{notesFile}\"",
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi)
                             ?? throw new InvalidOperationException("Failed to start gh. Is GitHub CLI on PATH?");
            var stdout = proc.StandardOutput.ReadToEnd();
            var stderr = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            try { File.Delete(notesFile); } catch { /* ignore */ }

            if (proc.ExitCode != 0)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(stderr) ? stdout : stderr);

            return string.IsNullOrWhiteSpace(stdout) ? $"https://github.com/ (tag {tag})" : stdout.Trim();
        }

        static string ReadBundleVersion()
        {
            var path = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "ProjectSettings", "ProjectSettings.asset");
            var text = File.ReadAllText(path);
            var m = Regex.Match(text, @"(?m)^\s*bundleVersion:\s*(.+)$");
            return m.Success ? m.Groups[1].Value.Trim() : PlayerSettings.bundleVersion;
        }

        static bool WriteBundleVersion(string version)
        {
            var path = Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, "ProjectSettings", "ProjectSettings.asset");
            var text = File.ReadAllText(path);
            var updated = Regex.Replace(text, @"(?m)^(\s*bundleVersion:\s*).+$", $"${{1}}{version}");
            if (updated == text)
                return false;
            File.WriteAllText(path, updated);
            return true;
        }

        static string ProposeMinorBump(string current)
        {
            var parts = current.Split('.');
            if (parts.Length < 2)
                return "0.1.0";
            var major = int.Parse(parts[0]);
            var minor = int.Parse(parts[1]) + 1;
            return $"{major}.{minor}.0";
        }
    }
}
