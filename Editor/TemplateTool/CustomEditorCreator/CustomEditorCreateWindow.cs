using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TemplateEditor
{
    public class CustomEditorCreateWindow : EditorWindow
    {
        private static readonly string CustomEditorCreateGroupSettingGuid = "39929be43dcc041049feecc87d7f23d3";

        public static void Open()
        {
            GetWindow<CustomEditorCreateWindow>(true);
        }

        private ScriptableObject _targetScriptableObject;

        private void OnGUI()
        {
            _targetScriptableObject = EditorGUILayout.ObjectField(_targetScriptableObject, typeof(ScriptableObject), false) as ScriptableObject;

            if (_targetScriptableObject == null)
            {
                return;
            }

            if (GUILayout.Button("生成") == false)
            {
                return;
            }

            var targetUseSerializeNames = ScriptableObjectUtility.GetSerializeNamesWithoutDefault(_targetScriptableObject);

            var type = _targetScriptableObject.GetType();
            var targetName = type.Name;
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_targetScriptableObject));

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

            TemplateUtility.ExecuteGroupSetting(CustomEditorCreateGroupSettingGuid, result);
        }
    }
}