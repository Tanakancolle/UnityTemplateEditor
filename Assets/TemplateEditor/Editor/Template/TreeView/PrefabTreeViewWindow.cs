﻿using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PrefabTreeViewWindow : EditorWindow
{
    private static PrefabTreeViewWindow _window;

    public static void Open(GameObject parent, GameObject target, Action<GameObject> changeAction)
    {
        _window = GetWindow<PrefabTreeViewWindow>(true);
        _window.Build(parent, target, changeAction);
    }

    private PrefabTreeView _treeView;
    private TreeViewState _state;
    private Action<GameObject> _changeAction;

    void Build(GameObject parent, GameObject target, Action<GameObject> changeAction)
    {
        _state = new TreeViewState();
        _treeView = new PrefabTreeView(_state, parent);
        _treeView.Reload();
        _treeView.SelectObject(target);

        this._changeAction = changeAction;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("変更"))
            {
                _changeAction.Invoke(_treeView.GetSelectObject());
                _window.Close();
            }

            if (GUILayout.Button("戻る"))
            {
                _window.Close();
            }
        }

        var lastRect = GUILayoutUtility.GetLastRect();
        _treeView.OnGUI(new Rect(0, lastRect.y + lastRect.height, position.width, position.height));
    }

    void OnDestroy()
    {
        _window = null;
    }
}