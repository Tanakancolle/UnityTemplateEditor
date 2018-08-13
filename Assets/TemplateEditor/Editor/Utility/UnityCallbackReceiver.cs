using System;
using UnityEditor;

namespace TemplateEditor
{
    public class UnityCallbackReceiver
    {
        [InitializeOnLoadMethod]
        private static void LoadMethod()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            EditorApplication.delayCall += TemplatePrefabCreator.CreateTempSettingPrefab;
        }
    }
}
