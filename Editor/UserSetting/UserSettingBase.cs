#pragma warning CS0649

using UnityEngine;

namespace TemplateEditor
{
    public abstract class UserSettingBase : ScriptableObject
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

        public TemplateSetting GetSetting(SettingType type)
        {
            return _settings[(int) type];
        }

        public TemplateGroupSetting GetSetting(GroupSettingType type)
        {
            return _groupSettings[(int) type];
        }
    }
}
