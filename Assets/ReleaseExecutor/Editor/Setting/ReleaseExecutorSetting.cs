using UnityEngine;

namespace ReleaseExecutor
{
    public class ReleaseExecutorSetting : ScriptableObject
    {
        public string RepositoryPath;
        public string ChangeLogPath = "CHANGELOG.md";
        public string LogStartPattern = string.Format("### {0}", Utility.ReplaceTagText);
        public string LogEndPattern = "###";
        public PackageExporterSetting[] UploadPackage;
    }
}