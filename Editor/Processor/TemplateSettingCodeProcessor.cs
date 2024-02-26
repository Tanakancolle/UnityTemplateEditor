using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TemplateEditor
{
    public class TemplateSettingCodeProcessor : IProcessChain
    {
        private static readonly string[] RepalceWords = {"TemplateSettingCode"};

        #region IProcessChain implementation

        public virtual void Process(ProcessMetadata metadata, ProcessDictionary result)
        {
            result.Add(RepalceWords[0], ReplaceCode(result));
        }

        public virtual string[] GetReplaceWords()
        {
            return RepalceWords;
        }

        public string GetDescription()
        {
            return "一つ前のテンプレート設定のコード部分を渡します";
        }

        #endregion

        protected string ReplaceCode(ProcessDictionary result)
        {
            object obj;
            result.TryGetValue(result.GetLastConvertReplaceWord(TemplateSetting.ResultKey), out obj);
            var setting = obj as TemplateSetting;
            if (setting == null)
            {
                Debug.LogErrorFormat("Not {0} object", TemplateSetting.ResultKey);
                return null;
            }

            var status = new TemplateSettingStatus(new SerializedObject(setting));

            TemplateSettingEditor.ExecuteChain(status, result);
            var words = ReplaceProcessor.GetReplaceWords(
                status.GetProperty(TemplateSettingPropertyGetter.Property.Path).stringValue,
                status.GetProperty(TemplateSettingPropertyGetter.Property.ScriptName).stringValue,
                status.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue
            );
            var replaces = TemplateSettingEditor.CreateReplaceList(new List<ReplaceInfo>(0), words.ToArray());
            foreach (var replace in replaces)
            {
                result.Add(replace.Key, replace.ReplaceWord);
            }

            return TemplateSettingEditor.Replace(
                status.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue,
                result
            );
        }
    }
}
