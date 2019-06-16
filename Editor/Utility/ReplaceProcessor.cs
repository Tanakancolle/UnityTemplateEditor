﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.Text;
using UnityEngine;

namespace TemplateEditor
{
    public static class ReplaceProcessor
    {
        private const string ReplacePrefix = "{<";
        private const string ReplaceSuffix = ">}";
        private static readonly Regex ReplaceRegex = new Regex(ReplacePrefix + "([\\s\\S]+?)" + ReplaceSuffix);
        private static readonly char SplitChar = ':';

        private static readonly Dictionary<string, Func<string[], object, string>> ProcessDic = new Dictionary<string, Func<string[], object, string>>()
        {
            {"Repeat", RepeatToProcess},
        };

        /// <summary>
        /// 置き換え文字置き換え ※{<Word>} 等
        /// </summary>
        public static string ReplaceProcess(string text, Dictionary<string, object> replace)
        {
            return ReplaceRegex.Replace(text, new MatchEvaluator((match) =>
                {
                    var orderText = match.Groups[1].Value;
                    var orders = orderText.Split(SplitChar);

                    // マッチするものがなければ次へ
                    var replaceWord = orders.Last();
                    if (replace.ContainsKey(replaceWord) == false)
                    {
                        return match.Value;
                    }

                    // 置き換え命令がなければ単純に置き換えのみ
                    if (orders.Length == 1 || ProcessDic.ContainsKey(orders[0]) == false)
                    {
                        return replace[replaceWord].ToString();
                    }

                    // 置き換え処理
                    return ProcessDic[orders[0]].Invoke(orders, replace[replaceWord]);
                }
            ));
        }

        public static HashSet<string> GetReplaceWords(params string[] texts)
        {
            var words = new HashSet<string>();
            foreach (var text in texts)
            {
                foreach (Match match in ReplaceRegex.Matches(text))
                {
                    words.Add(match.Groups[1].Value.Split(SplitChar).Last());
                }
            }

            return words;
        }

        public static string GetReplaceText(string word)
        {
            return ReplacePrefix + word + ReplaceSuffix;
        }

        public static bool Contains(string[] texts, string key)
        {
            foreach (var text in texts)
            {
                if (string.IsNullOrEmpty(text) == false && text.Contains(key))
                {
                    return true;
                }
            }

            return false;
        }

        #region Process

        private static string RepeatToProcess(string[] orders, object replace)
        {
            var objects = replace as System.Collections.IList;
            if (objects == null)
            {
                Debug.LogError("Repeat object is not list");
                return string.Empty;
            }

            // 最初と最後のSplitChar以外を一つに
            var builder = new StringBuilder();
            for (var i = 1; i < orders.Length - 1; ++i)
            {
                if (builder.Length != 0)
                {
                    builder.Append(SplitChar);
                }

                builder.Append(orders[i]);
            }

            var writeText = builder.ToString();
            builder.Clear();
            foreach (var obj in objects)
            {
                if (builder.Length != 0)
                {
                    builder.AppendLine();
                }

                if (obj is ICollection<object>)
                {
                    builder.Append(string.Format(writeText, ((ICollection<object>) obj).ToArray()));
                }
                else
                {
                    builder.Append(string.Format(writeText, obj));
                }
            }

            return builder.ToString();
        }

        #endregion
    }
}
