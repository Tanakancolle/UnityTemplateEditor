using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTreeView : TreeView
{
    public PrefabTreeView(TreeViewState state, GameObject obj) : base(state)
    {
        _target = obj;
    }

    private GameObject _target;
    private List<Transform> _transforms = new List<Transform>();

    protected override TreeViewItem BuildRoot()
    {
        int count = -1;
        int depth = -1;
        var root = new TreeViewItem(count, depth, "root");
        var parent = new TreeViewItem(++count, ++depth, _target.name);
        root.children = new List<TreeViewItem> {parent};
        _transforms.Clear();
        parent.children = BuildTree(_target.transform, depth, ref count);

        return root;
    }

    private List<TreeViewItem> BuildTree(Transform parent, int depth, ref int count)
    {
        _transforms.Add(parent);

        if (parent.childCount == 0)
        {
            return null;
        }

        depth++;
        var items = new List<TreeViewItem>();
        foreach (Transform child in parent)
        {
            var item = new TreeViewItem(++count, depth, child.name);
            var children = BuildTree(child, depth, ref count);
            if (children != null)
            {
                item.children = children;
            }

            items.Add(item);
        }

        return items;
    }

    /// <summary>
    /// 選択オブジェクト取得
    /// </summary>
    public GameObject GetSelectObject()
    {
        return _transforms[state.lastClickedID].gameObject;
    }

    /// <summary>
    /// 選択オブジェクト指定
    /// </summary>
    public void SelectObject(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        SetParentExpanded(target.transform.parent);
        SetSelection(new int[] {FindIndex(target.transform)});
    }

    private void SetParentExpanded(Transform transform)
    {
        if (transform == null)
        {
            return;
        }

        SetExpanded(FindIndex(transform), true);
        SetParentExpanded(transform.parent);
    }

    private int FindIndex(Transform transform)
    {
        return _transforms.IndexOf(transform);
    }
}
