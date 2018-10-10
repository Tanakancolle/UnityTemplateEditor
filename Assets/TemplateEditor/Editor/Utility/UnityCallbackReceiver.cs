using System;
using UnityEditor;

namespace TemplateEditor
{
    public static class UnityCallbackReceiver
    {
        [InitializeOnLoadMethod]
        private static void LoadMethod()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.delayCall += TemplatePrefabCreator.CreateTempSettingPrefab;
        }
    }
}
