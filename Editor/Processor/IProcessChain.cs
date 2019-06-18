using System.Collections.Generic;

namespace TemplateEditor
{
    public interface IProcessChain
    {
        void Process(ProcessMetadata metadata, ProcessDictionary result);
        string[] GetReplaceWords();
        string GetDescription();
    }

    /// <summary>
    /// Process実行時のメタデータ
    /// </summary>
    public class ProcessMetadata
    {
        public TemplateSetting setting { get; private set; }

        public ProcessMetadata(TemplateSetting setting)
        {
            this.setting = setting;
        }
    }
}
