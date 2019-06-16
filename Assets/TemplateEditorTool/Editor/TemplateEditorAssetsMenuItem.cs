using UnityEditor;
using System.Collections.Generic;

namespace TemplateEditor
{
    public class TemplateEditorAssetsMenuItem
    {
        private const string MenuItemPrefix = "Assets/Create/Template/";

        [MenuItem(MenuItemPrefix, false, 0)]
        public static void Dummy()
        {
        }

        [MenuItem(MenuItemPrefix + "UsedPrefabSetting", false, 0)]
        public static void UsedPrefabSetting()
        {
            TemplateUtility.OpenEditorWindow("d3fc99746fe524bc189500a3c374f44f");
        }


        [MenuItem(MenuItemPrefix + "SceneNamesSetting", false, 0)]
        public static void SceneNamesSetting()
        {
            TemplateUtility.OpenEditorWindow("cc262320af12f471d88568e0689f49c8");
        }


        [MenuItem(MenuItemPrefix + "TestSetting", false, 0)]
        public static void TestSetting()
        {
            TemplateUtility.OpenEditorWindow("79c13025c5d0e454994d5e58824d342e");
        }


        [MenuItem(MenuItemPrefix + "TestSetting2", false, 0)]
        public static void TestSetting2()
        {
            TemplateUtility.OpenEditorWindow("23a8bec1b1b824bd98e9ea6ceaa321b3");
        }


        [MenuItem(MenuItemPrefix + "MVP_Group", false, 1000)]
        public static void MVP_Group()
        {
            TemplateUtility.OpenEditorWindow("15ff2d174684c4665b69b2bec9fe16fe");
        }

    }
}