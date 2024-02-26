using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace TemplateEditor
{
    public class CustomEditorCreateWindow : EditorWindow
    {
        public static void Open()
        {
            GetWindow<CustomEditorCreateWindow>(true);
        }

        private ScriptableObject _targetScriptableObject;
        private MonoScript _targetScriptableClass;

        private void OnGUI()
        {
            _targetScriptableObject = EditorGUILayout.ObjectField(_targetScriptableObject, typeof(ScriptableObject), false) as ScriptableObject;
            EditorGUILayout.LabelField("  or");
            _targetScriptableClass = EditorGUILayout.ObjectField(_targetScriptableClass, typeof(MonoScript), false) as MonoScript;

            if (_targetScriptableObject == null && _targetScriptableClass == null)
            {
                return;
            }

            if (GUILayout.Button("生成") == false)
            {
                return;
            }

            var type = _targetScriptableClass.GetClass();
            if (_targetScriptableObject != null)
            {
                type = _targetScriptableObject.GetType();
            }
            
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath((Object)_targetScriptableObject ?? _targetScriptableClass));
            var targetScriptableObject = ScriptableObject.CreateInstance(type);

            var targetName = type.Name;
            var targetUseSerializeNames = ScriptableObjectUtility.GetSerializeNamesWithoutDefault(targetScriptableObject);

            var customEditorName = targetName + "Editor";
            var propertyGetterName = targetName + "PropertyGetter";
            var result = new ProcessDictionary();
            result.Add("TargetNamespace", "using " + type.Namespace + ";");
            result.Add("CustomEditorCreatePath", TemplateUtility.GetFilePathFromFileName(customEditorName + ".cs") ?? path);
            result.Add("PropertyGetterCreatePath", TemplateUtility.GetFilePathFromFileName(propertyGetterName + ".cs") ?? path);
            result.Add("TargetScriptableObjectName", targetName);
            result.Add("CustomEditorName", customEditorName);
            result.Add("PropertyGetterName", propertyGetterName);
            result.Add("TypeNames", targetUseSerializeNames.Select(StringBuilderExtension.ConvertEnumName).ToArray());
            result.Add("PropertyNames", targetUseSerializeNames);

            var setting = ToolExecutor.GetUseUserSetting().GetSetting(UserSetting.GroupSettingType.CustomEditorCreator);
            TemplateUtility.ExecuteGroupSetting(setting, result);

            DestroyImmediate(targetScriptableObject);
        }
    }
}