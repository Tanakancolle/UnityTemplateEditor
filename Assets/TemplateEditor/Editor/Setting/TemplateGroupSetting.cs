using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace TemplateEditor
{
    public class TemplateGroupSetting : ScriptableObject, IAssetsMenuItem
    {
        [SerializeField]
        public TemplateSetting[] Settings;

        [SerializeField]
        private bool AssetsMenuItem;

        #region IAssetsMenuItem implementation

        public bool IsAssetsMenuItem { get { return AssetsMenuItem; } set { AssetsMenuItem = value; } }

        #endregion

        [SerializeField]
        private string Description;

        [NonSerialized]
        public Action OnChangedSettings;
    }
}
