using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace TemplateEditor
{
    public class VisualTreeNameGetProcessor : IProcessChain
    {
        private static readonly string VisualTreeNameTableGuid = "92abed5f6c60344de969f42f7320a23b";
        private static readonly string ScriptName = "VisualTreeNameTable";

        private static readonly string[] ReplaceWords =
        {
            "CreatePath",
            "ScriptName",
            "Types",
            "Names",
        };

        public void Process(ProcessMetadata metadata, ProcessDictionary result)
        {
            var nameList = new HashSet<string>();
            foreach (var path in TemplateUtility.FindAssetPaths(typeof(VisualTreeAsset)))
            {
                if (path.StartsWith("Assets/") == false)
                {
                    continue;
                }
                var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                GetNames(treeAsset.CloneTree().Children(), nameList);
            }

            result.Add(ReplaceWords[0], TemplateUtility.GetFilePathFromFileName(ScriptName + ".cs") ?? "Assets");
            result.Add(ReplaceWords[1], ScriptName);
            result.Add(ReplaceWords[2], nameList.Select(StringBuilderExtension.ConvertEnumName).ToArray());
            result.Add(ReplaceWords[3], nameList.ToArray());
        }

        private void GetNames(IEnumerable<VisualElement> children, HashSet<string> nameList)
        {
            foreach (var child in children)
            {
                if (string.IsNullOrEmpty(child.name) == false)
                {
                    nameList.Add(child.name);
                }
                GetNames(child.Children(), nameList);
            }
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "Uxmlファイルから `name` 要素を取得します";
        }

        public static void Execute()
        {
            TemplateUtility.OpenEditorWindow(VisualTreeNameTableGuid);
        }
    }
}
