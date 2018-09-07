using UnityEditor;
using UnityEngine;

namespace SyntaxHighlightEditor
{
    public class SyntaxHighlightUtility
    {
        private const int DefaultFontSize = 12;
        private static readonly int MinHeight = 100;
        private static readonly int MaxHeight = Screen.height;
        private static readonly string CSharpGroupSettingGuid = "d81e71d8a3f0b48438a3f3a25bfe3248";

        private static SyntaxHighlightGroupSetting _syntaxHighlightGroupSetting;
        private static TextAreaSyntaxHighlightEditor _editor;

        /// <summary>
        /// C#のコードエリアを表示
        /// </summary>
        public static string DrawCSharpCode(ref Vector2 scrollPos, string code, int flontSize = DefaultFontSize)
        {
            string editedCode;

            // TODO : Cache ?
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(MinHeight), GUILayout.MaxHeight(MaxHeight));
            {
                // TODO : Cache ?
                var style = new GUIStyle(GUI.skin.textArea);
                {
                    style.padding = new RectOffset(6, 6, 6, 6);
                    style.fontSize = flontSize;
                    style.wordWrap = false;
                }

                editedCode = GetCSharpEditor().Draw(code, style, GUILayout.ExpandHeight(true));
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
