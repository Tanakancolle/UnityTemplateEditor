using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TemplateEditor
{
    public static class ScriptableObjectUtility
    {
        private static readonly string EmptyScriptableObjectGuid = "7b47cc4df51e64f73aa22432250a8be5";

        public static string[] GetSerializeNamesWithoutDefault(ScriptableObject target)
        {
            var defaultNames = GetDefaultSerializeNames();
            return GetSerializeNames(target).Where(n => defaultNames.Contains(n) == false).ToArray();
        }

        public static List<string> GetDefaultSerializeNames()
        {
            var empty = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(EmptyScriptableObjectGuid));
            return GetSerializeNames(empty);
        }

        public static List<string> GetSerializeNames(ScriptableObject target)
        {
            var list = new List<string>();
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.GetIterator();
            property.Next(true);
            list.Add(property.name);
            while (property.Next(false))
            {
                list.Add(property.name);
            }

            return list;
        }
    }
}
