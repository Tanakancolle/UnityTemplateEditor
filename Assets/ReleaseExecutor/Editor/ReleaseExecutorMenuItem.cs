using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ReleaseExecutor
{
    public class ReleaseExecutorMenuItem
    {
        private enum Priority
        {
            CreateSetting,
            ExportWindow = 20,
            ExportAllPackage,
        }

        private const string MenuItemPrefix = "Tools/Release Executor/";
        private const int OriginalPriorityNumber = 1000;


        [MenuItem(MenuItemPrefix + "Create Setting", false, OriginalPriorityNumber + (int)Priority.CreateSetting)]
        public static void CreateSetting()
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
