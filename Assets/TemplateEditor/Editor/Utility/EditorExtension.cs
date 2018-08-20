using System;
using System.Collections.Generic;
using SyntaxHighlightEditor;
using UnityEditor;
using UnityEngine;

namespace TemplateEditor
{
    public class FoldoutInfo
    {
        public bool IsFoldout = true;
        public string DrawName;
        public Action DrawAction;

        public FoldoutInfo(string name, Action action)
        {
            DrawName = name;
            DrawAction = action;
        }
    }

    public static class EditorGUIHelper
    {
        private const float DefaultDDAreaHeight = 30.0f;

        /// <summary>
        /// ドラッグ＆ドロップ エリアを表示
        /// </summary>
        public static string[] DrawDragAndDropArea(string dropName = "Drag & Drop a folder or file", float height = DefaultDDAreaHeight)
        {
            var dropRect = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));

            // TODO : Cache ?
            var style = new GUIStyle(GUI.skin.box);
            {
                style.normal.textColor = Color.white;
            }
            GUI.Box(dropRect, dropName, style);

            if (dropRect.Contains(Event.current.mousePosition) == false)
            {
                return null;
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                {
                    DragAndDrop.activeControlID = GUIUtility.GetControlID(FocusType.Passive);
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                    break;

                case EventType.DragPerform:
                {
                    DragAndDrop.activeControlID = GUIUtility.GetControlID(FocusType.Passive);
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
                    return DragAndDrop.paths;
            }

            return null;
        }

        /// <summary>
        /// 折りたたみ表示
        /// </summary>
        public static void DrawFoldouts(IEnumerable<FoldoutInfo> foldouts)
        {
            foreach (var foldout in foldouts)
            {
                DrawFoldout(foldout);
            }
        }

        /// <summary>
        /// 折りたたみ表示
        /// </summary>
        public static void DrawFoldout(FoldoutInfo foldout)
        {
            EditorGUI.indentLevel++;
            foldout.IsFoldout = EditorGUILayout.Foldout(foldout.IsFoldout, foldout.DrawName);
            EditorGUI.indentLevel--;
            if (foldout.IsFoldout == false)
            {
                return;
            }

            foldout.DrawAction();
        }

        /// <summary>
        /// スコープのスタイル取得
        /// </summary>
        public static GUIStyle GetScopeStyle()
        {
            return EditorStyles.helpBox;
        }
    }
}