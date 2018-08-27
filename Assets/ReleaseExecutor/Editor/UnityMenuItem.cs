using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ReleaseExecutor
{
    public class UnityMenuItem
    {
        private enum Priority
        {
            ReleaseSetting,
            ReleaseWindow,
            ExportSetting = 20,
            ExportWindow,
            ExportAllPackage,
        }

        private const string MenuItemPrefix = "Tools/Release Executor/";
        private const int OriginalPriorityNumber = 3000;

        [MenuItem(MenuItemPrefix + "Create Release Setting", false, OriginalPriorityNumber + (int)Priority.ReleaseSetting)]
        public static void CreateReleaseSetting()
        {
            CreateScriptableObject<ReleaseExecutorSetting>();
        }

        [MenuItem(MenuItemPrefix + "Release Window", false, OriginalPriorityNumber + (int)Priority.ReleaseWindow)]
        public static void OpenReleaseWindow()
        {
            ReleaseExecutorWindow.Open();
        }

        [MenuItem(MenuItemPrefix + "Create Export Setting", false, OriginalPriorityNumber + (int)Priority.ExportSetting)]
        public static void CreateExportSetting()
        {
            CreateScriptableObject<PackageExporterSetting>();
        }

        [MenuItem(MenuItemPrefix + "Export Package Window", false, OriginalPriorityNumber + (int)Priority.ExportWindow)]
        public static void OpenExportPackageWindow()
        {
            PackageExporter.Open();
        }

        [MenuItem(MenuItemPrefix + "Export Package All", false, OriginalPriorityNumber + (int)Priority.ExportAllPackage)]
        public static void ExportAllPackage()
        {
            var guids = FindAssetGuids(typeof(PackageExporterSetting));
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                PackageExporter.RunExpotPackage(AssetDatabase.LoadAssetAtPath<PackageExporterSetting>(path));
            }
        }

        private static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            var path = Path.Combine(GetActiveFolder(), obj.name);
            ProjectWindowUtil.CreateAsset(obj, GetCreateScriptableObjectPath<T>(path));
            AssetDatabase.Refresh();
        }

        private static string GetCreateScriptableObjectPath<T>(string dir)
        {
            var pathWithoutExtension = Path.Combine(dir, typeof(T).Name);
            var path = string.Format("{0}.asset", pathWithoutExtension);
            return AssetDatabase.GenerateUniqueAssetPath (path);
        }

        private static string GetActiveFolder()
        {
            if (Selection.activeObject == null)
            {
                return "Assets";
            }

            var activeObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(activeObjectPath))
            {
                return activeObjectPath;
            }

            return Directory.GetParent(activeObjectPath).ToString();
        }

        private static string[] FindAssetGuids(Type type)
        {
            return AssetDatabase.FindAssets("t:" + type.Name);
        }
    }
}
