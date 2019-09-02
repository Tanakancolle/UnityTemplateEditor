using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TemplateEditor
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumFlagsAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var totalIntValue = 0;
            var labelWidth = EditorGUIUtility.labelWidth;
            var buttonWidth = (position.width - labelWidth) / property.enumNames.Length;

            EditorGUI.LabelField(new Rect(position.x, position.y, labelWidth, position.height), label);

            var buttonPos = new Rect(position.x + labelWidth, position.y, buttonWidth, position.height);
            for (int i = 0; i < property.enumNames.Length; i++)
            {
                var intValue = 1 << i;
                if (GUI.Toggle(buttonPos, (property.intValue & intValue) > 0, property.enumNames[i], "Button"))
                {
                    totalIntValue += intValue;
                }

                buttonPos.x += buttonWidth;
            }

            property.intValue = totalIntValue;
        }
    }
#endif
}
