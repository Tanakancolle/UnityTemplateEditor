using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEditorInternal.VR;
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

        private UnityWebRequest _request;
        private ReleaseExecutorWindow.ReleaseParameter _parameter;
        private string _releaseId;
        private int _uploadIndex;
        private Action _onComplate;

        public void Execute(ReleaseExecutorWindow.ReleaseParameter parameter, Action onComplate)
        {
            _parameter = parameter;
            _onComplate = onComplate;

            var url = string.Format(ReleaseUrlFormat, _parameter.RepositoryPath);
            byte[] postData = Encoding.UTF8.GetBytes (CreateReleaseJson());
            _request = new UnityWebRequest(url, "POST");
            _request.uploadHandler = new UploadHandlerRaw(postData);
            _request.downloadHandler = new DownloadHandlerBuffer();
            _request.SetRequestHeader("Content-Type", "application/json");
            _request.SetRequestHeader("Authorization", "token " + _parameter.TokenValue);
            _request.SendWebRequest();
            EditorApplication.update += WaitRelease;
        }

        private void WaitRelease()
        {
            if (_request == null || _request.isDone == false)
            {
                return;
            }
            EditorApplication.update -= WaitRelease;
            OnReleased();
        }

        private void OnReleased()
        {
            if (_request.isHttpError)
            {
                Debug.LogError("Release Error : " + _request.error);
                _request.Dispose();
                _request = null;
                return;
            }

            _releaseId = GetId(_request.downloadHandler.text);

            _request.Dispose();
            _request = null;

            _uploadIndex = -1;
            StartUpload();
        }

        private void StartUpload()
        {
            ++_uploadIndex;
            if (_parameter.UploadFilePaths.Count <= _uploadIndex)
            {
                // 終了
                OnUploaded();
                return;
            }

            var url = string.Format(UploadUrlFormat, _parameter.RepositoryPath, _releaseId);
            var filePath = _parameter.UploadFilePaths[_uploadIndex];
            _request = new UnityWebRequest(url + Path.GetFileName(filePath), "POST");
            var postData = ReadFile(filePath);
            _request.uploadHandler = new UploadHandlerRaw(postData);
            _request.downloadHandler = new DownloadHandlerBuffer();
            _request.SetRequestHeader("Content-Type", "application/octet-stream");
            _request.SetRequestHeader("Authorization", "token " + _parameter.TokenValue);
            _request.SendWebRequest();
            EditorApplication.update += WaitUpload;
        }

        private void WaitUpload()
        {
            if (_request == null || _request.isDone == false)
            {
                return;
            }
            EditorApplication.update -= WaitUpload;

            if (_request.isHttpError)
            {
                Debug.LogErrorFormat("Upload Error : Index {0}, {1}", _uploadIndex, _request.error);
                _request.Dispose();
                _request = null;
                return;
            }
            _request.Dispose();
            _request = null;

            StartUpload();
        }

        private void OnUploaded()
        {
            _onComplate?.Invoke();
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