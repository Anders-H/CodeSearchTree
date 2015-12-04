using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeSearchTree
{
   public class FileSystem
   {
      /// <summary>
      /// Searches using the given search expression in the root of the code tree of each C# file.
      /// </summary>
      /// <param name="directory_name">Directory to search in.</param>
      /// <param name="search_expression">The search expression that must be matched from root.</param>
      /// <param name="include_child_directories"></param>
      /// <param name="approximately_maximum_result"></param>
      /// <returns></returns>
      public static List<NodeList> CreateTreesFromFolder(string directory_name, string search_expression, bool include_child_directories, int approximately_maximum_result)
      {
         var parser = new SearchExpressionParser(search_expression);
         var search_nodes = parser.Parse();
         var files = GetCsFiles(directory_name, include_child_directories, approximately_maximum_result);
         var result = new List<NodeList>();
         foreach (var file in files)
         {
            var tree = Node.CreateTreeFromFile(file.FullName);
            if (!(tree.GetChild(search_expression) == null))
               result.Add(tree);
         }
         return result;
      }

      /// <summary>
      /// Searches using the given search expressen in each node of the code tree of each C# file.
      /// </summary>
      /// <param name="directory_name">Directory to search in.</param>
      /// <param name="search_expression">The search expression that must be matched from any node.</param>
      /// <param name="include_child_directories"></param>
      /// <param name="approximately_maximum_result"></param>
      /// <returns></returns>
      public static List<NodeList> DeepSearch(string directory_name, string search_expression, bool include_child_directories, int approximately_maximum_result)
      {
         var parser = new SearchExpressionParser(search_expression);
         var search_nodes = parser.Parse();
         var files = GetCsFiles(directory_name, include_child_directories, approximately_maximum_result);
         var result = new List<NodeList>();
         foreach (var file in files)
         {
            var tree = Node.CreateTreeFromFile(file.FullName);
            if (tree.DeepSearch(search_expression).Count > 0)
               result.Add(tree);
         }
         return result;
      }

      public static List<FileInfo> GetCsFiles(string directory_name, bool include_child_directories, int approximately_maximum_result)
      {
         var ret = new List<FileInfo>();
         var parent = new DirectoryInfo(directory_name);
         if (parent.Exists)
         {
            ret.AddRange(parent.GetFiles().Where(x => string.Compare(x.Extension, ".cs", true) == 0));
            if (include_child_directories && ret.Count < approximately_maximum_result)
               parent.GetDirectories().ToList().ForEach(x => AddChildFiles(x, ret, approximately_maximum_result));
         }
         return ret;
      }

      private static void AddChildFiles(System.IO.DirectoryInfo parent, List<FileInfo> result, int approximately_maximum_result)
      {
         result.AddRange(parent.GetFiles().Where(x => string.Compare(x.Extension, ".cs", true) == 0));
         if (result.Count < approximately_maximum_result)
            parent.GetDirectories().ToList().ForEach(x => AddChildFiles(x, result, approximately_maximum_result));
      }
   }
}
