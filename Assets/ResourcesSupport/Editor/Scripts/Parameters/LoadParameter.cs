using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace TemplateEditor
{
    public class LoadParameter : ScriptableObject, IUsings
    {
        public string TypeName;

        [EnumFlags]
        public ResourcesLoaderUtility.LoadType EditLoadType = ResourcesLoaderUtility.LoadType.Load;

        public string[] TargetExtensions;

        public string[] EditUsings;
        public string[] usings { get { return EditUsings; } }
    }
}
