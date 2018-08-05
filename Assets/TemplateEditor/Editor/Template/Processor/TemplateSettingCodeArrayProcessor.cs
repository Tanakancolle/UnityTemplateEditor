using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TemplateEditor
{
    public class TemplateSettingCodeArrayProcessor : TemplateSettingCodeProcessor
    {
        private static readonly string[] RepalceWords = {"TemplateSettingCodeArray"};

        #region IProcessChain implementation

        public override void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            var code = ReplaceCode(result);
            if (string.IsNullOrEmpty(code))
            {
                return;
            }

            result.Add(this.ConvertReplaceWord(RepalceWords[0], result), code.Split('\n'));
        }

        public override string[] GetReplaceWords()
        {
            return RepalceWords;
        }

        #endregion
    }
}
