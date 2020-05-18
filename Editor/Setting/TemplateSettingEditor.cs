#pragma warning disable CS0618

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using SyntaxHighlightEditor;
using UnityEditorInternal;

namespace TemplateEditor
{
    public class TemplateSettingStatus
    {
        public readonly SerializedObject TargetSerializedObject;
        public readonly TemplateSetting TargetTemplateSetting;
        public readonly ReorderableList ChainReorderableList;
        public bool IsUpdateText;

        private readonly TemplateSettingPropertyGetter _propertyGetter;

        public TemplateSettingStatus(SerializedObject targetSerializedObject)
        {
            TargetSerializedObject = targetSerializedObject;
            TargetTemplateSetting = targetSerializedObject.targetObject as TemplateSetting;
            _propertyGetter = new TemplateSettingPropertyGetter(targetSerializedObject);

            ChainReorderableList = new ReorderableList(targetSerializedObject, GetProperty(TemplateSettingPropertyGetter.Property.Chain))
            {
                drawElementCallback = DrawChainListElement,
                drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "List"); },
            };
        }

        private void DrawChainListElement(Rect rect, int index, bool isActive, bool isFocuse)
        {
            var element = GetProperty(TemplateSettingPropertyGetter.Property.Chain).GetArrayElementAtIndex (index);
            rect.height -= 4f;
            rect.y += 2f;
            rect.xMin += 20f;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        public SerializedProperty GetProperty(TemplateSettingPropertyGetter.Property type)
        {
            return _propertyGetter.GetProperty(type);
        }
    }

    public class ReplaceInfo
    {
        public string Key;
        public string ReplaceWord;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TemplateSetting))]
    public class TemplateSettingEditor : Editor
    {
        public TemplateSettingStatus SettingStatus;
        private FoldoutInfo[] _foldouts;
        private FoldoutInfo _descriptionFoldout;
        private List<ReplaceInfo> _replaceList = new List<ReplaceInfo>();
        private string _instanceId;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _instanceId = target.GetInstanceID().ToString();
            SettingStatus = new TemplateSettingStatus(serializedObject);

            _foldouts = new FoldoutInfo[]
            {
                new FoldoutInfo("Code", () => DrawCode(SettingStatus)),
                new FoldoutInfo("Replace Texts", () => DrawReplace(_replaceList, _instanceId)),
                new FoldoutInfo("Pre Process", () => DrawChain(SettingStatus)),
            };

            var property = SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.IsFoldouts);
            for (int i = 0; i < _foldouts.Length; ++i)
            {
                if (property.arraySize <= i)
                {
                    break;
                }

                _foldouts[i].IsFoldout = property.GetArrayElementAtIndex(i).boolValue;
            }

            _descriptionFoldout = new FoldoutInfo("Description", DrawDescription);
            _descriptionFoldout.IsFoldout = string.IsNullOrEmpty(SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.Description).stringValue) == false;

            UpdateReplaceList(true);
        }

        public override void OnInspectorGUI()
        {
            SettingStatus.TargetSerializedObject.Update();
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    DrawHeader(SettingStatus);
                    EditorGUIHelper.DrawFoldouts(_foldouts);
                    DrawOverwrite(SettingStatus);
                    DrawIsAssetsMenuItem();
                    DrawPrefab(SettingStatus);
                    EditorGUIHelper.DrawFoldout(_descriptionFoldout);
                }
                EditorGUILayout.EndScrollView();

                DrawCreate();
                UpdateFoldout();
                UpdateReplaceList();
            }
            SettingStatus.TargetSerializedObject.ApplyModifiedProperties();
        }

        public static void DrawHeader(TemplateSettingStatus status)
        {
            EditorGUI.BeginChangeCheck();
            {
                // setting create path
                EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
                {
                    var property = status.GetProperty(TemplateSettingPropertyGetter.Property.Path);
                    EditorGUILayout.PropertyField(property, new GUIContent("Create Path"));

                    var paths = EditorGUIHelper.DrawDragAndDropArea();
                    if (paths != null && paths.Length > 0)
                    {
                        // Index 0 のパスを使用する
                        property.stringValue = TemplateUtility.GetDirectoryPath(paths[0]);
                    }

                    var createPath = status.TargetTemplateSetting.Path;
                    if (string.IsNullOrEmpty(createPath))
                    {
                        EditorGUILayout.HelpBox("If empty, the script will be created in active folder", MessageType.Info);

                    }
                    EditorGUILayout.HelpBox("Example: Assets/Folder", MessageType.Info);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
                {
                    EditorGUILayout.PropertyField(status.GetProperty(TemplateSettingPropertyGetter.Property.ScriptName), new GUIContent("Script Name"));
                    if (string.IsNullOrEmpty(status.TargetTemplateSetting.ScriptName))
                    {
                        EditorGUILayout.HelpBox("Example: Example.cs", MessageType.Info);
                    }
                    else if (Regex.IsMatch(status.TargetTemplateSetting.ScriptName, @"\..+$", RegexOptions.Compiled) == false)
                    {
                        EditorGUILayout.HelpBox("Extension required", MessageType.Warning);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                status.IsUpdateText = true;
                Undo.IncrementCurrentGroup();
            }
        }

        public static void SetReplaceListFromConfigValue(List<ReplaceInfo> replaces, string prefix)
        {
            foreach (var replace in replaces)
            {
                replace.ReplaceWord = TemplateUtility.GetConfigValue(prefix + replace.Key);
            }
        }

        public static void DrawChain(TemplateSettingStatus status)
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                status.ChainReorderableList.DoLayoutList();

                var selectIndex = status.ChainReorderableList.index;
                if (selectIndex >= 0)
                {
                    var select = status.ChainReorderableList.serializedProperty.GetArrayElementAtIndex(selectIndex);
                    var chain = TemplateUtility.ConvertProcessChianInstanceFromObject(select.objectReferenceValue);
                    if (chain != null)
                    {
                        var builder = new StringBuilder();
                        builder.AppendLine("[Used Variables]");
                        foreach (var word in chain.GetReplaceWords())
                        {
                            builder.AppendLine(ReplaceProcessor.GetReplaceText(word));
                        }

                        // TODO : Cache
                        var style = new GUIStyle(GUI.skin.label)
                        {
                            wordWrap = true,
                        };
                        var label = builder.ToString();
                        var content = new GUIContent(label);
                        var rect = GUILayoutUtility.GetRect(content, style);
                        EditorGUI.SelectableLabel(rect, label, style);
                        EditorGUILayout.LabelField("[Description]\n" + chain.GetDescription(), style);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("When you select item, description will be displayed", MessageType.Info, true);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawOverwrite(TemplateSettingStatus status)
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var property = status.GetProperty(TemplateSettingPropertyGetter.Property.Overwrite);
                EditorGUILayout.PropertyField(property, new GUIContent("Overwrite Type"));
            }
            EditorGUILayout.EndVertical();
        }

        public static void CreateScript(TemplateSettingStatus status, List<ReplaceInfo> replaces, ProcessDictionary result = null, bool isRefresh = true)
        {
            if (result == null)
            {
                result = new ProcessDictionary();
            }

            if (replaces != null)
            {
                foreach (var replace in replaces)
                {
                    if (string.IsNullOrEmpty(replace.ReplaceWord))
                    {
                        continue;
                    }

                    result[replace.Key] = replace.ReplaceWord;
                }
            }

            ExecuteChain(status, result);

            // 生成ディレクトリが指定されていなければアクティブなパスへ作成
            var createDirectory = status.GetProperty(TemplateSettingPropertyGetter.Property.Path).stringValue;
            var createPath = Path.Combine(
                string.IsNullOrEmpty(createDirectory) == false ? createDirectory : TemplateUtility.GetActiveFolder(),
                status.GetProperty(TemplateSettingPropertyGetter.Property.ScriptName).stringValue
            );
            var code = status.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue;

            var path = Replace(createPath, result);
            TemplateUtility.CreateScript(
                path,
                Replace(code, result),
                (TemplateUtility.OverwriteType) status.GetProperty(TemplateSettingPropertyGetter.Property.Overwrite).enumValueIndex
            );

            if (isRefresh)
            {
                AssetDatabase.ImportAsset(path);
                TemplateUtility.RefreshEditor();
            }

            // プレハブ生成登録
            var prefabObject = status.GetProperty(TemplateSettingStatus.Property.DuplicatePrefab).objectReferenceValue as GameObject;
            if (prefabObject != null)
            {
                TemplatePrefabCreator.AddTempCreatePrefabSetting(status.TargetTemplateSetting, path);
            }
        }

        public static string Replace(string text, Dictionary<string, object> result)
        {
            return ReplaceProcessor.ReplaceProcess(
                text,
                result
            );
        }

        public static void ExecuteChain(TemplateSettingStatus status, ProcessDictionary result)
        {
            var metadata = new ProcessMetadata(status.TargetTemplateSetting);
            var property = status.GetProperty(TemplateSettingPropertyGetter.Property.Chain);
            for (int i = 0; i < property.arraySize; ++i)
            {
                TemplateUtility.ExecuteProcessChain(property.GetArrayElementAtIndex(i).objectReferenceValue, metadata, result);
            }
        }

        public static void DrawReplace(List<ReplaceInfo> replaces, string savePrefix)
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            if (replaces.Count == 0)
            {
                EditorGUILayout.HelpBox("{<Foo>} in 'Create Path' and 'Script Name' and 'Code' will be replace\nNote: Variables used in 'Pre Process' are not listed here.", MessageType.Info);
            }
            else
            {
                foreach (var replace in replaces)
                {
                    var str = EditorGUILayout.TextField(ReplaceProcessor.GetReplaceText(replace.Key), replace.ReplaceWord);
                    if (str == replace.ReplaceWord)
                    {
                        continue;
                    }

                    // 置き換え文字をUnityへキャッシュ
                    replace.ReplaceWord = str;
                    TemplateUtility.SetConfigValue(savePrefix + replace.Key, replace.ReplaceWord);
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawCode(TemplateSettingStatus status)
        {
            var height = status.GetProperty(TemplateSettingPropertyGetter.Property.CodeAreaHeight);

            EditorGUILayout.BeginHorizontal(EditorGUIHelper.GetScopeStyle());
            {
                EditorGUILayout.PropertyField(height, new GUIContent("Min Height"));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var code = status.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue;
                var scrollPos = status.GetProperty(TemplateSettingPropertyGetter.Property.ScrollPos);
                var scroll = scrollPos.vector2Value;
                var editedCode = SyntaxHighlightUtility.DrawCSharpCode(ref scroll, code, 12, height.floatValue, height.floatValue);
                scrollPos.vector2Value = scroll;
                if (editedCode != code)
                {
                    status.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue = editedCode;
                    status.IsUpdateText = true;
                    Undo.IncrementCurrentGroup();
                }
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawPrefab(TemplateSettingStatus status)
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var prefabProperty = status.GetProperty(TemplateSettingPropertyGetter.Property.DuplicatePrefab);
                var targetProperty = status.GetProperty(TemplateSettingPropertyGetter.Property.AttachTarget);

                var oldObj = prefabProperty.objectReferenceValue as GameObject;
                EditorGUILayout.PropertyField(prefabProperty, new GUIContent("Attach Prefab"), true);

                var obj = prefabProperty.objectReferenceValue as GameObject;
                if (obj == null || PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab)
                {
                    targetProperty.objectReferenceValue = null;

                    EditorGUILayout.EndVertical();
                    return;
                }

                if (oldObj != obj)
                {
                    prefabProperty.objectReferenceValue = PrefabUtility.FindRootGameObjectWithSameParentPrefab(obj);
                    targetProperty.objectReferenceValue = null;
                }

                EditorGUILayout.BeginHorizontal(EditorGUIHelper.GetScopeStyle());
                {
                    if (GUILayout.Button("Change Attach Target"))
                    {
                        PrefabTreeViewWindow.Open(obj, targetProperty.objectReferenceValue as GameObject, (targetObj) =>
                        {
                            status.TargetSerializedObject.Update();
                            status.GetProperty(TemplateSettingPropertyGetter.Property.AttachTarget).objectReferenceValue = targetObj;
                            status.TargetSerializedObject.ApplyModifiedProperties();
                        });
                    }

                    EditorGUILayout.LabelField(targetProperty.objectReferenceValue == null ? string.Empty : targetProperty.objectReferenceValue.name);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
                {
                    var pathProperty = status.GetProperty(TemplateSettingPropertyGetter.Property.PrefabPath);
                    EditorGUILayout.PropertyField(pathProperty, new GUIContent("Create Prefab Path"), true);

                    var paths = EditorGUIHelper.DrawDragAndDropArea();
                    if (paths != null && paths.Length > 0)
                    {
                        pathProperty.stringValue = paths[0];
                    }

                    if (string.IsNullOrEmpty(pathProperty.stringValue))
                    {
                        EditorGUILayout.HelpBox("If empty, the script will be created in active folder", MessageType.Info);
                    }

                    EditorGUILayout.HelpBox("Example: Assets/Folder", MessageType.Info);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
                {
                    var nameProperty = status.GetProperty(TemplateSettingPropertyGetter.Property.PrefabName);
                    EditorGUILayout.PropertyField(nameProperty, new GUIContent("Prefab Name"), true);

                    if (string.IsNullOrEmpty(nameProperty.stringValue))
                    {
                        EditorGUILayout.HelpBox("Example: ExamplePrefab", MessageType.Info);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawIsAssetsMenuItem()
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var cache = SettingStatus.TargetTemplateSetting.IsAssetsMenuItem;
                var isAssetMenuProperty = SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.AssetsMenuItem);
                EditorGUILayout.PropertyField(isAssetMenuProperty, new GUIContent("Add Asset Menu"));

                // 生成時に設定反映が間に合わないため
                SettingStatus.TargetTemplateSetting.IsAssetsMenuItem = isAssetMenuProperty.boolValue;

                if (cache != SettingStatus.TargetTemplateSetting.IsAssetsMenuItem)
                {
                    AssetsMenuItemProcessor.Execute();
                }

                EditorGUILayout.HelpBox("Add a menu item to \"Assets/Create/Template/~\"", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCreate()
        {
            EditorGUILayout.BeginHorizontal(EditorGUIHelper.GetScopeStyle());
            {
                if (GUILayout.Button("Create"))
                {
                    Create(null, true);
                    return;
                }

                if (GUILayout.Button("No Refresh Create"))
                {

                    Create(null, false);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void Create(ProcessDictionary result = null, bool isRefresh = true)
        {
            CreateScript(SettingStatus, _replaceList, result, isRefresh);
        }

        private void DrawDescription()
        {
            EditorGUILayout.BeginVertical(EditorGUIHelper.GetScopeStyle());
            {
                var property = SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.Description);
                // TODO : Cache
                var style = new GUIStyle(GUI.skin.textArea)
                {
                    wordWrap = true,
                };
                property.stringValue = EditorGUILayout.TextArea(property.stringValue, style);
            }
            EditorGUILayout.EndVertical();
        }

        private void UpdateFoldout()
        {
            var property = SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.IsFoldouts);
            for (int i = 0; i < _foldouts.Length; ++i)
            {
                if (property.arraySize <= i)
                {
                    property.InsertArrayElementAtIndex(i);
                }

                var isFoldoutProperty = property.GetArrayElementAtIndex(i);
                isFoldoutProperty.boolValue = _foldouts[i].IsFoldout;
            }
        }

        private void UpdateReplaceList(bool isForce = false)
        {
            if (isForce == false && SettingStatus.IsUpdateText == false)
            {
                // 更新なし
                return;
            }

            SettingStatus.IsUpdateText = false;
            var words = ReplaceProcessor.GetReplaceWords(
                SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.Path).stringValue,
                SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.ScriptName).stringValue,
                SettingStatus.GetProperty(TemplateSettingPropertyGetter.Property.Code).stringValue
            );

            RemoveChainWords(words, SettingStatus.TargetTemplateSetting.Chain);
            _replaceList = CreateReplaceList(_replaceList, words.ToArray());
            SetReplaceListFromConfigValue(_replaceList, _instanceId);
        }

        public static void RemoveChainWords(ICollection<string> words, object[] objects)
        {
            if (objects == null)
            {
                return;
            }

            var chains = new List<IProcessChain>();
            foreach (var obj in objects)
            {
                var chain = TemplateUtility.ConvertProcessChianInstanceFromObject(obj);
                if (chain != null)
                {
                    chains.Add(chain);
                }
            }

            foreach (var chain in chains)
            {
                foreach (var word in chain.GetReplaceWords())
                {
                    words.Remove(word);

                    // ToArray = 遅延実行だとエラーになるため
                    var regex = new Regex(string.Format(ProcessDictionary.ConvertWordPattern, word, @"\d+"), RegexOptions.IgnoreCase);
                    foreach (var matchWord in words.Where(w => regex.IsMatch(w)).ToArray())
                    {
                        words.Remove(matchWord);
                    }
                }
            }
        }

        public static List<ReplaceInfo> CreateReplaceList(List<ReplaceInfo> oldReplaces, string[] words)
        {
            var replaces = new List<ReplaceInfo>();
            foreach (var word in words)
            {
                var replace = oldReplaces.FirstOrDefault(info => info.Key == word) ??
                              new ReplaceInfo()
                              {
                                  Key = word
                              };

                replaces.Add(replace);
            }

            return replaces;
        }
    }
}
