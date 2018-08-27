using UnityEditor;
using UnityEngine;

namespace ReleaseExecutor
{
    public static class Utility
    {
        public const string ReplaceTagText = "{tag}";

        public static T FindAssetFromType<T>() where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            foreach (var guid in guids)
            {
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
            }

            return default(T);
        }
    }
}