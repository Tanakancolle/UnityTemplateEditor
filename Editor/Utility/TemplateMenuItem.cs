using UnityEditor;
using UnityEngine;
using System.IO;

namespace TemplateEditor
{
    public static class TemplateMenuItem
    {
        private enum ScriptableObjectPriority
        {
            CreateSetting,
            CreateGroupSetting,
        }

        public enum ToolPriority
        {
            ResourcesLoadSupport,
            CustomEditorCreator,
            UnityTemplateChange,
            VisualTreeNameTableCreator,
        }

        public enum SimpleTemplatePriority
        {
            Class,
            MonoClass,
            EditorWindow,
            ScriptableObject,
        }

        private const string MenuItemPrefix = "Tools/Template Editor/";
        private const string ScriptableObjectPrefix = MenuItemPrefix + "Setting Object/";
        private const string ToolsPrefix = MenuItemPrefix + "Tools/";
        private const string SimpleToolsPrefix = "Assets/Create/Template/Default/";
        private const int OriginalPriorityNumber = 1000;
        private const int CreateCustomTemplateNumber = OriginalPriorityNumber + 100;

        #region Create Setting

        [MenuItem(ScriptableObjectPrefix + "Create Setting", false, OriginalPriorityNumber + (int)ScriptableObjectPriority.CreateSetting)]
        public static void CreateSetting()
        {
            CreateScriptableObject<TemplateSetting>(TemplateUtility.GetActiveFolder());
        }

        [MenuItem(ScriptableObjectPrefix + "Create Group Setting", false, OriginalPriorityNumber + (int)ScriptableObjectPriority.CreateGroupSetting)]
        public static void CreateGroupSetting()
        {
            CreateScriptableObject<TemplateGroupSetting>(TemplateUtility.GetActiveFolder());
        }

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

        #endregion

        #region Tool

        [MenuItem(ToolsPrefix + "Create ResourcesLoader", false, OriginalPriorityNumber + (int)ToolPriority.ResourcesLoadSupport)]
        public static void ExecuteResourcesLoadSupport()
        {
            ToolExecutor.Execute(ToolPriority.ResourcesLoadSupport);
        }

        [MenuItem(ToolsPrefix + "Open CustomEditor Creator", false, OriginalPriorityNumber + (int)ToolPriority.CustomEditorCreator)]
        public static void OpenCustomEditorCreator()
        {
            ToolExecutor.Execute(ToolPriority.CustomEditorCreator);
        }

        [MenuItem(ToolsPrefix + "Change Unity C# Template", false, OriginalPriorityNumber + (int)ToolPriority.UnityTemplateChange)]
        public static void ExecuteChangeUniteTemplate()
        {
            ToolExecutor.Execute(ToolPriority.UnityTemplateChange);
        }

#if UNITY_2019_1_OR_NEWER
        [MenuItem(ToolsPrefix + "Open VisualTreeName Creator", false, OriginalPriorityNumber + (int)ToolPriority.VisualTreeNameTableCreator)]
        public static void ExecuteVisualTreeNameTable()
        {
            ToolExecutor.Execute(ToolPriority.VisualTreeNameTableCreator);
        }
#endif

        #endregion

        #region Simple Tool

        [MenuItem(SimpleToolsPrefix + "Class", false, OriginalPriorityNumber + (int)SimpleTemplatePriority.Class)]
        public static void ExecuteCreateClass()
        {
            ToolExecutor.ExecuteSimple(SimpleTemplatePriority.Class);
        }

        [MenuItem(SimpleToolsPrefix + "Mono Class", false, OriginalPriorityNumber + (int)SimpleTemplatePriority.MonoClass)]
        public static void ExecuteCreateMonoClass()
        {
            ToolExecutor.ExecuteSimple(SimpleTemplatePriority.MonoClass);
        }

        [MenuItem(SimpleToolsPrefix + "ScriptableObject", false, OriginalPriorityNumber + (int)SimpleTemplatePriority.ScriptableObject)]
        public static void ExecuteCreateScriptableObject()
        {
            ToolExecutor.ExecuteSimple(SimpleTemplatePriority.ScriptableObject);
        }

        [MenuItem(SimpleToolsPrefix + "Editor Window", false, OriginalPriorityNumber + (int)SimpleTemplatePriority.EditorWindow)]
        public static void ExecuteCreate()
        {
            ToolExecutor.ExecuteSimple(SimpleTemplatePriority.EditorWindow);
        }

        #endregion

        [MenuItem(MenuItemPrefix + "Create Custom Template", false, CreateCustomTemplateNumber)]
        public static void ExecuteCustomCreate()
        {
            UserSettingCopyWindow.Open();
        }
    }
}
