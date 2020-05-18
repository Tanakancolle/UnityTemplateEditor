using System;
using UnityEditor;

public partial class TemplateSettingPropertyGetter
{
    public enum Property
    {
        @Path,
        @ScriptName,
        @Code,
        @CodeAreaHeight,
        @Overwrite,
        @Chain,
        @DuplicatePrefab,
        @AttachTarget,
        @PrefabPath,
        @PrefabName,
        @AssetsMenuItem,
        @Description,
        @IsFoldouts,
        @ScrollPos,
    }

    private static readonly string[] PropertyNames =
    {
        "Path",
        "ScriptName",
        "Code",
        "CodeAreaHeight",
        "Overwrite",
        "Chain",
        "DuplicatePrefab",
        "AttachTarget",
        "PrefabPath",
        "PrefabName",
        "AssetsMenuItem",
        "Description",
        "IsFoldouts",
        "ScrollPos",
    };

    private readonly SerializedProperty[] _properties;

    public TemplateSettingPropertyGetter(SerializedObject targetSerializedObject)
    {
        var names = Enum.GetNames(typeof(Property));
        _properties = new SerializedProperty[names.Length];
        for (int i = 0; i < _properties.Length; ++i)
        {
            _properties[i] = targetSerializedObject.FindProperty(names[i]);
        }
    }

    public SerializedProperty GetProperty(Property type)
    {
        return _properties[(int) type];
    }
}