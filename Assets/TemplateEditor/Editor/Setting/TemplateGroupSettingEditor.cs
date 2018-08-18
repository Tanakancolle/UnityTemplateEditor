using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TemplateEditor
{
    [CustomEditor(typeof(TemplateGroupSetting))]
    public class TemplateGroupSettingEditor : Editor
    {
        private static readonly TemplateSettingStatus.Property[] ReplaceProperty = new TemplateSettingStatus.Property[]
        {
            TemplateSettingStatus.Property.Path,
            TemplateSettingStatus.Property.ScriptName,
            TemplateSettingStatus.Property.Code,
        };

        private TemplateGroupSetting _groupSetting;
        private SerializedProperty _settingsProperty;
        private SerializedProperty _isAssetsMenuItemProperty;
        private SerializedProperty _descriptionProperty;
        private FoldoutInfo _descriptionFoldout;
        private List<TemplateSettingStatus> _statusList = new List<TemplateSettingStatus>();
        private List<FoldoutInfo> _foldoutList = new List<FoldoutInfo>();
        private List<ReplaceInfo> _replaceList = new List<ReplaceInfo>();
        private bool _isUpdateSetting;

        void OnEnable()
        {
            _groupSetting = target as TemplateGroupSetting;
            _settingsProperty = serializedObject.FindProperty("Settings");
            _isAssetsMenuItemProperty = serializedObject.FindProperty("AssetsMenuItem");
            _descriptionProperty = serializedObject.FindProperty("Description");
            _descriptionFoldout = new FoldoutInfo("Description", DrawDescription);
            BuildSettingList();
        }

        public override void OnInspectorGUI()
        {
            var isChanged = false;
            serializedObject.Update();
            {
                isChanged = DrawSettingList();

                EditorGUIHelper.DrawFoldouts(_foldoutList);
                TemplateSettingEditor.DrawReplace(_replaceList, _groupSetting.GetInstanceID().ToString());
                DrawIsAssetsMenuItem();
                DrawCreate();
                EditorGUIHelper.DrawFoldout(_descriptionFoldout);

                UpdateReplaceList();
            }
            serializedObject.ApplyModifiedProperties();

            if (isChanged)
            {
                BuildSettingList();
            }
        }

        private bool DrawSettingList()
        {
            bool isChange = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_settingsProperty, new GUIContent("Template Settings"), true);
            isChange = EditorGUI.EndChangeCheck();

            var paths = EditorGUIHelper.DrawDragAndDropArea("Drag & Drop (重複なし追加)");
            if (paths != null)
            {
                if (TemplateUtility.AddNonDuplicationProperty<TemplateSetting>(_settingsProperty, paths) == true)
                {
                    isChange = true;
                }
            }

            return isChange;
        }

        private void BuildSettingList()
        {
            _foldoutList.Clear();
            _statusList.Clear();

            if (_groupSetting.Settings != null)
            {
                foreach (var setting in _groupSetting.Settings)
                {
                    if (setting == null)
                    {
                        continue;
                    }

                    var status = new TemplateSettingStatus(new SerializedObject(setting));
                    var foldout = new FoldoutInfo(setting.name, () =>
                        {
                            status.TargetSerializedObject.Update();
                            DrawSetting(status);
                            status.TargetSerializedObject.ApplyModifiedProperties();
                        }
                    );

                    foldout.IsFoldout = false;
                    _foldoutList.Add(foldout);
                    _statusList.Add(status);
                }
            }

            UpdateReplaceList(true);
        }

        private void DrawSetting(TemplateSettingStatus status)
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                TemplateSettingEditor.DrawHeader(status);
                TemplateSettingEditor.DrawCode(status);
                TemplateSettingEditor.DrawChain(status);
                TemplateSettingEditor.DrawOverwrite(status);
                TemplateSettingEditor.DrawPrefab(status);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawIsAssetsMenuItem()
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var cache = _groupSetting.IsAssetsMenuItem;
                EditorGUILayout.PropertyField(_isAssetsMenuItemProperty, new GUIContent("Add Assets Menu"));

                // 生成時に設定反映が間に合わないため
                _groupSetting.IsAssetsMenuItem = _isAssetsMenuItemProperty.boolValue;

                if (cache != _groupSetting.IsAssetsMenuItem)
                {
                    AssetsMenuItemProcessor.Create();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCreate()
        {
            EditorGUILayout.BeginHorizontal(EditorGUIHelper.GetScopeStyle());
            {
                if (GUILayout.Button("Create"))
                {
                    CreateScript(true);
                    return;
                }

                if (GUILayout.Button("No Refresh Create"))
                {

                    CreateScript(false);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateScript(bool isRefresh)
        {
            foreach (var status in _statusList)
            {
                TemplateSettingEditor.CreateScript(status, _replaceList, null, isRefresh);
            }
        }

        private void DrawDescription()
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                _descriptionProperty.stringValue = EditorGUILayout.TextArea(_descriptionProperty.stringValue);
            }
            EditorGUILayout.EndVertical();
        }

        private void UpdateReplaceList(bool isForce = false)
        {
            if (isForce == false && _statusList.Any(status => status.IsUpdateText) == false)
            {
                // 更新なし
                return;
            }

            var texts = new string[_statusList.Count * ReplaceProperty.Length];
            for (int i = 0; i < _statusList.Count; ++i)
            {
                _statusList[i].IsUpdateText = false;

                var index = i * ReplaceProperty.Length;
                for (int j = 0; j < ReplaceProperty.Length; ++j)
                {
                    texts[index + j] = _statusList[i].GetProperty(ReplaceProperty[j]).stringValue;
                }
            }

            var words = ReplaceProcessor.GetReplaceWords(texts);
            foreach (var status in _statusList)
            {
                TemplateSettingEditor.RemoveChainWords(words, status.TargetTemplateSetting.Chain);
            }

            _replaceList = TemplateSettingEditor.CreateReplaceList(_replaceList, words.ToArray());

            TemplateSettingEditor.SetReplaceListFromConfigValue(_replaceList, _groupSetting.GetInstanceID().ToString());
        }
    }
}
