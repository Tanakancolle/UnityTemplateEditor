using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace TemplateEditor
{
    public class ProcessChainSampleProcessor : IProcessChain
    {
        private static readonly string[] ReplaceWords =
        {
            "ProcessChainSample",
        };

        public void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            result.Add(this.ConvertReplaceWord(ReplaceWords[0], result), "Test");
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "'Test' を受け渡します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.Class;
        }
    }
}
