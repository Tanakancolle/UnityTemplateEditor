using UnityEngine;
using UnityEditor;

namespace SyntaxHighlightEditor
{
    /// <summary>
    /// Code editor.
    /// </summary>
    public class TextAreaSyntaxHighlightEditor
    {
        public Color BackgroundColor { get; set; }

        public Color TextColor { get; set; }

        public System.Func<string, string> Highlighter { get; set; }

        public string CachedHighlightedCode { get; set; }

        private string _cachedCode;

        public TextAreaSyntaxHighlightEditor()
        {
            BackgroundColor = Color.black;
            TextColor = Color.white;
            Highlighter = code => code;
        }

        public string Draw(string code, GUIStyle style, params GUILayoutOption[] options)
        {
            // 文字を透明にする
            var backStyle = new GUIStyle(style);
            SetStyleTextColor(backStyle, Color.gray);

            // 現在の色を保存
            var preBackgroundColor = GUI.backgroundColor;
            var preColor = GUI.color;

            // 編集用のテキストエリアを描画
            GUI.backgroundColor = BackgroundColor;
            var editedCode = EditorGUILayout.TextArea(code, backStyle, options);

            // シンタックスハイライトさせたコードを更新
            if (editedCode != _cachedCode)
            {
                CachedHighlightedCode = Highlighter(editedCode);
                _cachedCode = editedCode;
            }

            // 文字（シンタックスハイライトされない部分）を指定色にする
            var forwardStyle = new GUIStyle(style);
            SetStyleTextColor(forwardStyle, TextColor);
            forwardStyle.richText = true;

            // シンタックスハイライト用のテキストエリアを表示
            GUI.backgroundColor = Color.clear;
            EditorGUI.TextArea(GUILayoutUtility.GetLastRect(), CachedHighlightedCode, forwardStyle);

            // 色を元に戻す
            GUI.backgroundColor = preBackgroundColor;
            GUI.color = preColor;

            return editedCode;
        }

        private void SetStyleTextColor(GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.hover.textColor = color;
            style.active.textColor = color;
        }
    }
}
