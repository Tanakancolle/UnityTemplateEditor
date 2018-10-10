using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.Serialization;

namespace TemplateEditor
{
    public class TemplateSetting : ScriptableObject, IProcessChain, IAssetsMenuItem
    {
        public static readonly string ResultKey = "TemplateSetting";
        private static readonly string[] ReplaceWords = {ResultKey};

        public void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            result.Add(
                this.ConvertReplaceWord(ReplaceWords[0], result),
                this
            );
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "自身のインスタンスを渡します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.ScriptableObject;
        }

        [SerializeField]
        public string Path;

        [SerializeField]
        public string ScriptName;

        [SerializeField]
        public string Code;

        [SerializeField]
        public TemplateUtility.OverwriteType Overwrite;

        [SerializeField]
        public UnityEngine.Object[] Chain;

        [SerializeField]
        public GameObject DuplicatePrefab;

        [SerializeField]
        public GameObject AttachTarget;

        [SerializeField]
        public string PrefabPath;

        [SerializeField]
        public string PrefabName;

        [SerializeField]
        private bool AssetsMenuItem;

        #region IAssetMenuItem implementation

        public bool IsAssetsMenuItem { get { return AssetsMenuItem; } set { AssetsMenuItem = value; } }

        #endregion

        [SerializeField]
        public string Description;
    }
}
