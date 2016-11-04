using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CodeSearchTree
{
    public class FileSystem
    {
        /// <summary>
        /// Searches using the given search expression in the root of the code tree of each C# file.
        /// </summary>
        /// <param name="directoryName">Directory to search in.</param>
        /// <param name="searchExpression">The search expression that must be matched from root.</param>
        /// <param name="includeChildDirectories"></param>
        /// <param name="approximatelyMaximumResult"></param>
        /// <returns></returns>
        public static List<NodeList> CreateTreesFromFolder(string directoryName, string searchExpression, bool includeChildDirectories, int approximatelyMaximumResult)
        {
            var parser = new SearchExpressionParser(searchExpression);
            var searchNodes = parser.Parse().ToArray();
            var files = GetCsFiles(directoryName, includeChildDirectories, approximatelyMaximumResult);
            return files.Select(file => Node.CreateTreeFromFile(file.FullName)).Where(tree => tree.GetChild(searchNodes) != null).ToList();
        }
        /// <summary>
        /// Searches using the given search expressen in each node of the code tree of each C# file.
        /// </summary>
        /// <param name="directoryName">Directory to search in.</param>
        /// <param name="searchExpression">The search expression that must be matched from any node.</param>
        /// <param name="includeChildDirectories"></param>
        /// <param name="approximatelyMaximumResult"></param>
        /// <returns></returns>
        public static List<NodeList> DeepSearch(string directoryName, string searchExpression, bool includeChildDirectories, int approximatelyMaximumResult)
        {
            var parser = new SearchExpressionParser(searchExpression);
            var searchNodes = parser.Parse().ToArray();
            var files = GetCsFiles(directoryName, includeChildDirectories, approximatelyMaximumResult);
            return files.Select(file => Node.CreateTreeFromFile(file.FullName)).Where(tree => tree.DeepSearch(searchNodes).Count > 0).ToList();
        }
        /// <summary>
        /// Returns all .cs files in a given directory.
        /// </summary>
        /// <param name="directoryName">A folder containing the files to be returned.</param>
        /// <param name="includeChildDirectories"></param>
        /// <param name="approximatelyMaximumResult"></param>
        /// <returns></returns>
        public static List<FileInfo> GetCsFiles(string directoryName, bool includeChildDirectories, int approximatelyMaximumResult)
        {
            var ret = new List<FileInfo>();
            var parent = new DirectoryInfo(directoryName);
            if (!parent.Exists) return ret;
            ret.AddRange(parent.GetFiles().Where(x => string.Compare(x.Extension, ".cs", StringComparison.OrdinalIgnoreCase) == 0));
            if (includeChildDirectories && ret.Count < approximatelyMaximumResult)
                parent.GetDirectories().ToList().ForEach(x => AddChildFiles(x, ret, approximatelyMaximumResult));
            return ret;
        }
        private static void AddChildFiles(DirectoryInfo parent, List<FileInfo> result, int approximatelyMaximumResult)
        {
            result.AddRange(parent.GetFiles().Where(x => string.Compare(x.Extension, ".cs", StringComparison.OrdinalIgnoreCase) == 0));
            if (result.Count < approximatelyMaximumResult)
                parent.GetDirectories().ToList().ForEach(x => AddChildFiles(x, result, approximatelyMaximumResult));
        }
    }
}
