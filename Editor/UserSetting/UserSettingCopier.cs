using System.IO;
using TemplateEditor;
using UnityEditor;
using UnityEngine;

public class UserSettingCopier
{
    private static readonly string DefaultTemplateDirectory = "/TemplateTool/";

    [MenuItem("Test/Test2")]
    public static void Test()
    {
        var setting = ToolExecutor.GetUseUserSetting();
        Copy(setting, "Assets/TestFolder");
    }

    private static void Copy(UserSettingBase target, string copyDirectory)
    {
        Directory.CreateDirectory(copyDirectory);

        var targetPath = AssetDatabase.GetAssetPath(target);
        var copyPath = Path.Combine(copyDirectory, Path.GetFileName(targetPath));
        AssetDatabase.CopyAsset(targetPath, copyPath);

        var copyAsset = AssetDatabase.LoadAssetAtPath<UserSettingBase>(copyPath);
        var copyAssetSerialized = new SerializedObject(copyAsset);
        foreach (var property in ScriptableObjectUtility.GetSerializedProperties(copyAssetSerialized, true, true))
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                if (property.name == "m_Script")
                {
                    continue;
                }

                var childTargetPath = AssetDatabase.GetAssetPath(property.objectReferenceValue);
                var childTargetDirectory = Path.GetDirectoryName(childTargetPath);
                var childCopyPath = Path.Combine(
                    copyDirectory,
                    "UsedAssets",
                    childTargetDirectory.Substring(childTargetDirectory.IndexOf(DefaultTemplateDirectory) + DefaultTemplateDirectory.Length),
                    Path.GetFileName(childTargetPath));
                Directory.CreateDirectory(Path.GetDirectoryName(childCopyPath));
                AssetDatabase.CopyAsset(childTargetPath, childCopyPath);
                property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(childCopyPath, property.objectReferenceValue.GetType());
            }
        }

        copyAssetSerialized.ApplyModifiedProperties();
    }
}