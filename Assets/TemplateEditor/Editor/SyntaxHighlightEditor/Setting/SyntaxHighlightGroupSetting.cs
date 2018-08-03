using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace SyntaxHighlightEditor
{
    public class SyntaxHighlightGroupSetting : ScriptableObject
    {
        public SyntaxHighlightSetting[] Settings;

        private Regex _regex;
        private MatchEvaluator _evaluator;

        public void Initialize()
        {
            var patterns = Settings.Select(setting => string.Format(
                setting.Format,
                setting.GetInstanceID(),
                string.Join("|", setting.Patterns))
            ).ToArray();

            var combinedPattern = string.Format("({0})", string.Join("|", patterns));

            _regex = new Regex(combinedPattern, RegexOptions.Compiled);
            _evaluator = match =>
            {
                foreach (var setting in Settings)
                {
                    // TODO : check ToString
                    if (match.Groups[setting.GetInstanceID().ToString()].Success)
                    {
                        return string.Format("<color=#{1}>{0}</color>", match.Value, setting.Color);
                    }
                }

                return match.Value;
            };
        }

        public string Highlight(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return _regex.Replace(text, _evaluator);
        }
    }
}
