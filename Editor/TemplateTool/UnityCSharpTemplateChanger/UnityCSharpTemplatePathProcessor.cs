using UnityEditor;
using System.IO;

namespace TemplateEditor
{
    public class UnityCSharpTemplatePathProcessor : IProcessChain
    {
        private static readonly string[] ReplaceWords =
        {
            "UnityTemplatePath",
            "UnityTemplateName",
        };

        public void Process(ProcessMetadata metadata, ProcessDictionary result)
        {
            result.Add(ReplaceWords[0], Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates"));
            result.Add(ReplaceWords[1], "81-C# Script-NewBehaviourScript.cs.txt");
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "UnityのC#テンプレートパスを渡します";
        }
    }
}
