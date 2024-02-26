using System;
using UnityEngine;
using UnityEditor;

namespace TemplateEditor
{
    [CustomPropertyDrawer(typeof(EnumArrayAttribute))]
    public class EnumArrayAttributeDrawer : PropertyDrawer
    {
        private string[] _names;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                if (_names == null)
                {
                    _names = Enum.GetNames(((EnumArrayAttribute) attribute).TargetEnumType);
                }

                var pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                EditorGUI.PropertyField(position, property, new GUIContent(_names[pos]));
            }
            catch
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumArrayAttribute : PropertyAttribute
    {
        public readonly Type TargetEnumType;

        public EnumArrayAttribute(Type enumType)
        {
            TargetEnumType = enumType;
        }
    }
}