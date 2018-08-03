using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace TemplateEditor
{
    public class AssetsMenuItemCreator
    {
        private static readonly string AssetsMenuItemSettingGuid = "1c95e1425131048ba82aeb753fe906b4";
        private static readonly string AssetsMenuItemScriptName = "TemplateEditorAssetsMenuItem";

        public static void Create()
        {
            var files = Directory.GetFiles("Assets", AssetsMenuItemScriptName + ".cs", SearchOption.AllDirectories);
            var createPath = "Assets/TemplateEditorTool/Editor";
            if (files.Length > 0)
            {
                createPath = Path.GetDirectoryName(files[0]);
            }

            var result = new Dictionary<string, object>();
            result.Add("Settings", BuildMenuItemList<TemplateSetting>());
            result.Add("GroupSettings", BuildMenuItemList<TemplateGroupSetting>());
            result.Add("CreatePath", createPath);
            result.Add("ScriptName", AssetsMenuItemScriptName);

            TemplateUtility.ExecuteSetting(AssetsMenuItemSettingGuid, result);
        }

        private static List<string[]> BuildMenuItemList<T>() where T : UnityEngine.Object
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
    }
}
