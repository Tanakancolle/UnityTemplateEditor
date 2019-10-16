#pragma warning disable CS0649

using System.IO;
using TemplateEditor;
using UnityEditor;
using UnityEngine;

namespace TemplateEditor
{
    public class UserSettingCopyWindow : EditorWindow
    {
        private static readonly string DefaultTemplateDirectory = "/TemplateTool/";

        public static void Open()
        {
                EditorWindow.GetWindow<UserSettingCopyWindow>(true);
        }

        private UserSetting _userSetting;
        private string _namespace;
        private string _createFolder = "Assets/CustomTemplateSetting";

        private void OnEnable()
        {
            _userSetting = ToolExecutor.GetUseUserSetting();
        }

        private void OnGUI()
        {
            _createFolder = EditorGUILayout.TextField("Create Folder", _createFolder);
            _namespace = EditorGUILayout.TextField("Add Namespace", _namespace);

            if (GUILayout.Button("Create Custom Template"))
            {
                Copy(_userSetting, _createFolder, _namespace);
            }
        }

        private static void Copy(UserSetting target, string copyDirectory, string namespaceName)
        {
            if (copyDirectory.Contains("/Editor/") == false)
            {
                copyDirectory = Path.Combine(copyDirectory, "Editor");
            }

            Directory.CreateDirectory(copyDirectory);

            var targetPath = AssetDatabase.GetAssetPath(target);
            var copyPath = Path.Combine(copyDirectory, Path.GetFileName(targetPath).Replace("Default", "Custom"));

            if (ToolExecutor.CheckCustomSetting())
            {
                Debug.LogWarning("Already custom setting exists");
                return;
            }

            AssetDatabase.CopyAsset(targetPath, copyPath);

            var copyAsset = AssetDatabase.LoadAssetAtPath<UserSetting>(copyPath);
            CopyChild(copyAsset, copyDirectory, namespaceName);
        }

        private static void CopyChild(Object target, string copyDirectory, string namespaceName)
        {
            var isNameSpaceInsert = target is TemplateSetting && string.IsNullOrEmpty(namespaceName) == false;

            var copyAssetSerialized = new SerializedObject(target);
            foreach (var property in ScriptableObjectUtility.GetSerializedProperties(copyAssetSerialized, true, true))
            {
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                {
                    if (property.objectReferenceValue is MonoScript)
                    {
                        continue;
                    }

                    var childTargetPath = AssetDatabase.GetAssetPath(property.objectReferenceValue);
                    var childTargetDirectory = Path.GetDirectoryName(childTargetPath);
                    var childCopyPath = Path.Combine(
                        copyDirectory,
                        childTargetDirectory.Substring(childTargetDirectory.IndexOf(DefaultTemplateDirectory) + DefaultTemplateDirectory.Length),
                        Path.GetFileName(childTargetPath));

                    Directory.CreateDirectory(Path.GetDirectoryName(childCopyPath));
                    AssetDatabase.CopyAsset(childTargetPath, childCopyPath);
                    var copyChildAsset = AssetDatabase.LoadAssetAtPath(childCopyPath, property.objectReferenceValue.GetType());
                    property.objectReferenceValue = copyChildAsset;
                    CopyChild(copyChildAsset, copyDirectory, namespaceName);
                }
                else if (isNameSpaceInsert && property.propertyType == SerializedPropertyType.String && property.name == "Code")
                {
                    property.stringValue = TemplateUtility.InsertNamespace(property.stringValue, namespaceName);
                }
            }

            copyAssetSerialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}