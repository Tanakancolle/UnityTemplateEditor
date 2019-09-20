using System;
using UnityEditor;

namespace TemplateEditor
{
    public static class ToolExecutor
    {
        private static readonly string DefaultSettingGuid = "18ecd24fd3c0a472096ada88c3a417ad";

        public static UserSettingBase GetUseUserSetting()
        {
            var paths = TemplateUtility.FindAssetPaths(typeof(UserSetting));
            var targetPath = string.Empty;
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    targetPath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                targetPath = AssetDatabase.GUIDToAssetPath(DefaultSettingGuid);
            }

            return AssetDatabase.LoadAssetAtPath<UserSettingBase>(targetPath);
        }

        public static void Execute(TemplateMenuItem.ToolPriority type)
        {
            var userSetting = GetUseUserSetting();
            switch (type)
            {
                case TemplateMenuItem.ToolPriority.ResourcesLoadSupport:
                    TemplateUtility.ExecuteSetting(userSetting.GetSetting(UserSettingBase.SettingType.ResourcesLoader));
                    break;
                case TemplateMenuItem.ToolPriority.CustomEditorCreator:
                    CustomEditorCreateWindow.Open();
                    break;
                case TemplateMenuItem.ToolPriority.UnityTemplateChange:
                    TemplateUtility.OpenEditorWindow(userSetting.GetSetting(UserSettingBase.SettingType.UnityCSharpTemplate));
                    break;
                case TemplateMenuItem.ToolPriority.VisualTreeNameTableCreator:
                    VisualTreeNameCreatorWindow.Open();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}