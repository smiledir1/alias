using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Utils.Editor;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Common.CheckCode.Editor
{
    public class CheckCodeWindow : EditorWindow
    {
        private const string PathToCliKey = "_pathToCliKey";
        private const string ProjectNameKey = "_projectNameKey";
        private const string InspectFormatKey = "_inspectFormatKey";
        private const string CleanupFormatKey = "_cleanupFormat";
        private const string InspectSeverityKey = "_inspectSeverityKey";
        private const string OutputFileNameKey = "_outputFileNameKey";
        private const string ExcludePathsKey = "_excludePathsKey";

        private static CheckCodeWindow _window;

        private static string _pathToCli;
        private static string _projectName;
        private static string _inspectFormat;
        private static string _cleanupFormat;
        private static string _inspectSeverity;
        private static string _outputFileName;
        private static List<string> _excludePaths;
        private static bool _excludePathsExpanded;
        private static GUIStyle _boldLabel;

        private static int _inspectFormatIndex;
        private static int _inspectSeverityIndex;
        private static int _cleanupFormatIndex;
        private static readonly string[] InspectFormats = {"Xml", "Text"};
        private static readonly string[] InspectSeverityLevels = {"ERROR", "WARNING", "SUGGESTION", "HINT", "NONE"};

        private static readonly string[] CleanupFormats =
        {
            "Built-in: Reformat Code",
            "Built-in: Reformat & Apply Syntax Style",
            "Built-in: Full Cleanup"
        };

        [MenuItem("Tools/CheckCode/CheckCode Window")]
        private static void InitializeWindow()
        {
            _window = GetWindow<CheckCodeWindow>();
            _window.Show();

            Initialize();
        }

        private void OnGUI()
        {
            if (_window == null) InitializeWindow();

            DrawOptions();

            if (GUILayout.Button("Open Cli Url")) OpenCliUrl();
            if (GUILayout.Button("Inspect Code")) InspectCode();
            if (GUILayout.Button("Cleanup Code")) CleanupCode();
        }

        private static void Initialize()
        {
            _pathToCli = EditorPrefs.GetString(PathToCliKey);
            _projectName = EditorPrefs.GetString(ProjectNameKey);

            _inspectFormat = EditorPrefs.GetString(InspectFormatKey);
            _inspectFormatIndex = ArrayUtility.IndexOf(InspectFormats, _inspectFormat);
            if (_inspectFormatIndex == -1) _inspectFormatIndex = 0;

            _inspectSeverity = EditorPrefs.GetString(InspectSeverityKey);
            _inspectSeverityIndex = ArrayUtility.IndexOf(InspectSeverityLevels, _inspectSeverity);
            if (_inspectSeverityIndex == -1) _inspectSeverityIndex = 0;

            _cleanupFormat = EditorPrefs.GetString(CleanupFormatKey);
            _cleanupFormatIndex = ArrayUtility.IndexOf(CleanupFormats, _cleanupFormat);
            if (_cleanupFormatIndex == -1) _cleanupFormatIndex = 0;

            _outputFileName = EditorPrefs.GetString(OutputFileNameKey);
            var excludePaths = EditorPrefs.GetString(ExcludePathsKey);
            _excludePaths = excludePaths.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            _excludePathsExpanded = false;

            _boldLabel = new GUIStyle("Label")
            {
                fontStyle = FontStyle.Bold
            };
        }

        private void DrawOptions()
        {
            _pathToCli = EditorGUILayout.TextField("Path to CLI", _pathToCli);
            _projectName = EditorGUILayout.TextField("Project Name (with .sln)", _projectName);
            _excludePathsExpanded = EditorUtils.DrawList(_excludePaths, _excludePathsExpanded, "ExcludePaths");

            GUILayout.Label("Inspection parameters", _boldLabel);
            _inspectFormatIndex = EditorGUILayout.Popup("Inspection output format", _inspectFormatIndex,
                InspectFormats);
            _inspectFormat = InspectFormats[_inspectFormatIndex];
            _inspectSeverityIndex = EditorGUILayout.Popup("Inspection severity", _inspectSeverityIndex,
                InspectSeverityLevels);
            _inspectSeverity = InspectSeverityLevels[_inspectSeverityIndex];
            _outputFileName = EditorGUILayout.TextField("Output File Name", _outputFileName);

            GUILayout.Label("Cleanup parameters", _boldLabel);
            _cleanupFormat = CleanupFormats[_cleanupFormatIndex];
            _cleanupFormatIndex = EditorGUILayout.Popup("Cleanup format", _cleanupFormatIndex,
                CleanupFormats);

            if (GUILayout.Button("Save Options")) SaveOptions();
        }

        private void SaveOptions()
        {
            EditorPrefs.SetString(PathToCliKey, _pathToCli);
            EditorPrefs.SetString(ProjectNameKey, _projectName);
            EditorPrefs.SetString(InspectFormatKey, _inspectFormat);
            EditorPrefs.SetString(CleanupFormatKey, _cleanupFormat);
            EditorPrefs.SetString(InspectSeverityKey, _inspectSeverity);
            EditorPrefs.SetString(OutputFileNameKey, _outputFileName);
            var excludePaths = string.Join(";", _excludePaths);
            EditorPrefs.SetString(ExcludePathsKey, excludePaths);
        }

        private void OpenCliUrl()
        {
            Application.OpenURL("https://www.jetbrains.com/ru-ru/resharper/download/#section=web-installer");
        }

        private void InspectCode()
        {
            var excludePaths = string.Join(";", _excludePaths);
            var args = $"{_projectName} -format={_inspectFormat} --no-swea -o=\"{_outputFileName}\" " +
                       $"--severity={_inspectSeverity} --exclude=\"{excludePaths}\" -x=JetBrains.Unity --no-build";
            Debug.Log(args);

            var fileName = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    fileName = Path.Combine(_pathToCli, "InspectCode.exe");
                    break;
                case RuntimePlatform.OSXEditor:
                    // TODO: for MAC
                    break;
                default:
                    return;
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            ReadLines(proc).Forget();
        }

        private async UniTask ReadLines(Process proc)
        {
            while (!proc.StandardOutput.EndOfStream)
            {
                var line = await proc.StandardOutput.ReadLineAsync();
                Debug.Log(line);
                await UniTask.Yield();
            }

            EditorUtility.RevealInFinder(_outputFileName);
        }

        private void CleanupCode()
        {
            var excludePaths = string.Join(";", _excludePaths);
            var args =
                $"{_projectName} --profile=\"{_cleanupFormat}\" --exclude=\"{excludePaths}\" -x=JetBrains.Unity --no-build";
            Debug.Log(args);

            var fileName = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    fileName = Path.Combine(_pathToCli, "cleanupcode.exe");
                    break;
                case RuntimePlatform.OSXEditor:
                    // TODO: for MAC
                    break;
                default:
                    return;
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            ReadLines(proc).Forget();
        }
    }
}