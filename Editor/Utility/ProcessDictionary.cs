using System.Collections.Generic;

namespace TemplateEditor
{
    public class ProcessDictionary : Dictionary<string, object>
    {
        public static readonly string ConvertWordPattern = "{0}_{1}";

        public new void Add(string key, object value)
        {
            base.Add(ConvertReplaceWord(key), value);
        }

        public string ConvertReplaceWord(string word)
        {
            var convertWord = word;
            var counter = 2;
            while (ContainsKey(convertWord))
            {
                convertWord = string.Format(ConvertWordPattern, word, counter);
                counter++;
            }

            return convertWord;
        }

        public string GetLastConvertReplaceWord(string word)
        {
            string convertWord = word;
            int counter = 2;
            while (true)
            {
                var nextWord = string.Format(ConvertWordPattern, convertWord, counter);
                if (ContainsKey(nextWord))
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
}
