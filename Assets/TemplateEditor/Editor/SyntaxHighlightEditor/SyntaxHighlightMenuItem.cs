using UnityEditor;

namespace SyntaxHighlightEditor
{
    public class SyntaxHighlightMenuItem
    {
        private enum Priority
        {
            ResetCSharpEditor,
        }

        private const string MenuItemPrefix = "Tools/Syntax Highlight Editor/";
        private const int OriginalPriorityNumber = 2000;

        [MenuItem(MenuItemPrefix + "Reset CSharp Editor", false, OriginalPriorityNumber + (int) Priority.ResetCSharpEditor)]
        public static void ResetCSharpEditor()
        {
            SyntaxHighlightUtility.ResetCSharpEditor();
        }
    }
}
