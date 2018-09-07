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

        public virtual void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            result.Add(this.ConvertReplaceWord(RepalceWords[0], result), ReplaceCode(result));
        }

        public virtual string[] GetReplaceWords()
        {
            return RepalceWords;
        }

        public string GetDescription()
        {
            return "一つ前のテンプレート設定のコード部分を渡します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.Class;
        }

        #endregion

        protected string ReplaceCode(Dictionary<string, object> result)
        {
            object obj;
            result.TryGetValue(this.GetLastConvertReplaceWord(TemplateSetting.ResultKey, result), out obj);
            var setting = obj as TemplateSetting;
            if (setting == null)
            {
                Debug.LogErrorFormat("Not {0} object", TemplateSetting.ResultKey);
                return null;
            }

            var status = new TemplateSettingStatus(new SerializedObject(setting));

            TemplateSettingEditor.ExecuteChain(status, result);
            var words = ReplaceProcessor.GetReplaceWords(
                status.GetProperty(TemplateSettingStatus.Property.Path).stringValue,
                status.GetProperty(TemplateSettingStatus.Property.ScriptName).stringValue,
                status.GetProperty(TemplateSettingStatus.Property.Code).stringValue
            );
            var replaces = TemplateSettingEditor.CreateReplaceList(new List<ReplaceInfo>(0), words.ToArray());
            foreach (var replace in replaces)
            {
                result.Add(replace.Key, replace.ReplaceWord);
            }

            return TemplateSettingEditor.Replace(
                status.GetProperty(TemplateSettingStatus.Property.Code).stringValue,
                result
            );
        }
    }
}
