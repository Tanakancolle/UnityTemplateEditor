#pragma warning disable CS0649

using UnityEngine;

namespace TemplateEditor
{
    public class UserSetting : ScriptableObject
    {
        public enum SettingType
        {
            ResourcesLoader,
            UnityCSharpTemplate,
            VisualTreeName,
        }

        public enum GroupSettingType
        {
            CustomEditorCreator,
        }

        [EnumArray(typeof(SettingType))]
        [SerializeField]
        private TemplateSetting[] _settings;

        [EnumArray(typeof(GroupSettingType))]
        [SerializeField]
        private TemplateGroupSetting[] _groupSettings;

        [EnumArray(typeof(TemplateMenuItem.SimpleTemplatePriority))]
        [SerializeField]
        private TemplateSetting[] _simpleSettings;

        public TemplateSetting GetSetting(SettingType type)
        {
            return _settings[(int) type];
        }

        public TemplateGroupSetting GetSetting(GroupSettingType type)
        {
            return _groupSettings[(int) type];
        }

        public TemplateSetting GetSetting(TemplateMenuItem.SimpleTemplatePriority type)
        {
            return _simpleSettings[(int) type];
        }
    }
}