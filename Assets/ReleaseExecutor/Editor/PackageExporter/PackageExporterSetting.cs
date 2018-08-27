using UnityEngine;
using UnityEditor;

namespace ReleaseExecutor
{
    public class PackageExporterSetting : ScriptableObject
    {
        public string ExportName;
        public string[] TargetPaths;
        public ExportPackageOptions[] Options =
        {
            ExportPackageOptions.Recurse
        };
    }
}
