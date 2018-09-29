using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TemplateEditor
{

    public static class TemplateUtility
    {
        public enum OverwriteType
        {
            Not,
            Replace,
            AddStart,
            AddEnd,
        }

        public static bool AddNonDuplicationProperty<T>(SerializedProperty arrayProperty, ICollection<string> paths) where T : UnityEngine.Object
        {
            bool isAdd = false;
            foreach (var path in paths)
            {
                var templateSetting = AssetDatabase.LoadAssetAtPath<T>(path);
                if (templateSetting == null)
                {
                    continue;
                }

                bool isDuplication = false;
                for (int i = 0; i < arrayProperty.arraySize; ++i)
                {
                    if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceInstanceIDValue == templateSetting.GetInstanceID())
                    {
                        isDuplication = true;
                        break;
                    }
                }

                if (isDuplication == true)
                {
                    continue;
                }

                var index = arrayProperty.arraySize;
                arrayProperty.InsertArrayElementAtIndex(index);
                arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue = templateSetting;

                isAdd = true;
            }

            return isAdd;
        }

        public static IProcessChain ConvertProcessChianInstanceFromObject(object obj)
        {
            IProcessChain processChain = null;

            // MonoScript pattern
            var monoScript = obj as MonoScript;
            if (monoScript != null)
            {
                var classType = monoScript.GetClass();
                if (classType == null)
                {
                    Debug.LogErrorFormat("Class not found : {0} ", monoScript.name);
                    return null;
                }

                if (typeof(IProcessChain).IsAssignableFrom(classType) == false)
                {
                    Debug.LogErrorFormat("Not inherited 'IProcessChain' : {0} ", classType.Name);
                    return null;
                }

                processChain = Activator.CreateInstance(classType) as IProcessChain;
            }

            // ScriptableObject pattern
            var scriptableObject = obj as ScriptableObject;
            if (scriptableObject != null)
            {
                var classType = scriptableObject.GetType();
                if (typeof(IProcessChain).IsAssignableFrom(classType) == false)
                {
                    Debug.LogErrorFormat("Not inherited 'IProcessChain' : {0} ", classType.Name);
                    return null;
                }

                processChain = scriptableObject as IProcessChain;
            }

            return processChain;
        }

        public static void ExecuteProcessChain(object obj, ProcessMetadata metadata, Dictionary<string, object> result)
        {
            var processChain = ConvertProcessChianInstanceFromObject(obj);
            if (processChain != null)
            {
                processChain.Process(metadata, result);
            }
        }

        public static string[] FindAssetGuids(Type type)
        {
            return AssetDatabase.FindAssets("t:" + type.Name);
        }

        public static IEnumerable<string> FindAssetPaths(Type type)
        {
            var guids = FindAssetGuids(type);

            foreach (var guid in guids)
            {
                yield return AssetDatabase.GUIDToAssetPath(guid);
            }
        }

        public static void OpenEditorWindow(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            var editor = Editor.CreateEditor(asset);

            EditorPreviewWindow.Open(editor);
        }

        public static void ExecuteSetting(string guid, Dictionary<string, object> result = null)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<TemplateSetting>(path);
            var editor = Editor.CreateEditor(asset) as TemplateSettingEditor;
            editor.Create(result);
        }

        public static string GetDirectoryPath(string path)
        {
            return Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        }

        #region Config Value

        public static void SetConfigValue(string name, string value)
        {
            EditorUserSettings.SetConfigValue(name, value);
        }

        public static string GetConfigValue(string key)
        {
            return EditorUserSettings.GetConfigValue(key);
        }

        #endregion

        #region Create Script

        public static bool CreateScript(string path, string text, OverwriteType overwrite)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directoryName) == false && Directory.Exists(directoryName) == false)
            {
                Directory.CreateDirectory(directoryName);
            }

            if (File.Exists(path) == false)
            {
                File.WriteAllText(path, text, Encoding.UTF8);
                return true;
            }

            // TODO : 最適化？
            switch (overwrite)
            {
                case OverwriteType.Not:
                    return false;

                case OverwriteType.AddStart:
                    text = text + File.ReadAllText(path, Encoding.UTF8);
                    break;

                case OverwriteType.AddEnd:
                    text = File.ReadAllText(path, Encoding.UTF8) + text;
                    break;
            }

            File.WriteAllText(path, text, Encoding.UTF8);

            return true;
        }

        public static void RefreshEditor()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        }

        public static string GetActiveFolder()
        {
            if (Selection.activeObject == null)
            {
                return "Assets";
            }

            var activeObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(activeObjectPath))
            {
                return activeObjectPath;
            }

            return Directory.GetParent(activeObjectPath).ToString();
        }

        #endregion
    }
}
