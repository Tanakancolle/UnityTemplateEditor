using System;
using UnityEditor;

namespace TemplateEditor
{
    public static class ToolExecutor
    {
        private static readonly string DefaultSettingGuid = "18ecd24fd3c0a472096ada88c3a417ad";

        public static bool CheckCustomSetting()
        {
            var guids = TemplateUtility.FindAssetGuids(typeof(UserSetting));
            return guids.Length > 1;
        }

        public static UserSetting GetUseUserSetting()
        {
            var guids = TemplateUtility.FindAssetGuids(typeof(UserSetting));
            var targetPath = string.Empty;
            if (guids != null)
            {
                foreach (var guid in guids)
                {
                    if (guid == DefaultSettingGuid)
                    {
                        continue;
                    }

                    targetPath = AssetDatabase.GUIDToAssetPath(guid);
                    break;
                }
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                targetPath = AssetDatabase.GUIDToAssetPath(DefaultSettingGuid);
            }

            return AssetDatabase.LoadAssetAtPath<UserSetting>(targetPath);
        }

        public static void Execute(TemplateMenuItem.ToolPriority type)
        {
            var userSetting = GetUseUserSetting();
            switch (type)
            {
                case TemplateMenuItem.ToolPriority.ResourcesLoadSupport:
                    TemplateUtility.ExecuteSetting(userSetting.GetSetting(UserSetting.SettingType.ResourcesLoader));
                    break;
                case TemplateMenuItem.ToolPriority.CustomEditorCreator:
                    CustomEditorCreateWindow.Open();
                    break;
                case TemplateMenuItem.ToolPriority.UnityTemplateChange:
                    TemplateUtility.OpenEditorWindow(userSetting.GetSetting(UserSetting.SettingType.UnityCSharpTemplate));
                    break;
                case TemplateMenuItem.ToolPriority.VisualTreeNameTableCreator:
                    VisualTreeNameCreatorWindow.Open();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void ExecuteSimple(TemplateMenuItem.SimpleTemplatePriority type)
        {
            var userSetting = GetUseUserSetting();
            var setting = userSetting.GetSetting(type);
            TemplateUtility.OpenEditorWindow(setting);
        }
    }
}