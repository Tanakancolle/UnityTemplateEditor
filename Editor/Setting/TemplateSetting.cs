﻿using UnityEngine;
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

        [SerializeField]
        public string Path;

        [SerializeField]
        public string ScriptName;

        [SerializeField]
        public string Code;

        [SerializeField]
        public float CodeAreaMinHeight;

        [SerializeField]
        public float CodeAreaMaxHeight;

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

        [SerializeField]
        public string Description;

        [SerializeField]
        public List<bool> IsFoldouts;

        [SerializeField]
        public Vector2 ScrollPos;

        #region IAssetMenuItem implementation

        public bool IsAssetsMenuItem { get { return AssetsMenuItem; } set { AssetsMenuItem = value; } }

        #endregion
    }
}
