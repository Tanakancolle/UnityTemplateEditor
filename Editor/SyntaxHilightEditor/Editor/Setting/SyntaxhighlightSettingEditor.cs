using UnityEditor;
using UnityEngine;
using System;

namespace SyntaxHighlightEditor
{
    [CustomEditor(typeof(SyntaxHighlightSetting))]
    public class SyntaxhighlightSettingEditor : Editor
    {
        /// <summary>
        /// Property.
        /// SyntaxhighlightSetting のメンバ名と同じにする
        /// </summary>
        public enum Property
        {
            Format,
            FormatTypeText,
            Color,
            Patterns
        }

        private enum Format
        {
            Custom = -1,
            Normal,
            Delimit,
        }

        private static readonly string[] Formats =
        {
            "(?<{0}>({1}))",
            "(?<{0}>(?<![0-9a-zA-Z_])({1})(?![0-9a-zA-Z_]))"
        };

        private Color _highlightColor = Color.white;
        private string _customFormat;
        private SerializedProperty[] _properties;
        private Format _formatType;

        private void OnEnable()
        {
            var setting = target as SyntaxHighlightSetting;

            var names = Enum.GetNames(typeof(Property));
            _properties = new SerializedProperty[names.Length];
            for (int i = 0; i < _properties.Length; ++i)
            {
                _properties[i] = serializedObject.FindProperty(names[i]);
            }

            ColorUtility.TryParseHtmlString("#" + setting.Color, out _highlightColor);
            _formatType = ConvertFormatTypeFromText(setting.FormatTypeText);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                DrawFormat();
                _highlightColor = EditorGUILayout.ColorField("Highlight Color", _highlightColor);
                GetProperty(Property.Color).stringValue = ColorUtility.ToHtmlStringRGBA(_highlightColor);
                EditorGUILayout.PropertyField(GetProperty(Property.Patterns), true);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private SerializedProperty GetProperty(Property property)
        {
            return _properties[(int) property];
        }

        private void DrawFormat()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                _formatType = (Format) EditorGUILayout.EnumPopup("Format Type", _formatType);
                GetProperty(Property.FormatTypeText).stringValue = ConvertTextFromFormatType(_formatType);
                switch (_formatType)
                {
                    case Format.Custom:
                    {
                        GetProperty(Property.Format).stringValue = _customFormat = EditorGUILayout.TextField(_customFormat);
                    }
                        break;

                    default:
                    {
                        var result = GetFormatText(_formatType);
                        GetProperty(Property.Format).stringValue = result;
                        EditorGUILayout.LabelField(result);
                    }
                        break;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private Format ConvertFormatTypeFromText(string text)
        {
            try
            {
                return (Format) Enum.Parse(typeof(Format), text);
            }
            catch
            {
                return Format.Normal;
            }
        }

        private string ConvertTextFromFormatType(Format format)
        {
            return Enum.GetName(typeof(Format), format);
        }

        private static string GetFormatText(Format format)
        {
            if (format == Format.Custom)
            {
                return string.Empty;
            }

            return Formats[(int) format];
        }
    }
}
