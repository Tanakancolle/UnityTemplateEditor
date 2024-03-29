﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace TemplateEditor
{
    public class AssetsMenuItemProcessor : IProcessChain
    {
        private static readonly string AssetsMenuItemScriptName = "TemplateEditorAssetsMenuItem";

        private enum ReplaceWordType
        {
            Settings,
            GroupSettings,
            CreatePath,
            ScriptName,
        }

        private static readonly string[] ReplaceWords =
        {
            "Settings",
            "GroupSettings",
            "CreatePath",
            "ScriptName",
        };

        /// <summary>
        /// 生成を実行
        /// </summary>
        public static void Execute()
        {
            var setting = ToolExecutor.GetUseUserSetting().GetSetting(UserSetting.SettingType.AssetsMenuItemCreator);
            TemplateUtility.ExecuteSetting(setting);
        }

        private List<string[]> BuildMenuItemList<T>() where T : UnityEngine.Object
        {
            var list = new List<string[]>();
            foreach (var guid in TemplateUtility.FindAssetGuids(typeof(T)))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var setting = AssetDatabase.LoadAssetAtPath<T>(path) as IAssetsMenuItem;
                if (setting == null || setting.IsAssetsMenuItem == false)
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension(path);
                var array = new string[2];
                array[0] = name;
                array[1] = guid;
                list.Add(array);
            }

            return list;
        }

        public void Process(ProcessMetadata metadata, ProcessDictionary result)
        {
            var createPath = TemplateUtility.GetFilePathFromFileName(AssetsMenuItemScriptName + ".cs") ?? "Assets/TemplateEditorTool/Editor";
            result.Add(ReplaceWords[(int) ReplaceWordType.Settings], BuildMenuItemList<TemplateSetting>());
            result.Add(ReplaceWords[(int) ReplaceWordType.GroupSettings], BuildMenuItemList<TemplateGroupSetting>());
            result.Add(ReplaceWords[(int) ReplaceWordType.CreatePath], createPath);
            result.Add(ReplaceWords[(int) ReplaceWordType.ScriptName], AssetsMenuItemScriptName);
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "TemplateをAssetメニューへ追加するための情報を提供します";
        }
    }
}
