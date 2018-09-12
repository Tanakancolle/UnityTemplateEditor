using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ReleaseExecutor
{
    public class GitHubReleaseExecutor : IReleaseExecutor
    {
        private const string ReleaseUrlFormat = "https://api.github.com/repos/{0}/releases";
        private const string UploadUrlFormat = "https://uploads.github.com/repos/{0}/releases/{1}/assets?name=";

        /// <summary>
        /// リリース用パラメーター
        /// github apiの仕様に合わせる必要があります
        /// </summary>
        [Serializable]
        private class ReleaseParameter
        {
            public string tag_name;
            public string target_commitish;
            public string name;
            public string body;
            public bool draft = false;
            public bool prerelease = false;
        }

        private ReleaseExecutorWindow.ReleaseParameter _parameter;
        private string _releaseId;
        private Action _onComplete;

        public void Execute(ReleaseExecutorWindow.ReleaseParameter parameter, Action onComplete)
        {
            _parameter = parameter;
            _onComplete = onComplete;

            StartRelease();
        }

        private async void StartRelease()
        {
            var url = string.Format(ReleaseUrlFormat, _parameter.RepositoryPath);
            byte[] postData = Encoding.UTF8.GetBytes (CreateReleaseJson());
            using (var request = SetupWebRequest(url, postData))
            {
                await request.SendWebRequest();

                if (CheckError(request, "Release Error : "))
                {
                    return;
                }

                _releaseId = GetId(request.downloadHandler.text);
            }

            // アップロード開始
            StartUpload();
        }

        private async void StartUpload()
        {
            var url = string.Format(UploadUrlFormat, _parameter.RepositoryPath, _releaseId);

            foreach (var filePath in _parameter.UploadFilePaths)
            {
                var postData = ReadFile(filePath);
                using (var request = SetupWebRequest(url + Path.GetFileName(filePath), postData, "application/octet-stream"))
                {
                    await request.SendWebRequest();
                    if (CheckError(request, "Release Upload Error : "))
                    {
                        return;
                    }
                }
            }

            // リリース完了
            _onComplete?.Invoke();
        }

        private UnityWebRequest SetupWebRequest(string url, byte[] postData, string contentType = "application/json")
        {
            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", contentType);
            request.SetRequestHeader("Authorization", "token " + _parameter.TokenValue);
            return request;
        }

        private bool CheckError(UnityWebRequest request, string prefix)
        {
            if (string.IsNullOrEmpty(request.error))
            {
                return false;
            }
            
            Debug.LogError(prefix + request.error);
            return true;
        }

        private string CreateReleaseJson()
        {
            var parameter = new ReleaseParameter
            {
                tag_name = _parameter.TagName,
                target_commitish = _parameter.BranchName,
                name = _parameter.ReleaseName,
                body = _parameter.Description
            };

            return JsonUtility.ToJson(parameter);
        }

        private byte[] ReadFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        private string GetId(string text)
        {
            const string idWord = "id";
            var index = text.IndexOf(idWord, StringComparison.Ordinal) + idWord.Length;
            var isId = false;
            var isNumberStart = false;
            var builder = new StringBuilder();
            for (var i = index; i < text.Length; ++i)
            {
                var c = text[i];
                if (isId)
                {
                    var isNumber = char.IsNumber(c);

                    if (isNumberStart)
                    {
                        if (isNumber == false)
                        {
                            break;
                        }

                        builder.Append(c);
                    }
                    else if(isNumber)
                    {
                        builder.Append(c);
                        isNumberStart = true;
                    }
                }
                else if(c == '"')
                {
                    isId = true;
                }
            }

            return builder.ToString();
        }
    }
}