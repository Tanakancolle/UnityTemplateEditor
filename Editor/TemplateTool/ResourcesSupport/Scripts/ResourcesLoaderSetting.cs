#pragma warning disable CS0649

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Text;

namespace TemplateEditor
{
    public class ResourcesLoaderSetting : ScriptableObject, IUsings, IProcessChain
    {
        private static readonly string ScriptName = "ResourcesLoader";

        private enum ReplaceWordType
        {
            CreatePath,
            ScriptName,
            Usings,
            Enums,
            Paths,
            Methods,
        }

        private static readonly string[] ReplaceWords =
        {
            "CreatePath",
            "ScriptName",
            "Usings",
            "Enums",
            "Paths",
            "Methods",
        };

        private enum TabSpaceType
        {
            Enum,
            Path,
        }

        private static readonly string[] TabSpaceWords = new[]
        {
            "EnumTab",
            "PathTab",
        };

        #region IProcessChain implementation

        public void Process(ProcessMetadata metadata, ProcessDictionary result)
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

                filePathsList.Add(new string[] {parameter.TypeName, string.Join(",\n" + GetTabSpace(TabSpaceType.Path, result), paths.Select(path => "\"" + path + "\"").ToArray())});

                // ファイルパスからファイル名を取得
                // TODO : StringBuilderExtension.ConvertEnumName
                var fileNames = paths.Select(Path.GetFileNameWithoutExtension).Select(StringBuilderExtension.ConvertEnumName);
                fileNamesList.Add(new string[] {parameter.TypeName, string.Join(",\n" + GetTabSpace(TabSpaceType.Enum, result), fileNames.ToArray())});

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

            // 生成パス設定
            result.Add(ReplaceWords[(int)ReplaceWordType.CreatePath], TemplateUtility.GetFilePathFromFileName(ScriptName + ".cs") ?? "Assets");

            // スクリプト名設定
            result.Add(ReplaceWords[(int) ReplaceWordType.ScriptName], ScriptName);

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
                    ReplaceWords[(int) ReplaceWordType.Usings],
                    usingNames.ToArray()
                );
            }

            // Enums
            {
                result.Add(
                    ReplaceWords[(int) ReplaceWordType.Enums],
                    fileNamesList
                );
            }

            // Paths
            {
                result.Add(
                    ReplaceWords[(int) ReplaceWordType.Paths],
                    filePathsList
                );
            }

            // Methods
            {
                result.Add(
                    ReplaceWords[(int) ReplaceWordType.Methods],
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
            var builder = new StringBuilder();
            foreach (var word in TabSpaceWords)
            {
                builder.Append(word + ", ");
            }

            return "設定に従い、Resourcesフォルダ内のアセット情報を受け渡します\n※次の置き換え文字を設定する必要があります : " + builder;
        }

        private string GetTabSpace(TabSpaceType type, ProcessDictionary result)
        {
            var tabSpace = string.Empty;
            object tabSpaceObject;
            if (result.TryGetValue(TabSpaceWords[(int)type], out tabSpaceObject))
            {
                tabSpace = tabSpaceObject.ToString();
            }

            return tabSpace;
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
