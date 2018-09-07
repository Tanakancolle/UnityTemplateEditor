/*
 * The MIT License (MIT)
 * Copyright (c) 2016 hecomi
 * https://github.com/hecomi/uRaymarching#license
 */

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

        // ハイライト用の関数
        public System.Func<string, string> Highlighter { get; set; }

        // 表示高速化の為に変更があった時だけコードを更新
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
            GUI.backgroundColor = BackgroundColor;

            // 編集用のテキストエリアを描画
            var editedCode = EditorGUILayout.TextArea(code, backStyle, options);

            // シンタックスハイライトさせたコードを更新
            if (string.IsNullOrEmpty(CachedHighlightedCode) || (editedCode != _cachedCode) || (editedCode != code))
            {
                CachedHighlightedCode = Highlighter(editedCode);
                _cachedCode = editedCode;
            }

            // 背景を透明にする
            GUI.backgroundColor = Color.clear;

            // 文字（シンタックスハイライトされない部分）を指定色にする
            var foreStyle = new GUIStyle(style);
            foreStyle.normal.textColor = TextColor;
            foreStyle.hover.textColor = TextColor;
            foreStyle.active.textColor = TextColor;
            foreStyle.focused.textColor = TextColor;

            // リッチテキストを ON にする
            foreStyle.richText = true;

            // シンタックスハイライト用のテキストエリアを表示
            EditorGUI.TextArea(GUILayoutUtility.GetLastRect(), CachedHighlightedCode, foreStyle);

            // 色を元に戻す
            GUI.backgroundColor = preBackgroundColor;
            GUI.color = preColor;

            return editedCode;
        }
    }
}
