using UnityEditor;
using UnityEngine;
using System.IO;

namespace TemplateEditor
{
    public class TemplateMenuItem
    {
        private enum Priority
        {
            CreateSetting,
            CreateGroupSetting,
            ResourcesLoadSupport,
            CustomEditorCreator,
            UnityTemplateChange,
            VisualTreeNameTable,
        }

        private const string MenuItemPrefix = "Tools/Template Editor/";
        private const string ScriptableObjectPrefix = MenuItemPrefix + "Setting Object/";
        private const string ToolsPrefix = MenuItemPrefix + "Tools/";
        private const int OriginalPriorityNumber = 1000;

        [MenuItem(ScriptableObjectPrefix + "Create Setting", false, OriginalPriorityNumber + (int)Priority.CreateSetting)]
        public static void CreateSetting()
        {
            CreateScriptableObject<TemplateSetting>(TemplateUtility.GetActiveFolder());
        }

        [MenuItem(ScriptableObjectPrefix + "Create Group Setting", false, OriginalPriorityNumber + (int)Priority.CreateGroupSetting)]
        public static void CreateGroupSetting()
        {
            CreateScriptableObject<TemplateGroupSetting>(TemplateUtility.GetActiveFolder());
        }

        [MenuItem(ToolsPrefix + "Create ResourcesLoader", false, OriginalPriorityNumber + (int)Priority.ResourcesLoadSupport)]
        public static void ExecuteResourcesLoadSupport()
        {
            ResourcesLoaderSetting.Execute();
        }

        [MenuItem(ToolsPrefix + "Open CustomEditor Creator", false, OriginalPriorityNumber + (int)Priority.CustomEditorCreator)]
        public static void OpenCustomEditorCreator()
        {
            CustomEditorCreateWindow.Open();
        }

        [MenuItem(ToolsPrefix + "Change Unity C# Template", false, OriginalPriorityNumber + (int)Priority.UnityTemplateChange)]
        public static void ExecuteChangeUniteTemplate()
        {
            UnityCSharpTemplatePathProcessor.Execute();
        }

#if UNITY_2019_1_OR_NEWER
        [MenuItem(ToolsPrefix + "Create VisualTreeName Table", false, OriginalPriorityNumber + (int)Priority.VisualTreeNameTable)]
        public static void ExecuteVisualTreeNameTable()
        {
            VisualTreeNameGetProcessor.Execute();
        }
#endif

        private static void CreateScriptableObject<T>(string dir) where T : ScriptableObject
        {
            var obj = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(obj, GetCreateScriptableObjectPath<T>(dir));
            AssetDatabase.Refresh();
        }

        private static string GetCreateScriptableObjectPath<T>(string dir)
        {
            var pathWithoutExtension = Path.Combine(dir, typeof(T).Name);
            var path = string.Format("{0}.asset", pathWithoutExtension);
            return AssetDatabase.GenerateUniqueAssetPath (path);
        }
    }
}
