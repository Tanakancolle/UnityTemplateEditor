using UnityEngine;
using UnityEditor;

namespace SyntaxHighlightEditor
{
    /// <summary>
    /// Code editor.
    /// </summary>
    /// <remarks>
    /// https://github.com/hecomi/uRaymarching
    /// </remarks>
    public class TextAreaSyntaxHighlightEditor
    {
        public Color backgroundColor { get; set; }

        public Color textColor { get; set; }

        // ハイライト用の関数
        public System.Func<string, string> highlighter { get; set; }

        // 表示高速化の為に変更があった時だけコードを更新
        string cachedHighlightedCode { get; set; }

        private string cachedCode;

        public TextAreaSyntaxHighlightEditor()
        {
            backgroundColor = Color.black;
            textColor = Color.white;
            highlighter = code => code;
        }

        public string Draw(string code, GUIStyle style, params GUILayoutOption[] options)
        {
            // 現在の色を保存
            var preBackgroundColor = GUI.backgroundColor;
            var preColor = GUI.color;

            // 文字を透明にする
            var backStyle = new GUIStyle(style);
            backStyle.normal.textColor = Color.clear;
            backStyle.hover.textColor = Color.clear;
            backStyle.active.textColor = Color.clear;
            backStyle.focused.textColor = Color.clear;

            // 背景を色付きにする
            GUI.backgroundColor = backgroundColor;

            // 編集用のテキストエリアを描画
            var editedCode = EditorGUILayout.TextArea(code, backStyle, options);

            // シンタックスハイライトさせたコードを更新
            if (string.IsNullOrEmpty(cachedHighlightedCode) || (editedCode != cachedCode) || (editedCode != code))
            {
                cachedHighlightedCode = highlighter(editedCode);
                cachedCode = editedCode;
            }

            // 背景を透明にする
            GUI.backgroundColor = Color.clear;

            // 文字（シンタックスハイライトされない部分）を指定色にする
            var foreStyle = new GUIStyle(style);
            foreStyle.normal.textColor = textColor;
            foreStyle.hover.textColor = textColor;
            foreStyle.active.textColor = textColor;
            foreStyle.focused.textColor = textColor;

            // リッチテキストを ON にする
            foreStyle.richText = true;

            // シンタックスハイライト用のテキストエリアを表示
            EditorGUI.TextArea(GUILayoutUtility.GetLastRect(), cachedHighlightedCode, foreStyle);

            // 色を元に戻す
            GUI.backgroundColor = preBackgroundColor;
            GUI.color = preColor;

            return editedCode;
        }
    }
}
