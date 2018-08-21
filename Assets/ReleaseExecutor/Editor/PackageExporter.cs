using UnityEngine;
using UnityEditor;
using System.IO;

namespace ReleaseExecutor
{
    public class PackageExporter : EditorWindow
    {
        public static void Open()
        {
            GetWindow<PackageExporter>(true);
        }

        private PackageExporterSetting _exportSetting;
        private Editor _editor;

        void OnGUI()
        {
            var newSetting = EditorGUILayout.ObjectField(_exportSetting, typeof(PackageExporterSetting), false) as PackageExporterSetting;
            if (newSetting == null)
            {
                return;
            }

            if (newSetting != _exportSetting)
            {
                _exportSetting = newSetting;
                _editor = Editor.CreateEditor(_exportSetting);
            }

            _editor.OnInspectorGUI();

            if (GUILayout.Button("エクスポート"))
            {
                RunExpotPackage(_exportSetting);
            }
        }

        public static void RunExpotPackage(PackageExporterSetting setting)
        {
            ExportPackage(setting.TargetPaths, setting.ExportName, setting.Options);
        }

        private static void ExportPackage(string[] files, string exportName, ExportPackageOptions[] optionsArray)
        {
            if (!CheckFilesExist(files))
            {
                return;
            }

            var exportFullName = AddExportExtension(exportName);

            ExportPackageOptions exportOptions = ExportPackageOptions.Default;
            foreach (var options in optionsArray)
            {
                exportOptions |= options;
            }

            AssetDatabase.ExportPackage(files, exportFullName, exportOptions);
            Debug.LogFormat("Exported to: {0}\nExported: {1}", exportFullName, string.Join("\n", files));
        }

        private static string[] AddAssetPrefix(string[] strings)
        {
            var paths = new string[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                paths[i] = "Assets/" + strings[i];
            }

            return paths;
        }

        public static string AddExportExtension(string exportName)
        {
            return exportName + ".unitypackage";
        }

        private static bool CheckFilesExist(string[] files)
        {
            var root = Application.dataPath;

            foreach (var file in files)
            {
                var path = Path.Combine(root, file);

                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    Debug.LogErrorFormat("'{0}' doesn't exist!", file);
                    return false;
                }
            }

            return true;
        }
    }
}