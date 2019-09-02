using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TemplateEditor
{
    public class ResourcesLoaderUtility
    {
        [Flags]
        public enum LoadType
        {
            Load = 1,
            LoadAll = 1 << 1,
            LoadAsync = 1 << 2,
        }

        public static readonly string[] LoadNames = new string[]
        {
            "Load", "LoadAll", "LoadAsync",
        };

        public static readonly string[] ReturnNamesFormat = new string[]
        {
            "{0}", "{0}[]", "ResourceRequest",
        };

        public static IEnumerable<string> GetFilePaths(IEnumerable<string> paths, IEnumerable<string> ignoreExtensions)
        {
            var filePathList = new List<string>();
            foreach (var path in paths)
            {
                var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                filePathList.AddRange(files);
            }

            return filePathList
                .Where(path => !ignoreExtensions.Contains(Path.GetExtension(path))) // 無視する拡張子のファイルをフィルタリング
                .Select(path => path.Replace("\\", "/")); // ファイルパスの「￥」を「/」に変換（WindowsとMacの差を吸収するため）
        }

        /// <summary>
        /// 指定した拡張子のファイルパスを検索
        /// </summary>
        public static IEnumerable<string> FindByExtension(IEnumerable<string> paths, IEnumerable<string> extensions)
        {
            var builder = new StringBuilder();
            foreach (var extension in extensions)
            {
                if (builder.Length == 0)
                {
                    builder.Append(@".*\.(");
                }
                else
                {
                    builder.Append("|");
                }

                builder.Append(extension);
            }

            builder.Append(")");

            var regex = new Regex(@builder.ToString(), RegexOptions.IgnoreCase);

            return paths.Where(path => regex.IsMatch(path));
        }
    }
}
