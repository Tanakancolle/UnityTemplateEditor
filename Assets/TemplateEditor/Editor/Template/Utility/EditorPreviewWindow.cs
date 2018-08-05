using UnityEditor;

namespace TemplateEditor
{
    /// <summary>
    /// エディタプレビュー用ウィンドウ
    /// </summary>
    public class EditorPreviewWindow : EditorWindow
    {
        public static void Open(Editor editor)
        {
            var window = GetWindow<EditorPreviewWindow>(true);
            window.SetEditor(editor);
        }

        private Editor _previewEditor;

        public void SetEditor(Editor editor)
        {
            _previewEditor = editor;
        }

        private void OnGUI()
        {
            if (_previewEditor == null)
            {
                return;
            }

            _previewEditor.OnInspectorGUI();
        }
    }
}