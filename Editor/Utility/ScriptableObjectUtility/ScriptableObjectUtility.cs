using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TemplateEditor
{
    public static class ScriptableObjectUtility
    {
        private static ScriptableObject _tempScriptableObject;

        public static string[] GetSerializeNamesWithoutDefault(ScriptableObject target)
        {
            var defaultNames = GetDefaultSerializeNames();
            return GetSerializeNames(target).Where(n => defaultNames.Contains(n) == false).ToArray();
        }

        public static List<string> GetDefaultSerializeNames()
        {
            if (_tempScriptableObject == null)
            {
                _tempScriptableObject = ScriptableObject.CreateInstance<EmptyScriptableObject>();
            }

            return GetSerializeNames(_tempScriptableObject);
        }

        public static List<string> GetSerializeNames(ScriptableObject target)
        {
            var list = new List<string>();
            foreach (var property in GetSerializedProperties(target))
            {
                list.Add(property.name);
            }

            return list;
        }

        public static IEnumerable<SerializedProperty> GetSerializedProperties(ScriptableObject target, bool isChildren = false, bool isApplyModified = false)
        {
            var serializedObject = new SerializedObject(target);
            return GetSerializedProperties(serializedObject, isChildren, isApplyModified);
        }

        public static IEnumerable<SerializedProperty> GetSerializedProperties(SerializedObject serializedObject, bool isChildren = false, bool isApplyModified = false)
        {
            var property = serializedObject.GetIterator();
            property.Next(true);

            yield return property;

            while (property.Next(isChildren))
            {
                yield return property;
            }

            if (isApplyModified)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
