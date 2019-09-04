#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TemplateEditor
{
    public class VisualTreeNameGetProcessor : ScriptableObject, IProcessChain
    {
        private static readonly string ScriptName = "VisualTreeNameTable";

        private static readonly string[] ReplaceWords =
        {
            "CreatePath",
            "ScriptName",
            "Types",
            "Names",
            "Methods"
        };

        [SerializeField]
        private VisualTreeAsset[] _targets = new VisualTreeAsset[1];

        public void Process(ProcessMetadata metadata, ProcessDictionary result)
        {
            var typeNameList = new List<string[]>();
            var elementNameList = new List<string[]>();
            var nameList = new HashSet<string>();
            var targetNameList = new List<string>();
            foreach (var target in _targets)
            {
                if (target == null)
                {
                    continue;
                }

                GetNames(target.CloneTree().Children(), nameList);
                var tab = new string(' ', 8);
                typeNameList.Add(new[] {target.name, string.Join(",\n" + tab, nameList.Select(StringBuilderExtension.ConvertEnumName).ToArray())});
                elementNameList.Add(new[] {target.name, string.Join(",\n" + tab, nameList.Select(n => "\"" + n + "\"").ToArray())});
                nameList.Clear();

                targetNameList.Add(target.name);
            }

            result.Add(ReplaceWords[0], TemplateUtility.GetFilePathFromFileName(ScriptName + ".cs") ?? "Assets");
            result.Add(ReplaceWords[1], ScriptName);
            result.Add(ReplaceWords[2], typeNameList);
            result.Add(ReplaceWords[3], elementNameList);
            result.Add(ReplaceWords[4], targetNameList);
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
    }
}
#endif
