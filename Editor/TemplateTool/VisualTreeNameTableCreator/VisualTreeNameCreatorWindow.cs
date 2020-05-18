using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TemplateEditor
{
    public class VisualTreeNameCreatorWindow : EditorWindow
    {
        private static readonly string VisualTreeNameGetProcessorGuid = "84611633f6726434f8d0065957b8d1ea";

        public static void Open()
        {
            GetWindow<VisualTreeNameCreatorWindow>(true);
        }

        private SerializedProperty _targetListProperty;
        private SerializedObject _serializedObject;
        private Vector2 _scrollPos;
        private UserSetting _userSetting;

        private void OnEnable()
        {
            var targetGetProcessor = AssetDatabase.LoadAssetAtPath<VisualTreeNameGetProcessor>(AssetDatabase.GUIDToAssetPath(VisualTreeNameGetProcessorGuid));
            _serializedObject = new SerializedObject(targetGetProcessor);
            _targetListProperty = _serializedObject.FindProperty("_targets");

            _userSetting = ToolExecutor.GetUseUserSetting();
        }

        private void OnGUI()
        {
            if (_targetListProperty == null)
            {
                return;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                _serializedObject.Update();
                EditorGUILayout.PropertyField(_targetListProperty, new GUIContent("Target VisualTreeAssets"), true);
                _serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Create"))
            {
                TemplateUtility.ExecuteSetting(_userSetting.GetSetting(UserSetting.SettingType.VisualTreeName));
            }
        }
    }
}