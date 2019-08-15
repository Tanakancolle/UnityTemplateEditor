using UnityEditor;
using UnityEngine;

namespace SyntaxHighlightEditor
{
    public static class SyntaxHighlightUtility
    {
        private const int DefaultFontSize = 12;
        private const float DefaultMinHeight = 100f;
        private static readonly string CSharpGroupSettingGuid = "d81e71d8a3f0b48438a3f3a25bfe3248";

        private static SyntaxHighlightGroupSetting _syntaxHighlightGroupSetting;
        private static TextAreaSyntaxHighlightEditor _editor;

        private static GUIStyle _style;

        /// <summary>
        /// C#のコードエリアを表示
        /// </summary>
        public static string DrawCSharpCode(ref Vector2 scrollPos, string code, int fontSize = DefaultFontSize, float minHeight = -1f, float maxHeight = -1f)
        {
            string editedCode;

            minHeight = minHeight <= 0f ? DefaultMinHeight : minHeight;
            maxHeight = maxHeight <= 0f ? Screen.height : maxHeight;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(minHeight), GUILayout.MaxHeight(maxHeight));
            {
                if (_style == null)
                {
                    _style = new GUIStyle(GUI.skin.textArea)
                    {
                        padding = new RectOffset(4, 4, 4, 4),
                        wordWrap = false,
                    };
                }
                _style.fontSize = fontSize;

                editedCode = GetCSharpEditor().Draw(code, _style, GUILayout.ExpandHeight(true));
            }
            EditorGUILayout.EndScrollView();

            return editedCode;
        }

        public static TextAreaSyntaxHighlightEditor GetCSharpEditor()
        {
            if (_syntaxHighlightGroupSetting == null)
            {
                var path = AssetDatabase.GUIDToAssetPath(CSharpGroupSettingGuid);
                _syntaxHighlightGroupSetting = AssetDatabase.LoadAssetAtPath<SyntaxHighlightGroupSetting>(path);
            }

            if (_editor == null)
            {
                _editor = new TextAreaSyntaxHighlightEditor();
                _editor.BackgroundColor = Color.gray;
                _editor.TextColor = Color.white;

                _syntaxHighlightGroupSetting.Initialize();
                _editor.Highlighter = _syntaxHighlightGroupSetting.Highlight;
            }

            return _editor;
        }

        public static void ResetCSharpEditor()
        {
            _editor = null;
        }
    }
}
