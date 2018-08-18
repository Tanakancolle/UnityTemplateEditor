using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Serialization;

namespace TemplateEditor
{
    public class ResourcesLoaderSetting : ScriptableObject, IUsings, IProcessChain
    {
        private enum ReplaceWordType
        {
            Usings,
            Enums,
            Paths,
            Methods,
        }

        private static readonly string[] ReplaceWords =
        {
            "Usings",
            "Enums",
            "Paths",
            "Methods",
        };

        #region IProcessChain implementation

        public void Process(ProcessMetadata metadata, Dictionary<string, object> result)
        {
            // ファイルパス取得
            var removeWord = "Resources/";
            var removeWordLength = removeWord.Length;

            // Assets内にあるResourcesフォルダの全てのパスを取得
            var resourcesPaths = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories);

            // Resourcesフォルダ内のファイルパスを取得
            // TODO : 最適化？
            var ignorePatterns = _ignorePathPatterns.Select(pattern => new Regex(pattern, RegexOptions.Compiled));
            var filePaths = ResourcesLoaderUtility.GetFilePaths(resourcesPaths, new string[] {".meta"})
                .Where(path => _ignoreFileNames.Contains(Path.GetFileNameWithoutExtension(path)) == false)
                .Where(path => ignorePatterns.Any(pattern => pattern.IsMatch(path)) == false)
                .OrderBy(path => Path.GetFileNameWithoutExtension(path));

            var filePathsList = new List<string[]>();
            var fileNamesList = new List<string[]>();
            var methodsList = new List<string[]>();
            var enumValues = Enum.GetValues(typeof(ResourcesLoaderUtility.LoadType)).Cast<int>().ToArray();
            foreach (var parameter in _parameters)
            {
                // 指定した拡張子のファイルパスを取得
                var paths = ResourcesLoaderUtility.FindByExtension(filePaths, parameter.TargetExtensions).Select(path =>
                {
                    var startIndex = path.IndexOf(removeWord) + removeWordLength;
                    var length = path.Length - startIndex - Path.GetExtension(path).Length;
                    return path.Substring(startIndex, length);
                });

                if (paths.Any() == false)
                {
                    continue;
                }

                filePathsList.Add(new string[] {parameter.TypeName, string.Join(", ", paths.Select(path => "\"" + path + "\"").ToArray())});

                // ファイルパスからファイル名を取得
                var fileNames = paths.Select(Path.GetFileNameWithoutExtension);
                fileNamesList.Add(new string[] {parameter.TypeName, string.Join(", ", fileNames.ToArray())});

                var intValue = (int) parameter.EditLoadType;
                for (int i = 0; i < enumValues.Length; ++i)
                {
                    if ((intValue & enumValues[i]) == 0)
                    {
                        continue;
                    }

                    methodsList.Add(new string[]
                    {
                        string.Format(ResourcesLoaderUtility.ReturnNamesFormat[i], parameter.TypeName),
                        ResourcesLoaderUtility.LoadNames[i],
                        parameter.TypeName
                    });
                }
            }

            // Usings
            {
                var usingsList = new List<IUsings>(_parameters.Length + 1);
                usingsList.Add(this);
                usingsList.AddRange(_parameters);

                var usingNames = new HashSet<string>();
                foreach (var usings in usingsList)
                {
                    foreach (var names in usings.usings)
                    {
                        usingNames.Add(names);
                    }
                }

                result.Add(
                    this.ConvertReplaceWord(ReplaceWords[(int) ReplaceWordType.Usings], result),
                    usingNames.ToArray()
                );
            }

            // Enums
            {
                result.Add(
                    this.ConvertReplaceWord(ReplaceWords[(int) ReplaceWordType.Enums], result),
                    fileNamesList
                );
            }

            // Paths
            {
                result.Add(
                    this.ConvertReplaceWord(ReplaceWords[(int) ReplaceWordType.Paths], result),
                    filePathsList
                );
            }

            // Methods
            {
                result.Add(
                    this.ConvertReplaceWord(ReplaceWords[(int) ReplaceWordType.Methods], result),
                    methodsList
                );
            }
        }

        public string[] GetReplaceWords()
        {
            return ReplaceWords;
        }

        public string GetDescription()
        {
            return "設定に従い、Resourcesフォルダ内のアセット情報を受け渡します";
        }

        public ProcessFileType GetFileType()
        {
            return ProcessFileType.ScriptableObject;
        }

        #endregion

        [SerializeField]
        private LoadParameter[] _parameters;

        [SerializeField]
        private string[] _editUsings;
        public string[] usings { get { return _editUsings; } }

        [Header("除外ファイル名")]
        [SerializeField]
        private string[] _ignoreFileNames;

        [Header("除外ファイルパスの正規表現 Assetsから始まる")]
        [SerializeField]
        private string[] _ignorePathPatterns;
    }
}
