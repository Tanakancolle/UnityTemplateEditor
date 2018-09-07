using System;
using System.IO;
using System.Text.RegularExpressions;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace ReleaseExecutor
{
    public class ReleaseExecutorWindow : EditorWindow
    {
        public static void Open()
        {
            GetWindow<ReleaseExecutorWindow>(true);
        }

        public class ReleaseParameter
        {
            public string BranchName = string.Empty;
            public string TagName = string.Empty;
            public string UserName = string.Empty;
            public string TokenValue = string.Empty;
            public string RepositoryPath = string.Empty;
            public string ReleaseName = string.Empty;
            public string Description = string.Empty;
            public List<string> UploadFilePaths = new List<string>();
        }

        private enum ExecutorType
        {
            GitHub,
        }

        private enum SaveKeyType
        {
            Branch,
            TagName,
            UserName,
            Token,
        }

        private static readonly string SaveKeyFormat = "ReleaseExecutor.{0}";

        private static readonly string[] SaveKeys =
        {
            "Branch",
            "TagName",
            "UserName",
            "Token",
        };

        private ReleaseParameter _releaseParameter = new ReleaseParameter();
        private ReleaseExecutorSetting _releaseSetting;
        private Editor _releaseSettingEditor;
        private ExecutorType _executorType;
        private Vector2 _scrollPos;

        void Awake()
        {
            _releaseParameter.BranchName = EditorUserSettings.GetConfigValue(GetSaveKey(SaveKeyType.Branch));
            _releaseParameter.TagName = EditorUserSettings.GetConfigValue(GetSaveKey(SaveKeyType.TagName));
            _releaseParameter.UserName = EditorUserSettings.GetConfigValue(GetSaveKey(SaveKeyType.UserName));
            _releaseParameter.TokenValue = EditorUserSettings.GetConfigValue(GetSaveKey(SaveKeyType.Token));
            _releaseSetting = Utility.FindAssetFromType<ReleaseExecutorSetting>();

            _releaseSettingEditor = Editor.CreateEditor(_releaseSetting, typeof(ReleaseExecutorSetting));
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("リリースタイプ");
                _executorType = (ExecutorType)EditorGUILayout.EnumPopup(_executorType);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("ブランチ名");
                _releaseParameter.BranchName = EditorGUILayout.TextArea(_releaseParameter.BranchName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("タグ名");
                _releaseParameter.TagName = EditorGUILayout.TextArea(_releaseParameter.TagName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Githubユーザー名");
                _releaseParameter.UserName = EditorGUILayout.TextArea(_releaseParameter.UserName);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Githubトークン");
                _releaseParameter.TokenValue = EditorGUILayout.TextArea(_releaseParameter.TokenValue);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("リリース設定");
                var setting = EditorGUILayout.ObjectField(_releaseSetting, typeof(ReleaseExecutorSetting), false) as ReleaseExecutorSetting;
                if (setting != _releaseSetting)
                {
                    _releaseSetting = setting;
                }
            }

            _releaseSettingEditor = Editor.CreateEditor(_releaseSetting);
            if (_releaseSettingEditor)
            {
                _releaseSettingEditor.OnInspectorGUI();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("リリース"))
            {
                Release();
            }
        }

        private void Release()
        {
            if (string.IsNullOrEmpty(_releaseParameter.TagName))
            {
                Debug.LogWarning("invalid tag name");
                return;
            }

            if (string.IsNullOrEmpty(_releaseParameter.UserName))
            {
                Debug.LogWarning("invalid user name");
                return;
            }

            if (string.IsNullOrEmpty(_releaseParameter.TokenValue))
            {
                Debug.LogWarning("invalid token");
                return;
            }

            if (_releaseSetting == null)
            {
                Debug.LogWarning("invalid setting");
                return;
            }

            EditorUserSettings.SetConfigValue(GetSaveKey(SaveKeyType.Branch), _releaseParameter.BranchName);
            EditorUserSettings.SetConfigValue(GetSaveKey(SaveKeyType.TagName), _releaseParameter.TagName);
            EditorUserSettings.SetConfigValue(GetSaveKey(SaveKeyType.UserName), _releaseParameter.UserName);
            EditorUserSettings.SetConfigValue(GetSaveKey(SaveKeyType.Token), _releaseParameter.TokenValue);

            _releaseParameter.UploadFilePaths.Clear();
            foreach (var packageSetting in _releaseSetting.UploadPackage)
            {
                PackageExporter.RunExpotPackage(packageSetting);
                _releaseParameter.UploadFilePaths.Add(PackageExporter.AddExportExtension(packageSetting.ExportName));
            }

            // リリース詳細
            try
            {
                var changeLog = File.ReadAllText(_releaseSetting.ChangeLogPath);

                var searchPattern = _releaseSetting.LogStartPattern.Replace(Utility.ReplaceTagText, _releaseParameter.TagName);
                var startMatch = Regex.Match(changeLog, searchPattern);
                var searchIndex = startMatch.Index + searchPattern.Length;
                var endMatch = Regex.Match(
                    changeLog.Substring(searchIndex),
                    _releaseSetting.LogEndPattern.Replace(Utility.ReplaceTagText, _releaseParameter.TagName)
                );

                var endIndex = endMatch.Index - 1;
                if (endIndex < 0)
                {
                    // マッチしない場合
                    endIndex = changeLog.Length - startMatch.Index;
                }
                else
                {
                    // マッチした場合
                    endIndex += searchPattern.Length;
                }

                _releaseParameter.Description = changeLog.Substring(startMatch.Index, endIndex);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Change Log Error: {0}", e);
            }

            _releaseParameter.ReleaseName = _releaseParameter.TagName;
            _releaseParameter.RepositoryPath = _releaseSetting.RepositoryPath;

            var executor = CreateExecutor(_executorType);
            executor.Execute(_releaseParameter, () => Debug.Log("Release Complete"));
        }

        private string GetSaveKey(SaveKeyType type)
        {
            return string.Format(SaveKeyFormat, SaveKeys[(int) type]);
        }

        private IReleaseExecutor CreateExecutor(ExecutorType type)
        {
            switch (type)
            {
                case ExecutorType.GitHub: return new GitHubReleaseExecutor();
            }

            return null;
        }
    }
}