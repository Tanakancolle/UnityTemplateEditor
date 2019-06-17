using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace TemplateEditor
{
    public class UnityCSharpTemplatePathProcessor : IProcessChain
    {
        private static readonly string[] ReplaceWords =
        {
            "UnityTemplatePath",
            "UnityTemplateName",
        };

        public void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            result.Add(
                this.ConvertReplaceWord(ReplaceWords[0], result),
                Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates")
            );

            result.Add(
                this.ConvertReplaceWord(ReplaceWords[1], result),
                "81-C# Script-NewBehaviourScript.cs.txt"
            );
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "UnityのC#テンプレートパスを渡します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.Class;
        }
    }
}
