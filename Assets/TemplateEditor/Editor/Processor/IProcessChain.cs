using System.Collections.Generic;

namespace TemplateEditor
{
    public interface IProcessChain
    {
        void Process(ProcessMetadata metadata, Dictionary<string, object> result);
        string[] GetReplaceWords();
        string GetDescription();
        ProcessFileType GetFileType();
    }

    public enum ProcessFileType
    {
        Class,
        ScriptableObject,
        ClassAndScriptableObject,
    }

    /// <summary>
    /// IProcessChainで扱う拡張クラス
    /// </summary>
    public static class ProcessChainExtension
    {
        public static readonly string ConvertWordPattern = "{0}_{1}";

        public static string ConvertReplaceWord(this IProcessChain chain, string word, Dictionary<string, object> result)
        {
            string convertWord = word;
            int counter = 2;
            while (result.ContainsKey(convertWord))
            {
                convertWord = string.Format(ConvertWordPattern, convertWord, counter);
                counter++;
            }

            return convertWord;
        }

        public static string GetLastConvertReplaceWord(this IProcessChain chain, string word, Dictionary<string, object> result)
        {
            string convertWord = word;
            int counter = 2;
            while (true)
            {
                var nextWord = string.Format(ConvertWordPattern, convertWord, counter);
                if (result.ContainsKey(nextWord))
                {
                    convertWord = nextWord;
                }
                else
                {
                    break;
                }

                counter++;
            }

            return convertWord;
        }
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
