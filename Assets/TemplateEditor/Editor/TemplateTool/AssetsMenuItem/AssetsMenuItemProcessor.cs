using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace TemplateEditor
{
    public class AssetsMenuItemProcessor : IProcessChain
    {
        private static readonly string AssetsMenuItemSettingGuid = "1c95e1425131048ba82aeb753fe906b4";
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
        public static void Create()
        {
            TemplateUtility.ExecuteSetting(AssetsMenuItemSettingGuid, new Dictionary<string, object>());
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

        public void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            var files = Directory.GetFiles("Assets", AssetsMenuItemScriptName + ".cs", SearchOption.AllDirectories);
            var createPath = files.Length > 0 ? Path.GetDirectoryName(files[0]) : "Assets/TemplateEditorTool/Editor";

            result.Add(this.ConvertReplaceWord(ReplaceWords[(int)ReplaceWordType.Settings], result), BuildMenuItemList<TemplateSetting>());
            result.Add(this.ConvertReplaceWord(ReplaceWords[(int)ReplaceWordType.GroupSettings], result), BuildMenuItemList<TemplateGroupSetting>());
            result.Add(this.ConvertReplaceWord(ReplaceWords[(int)ReplaceWordType.CreatePath], result), createPath);
            result.Add(this.ConvertReplaceWord(ReplaceWords[(int)ReplaceWordType.ScriptName], result), AssetsMenuItemScriptName);
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "TemplateをAssetメニューへ追加するための情報を提供します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.ScriptableObject;
        }
    }
}
