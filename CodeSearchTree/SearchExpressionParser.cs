using System;
using System.Text.RegularExpressions;

namespace CodeSearchTree
{
    internal sealed class SearchExpressionParser
    {
        private string Source { get; }

        public SearchExpressionParser(string source)
        {
            Source = source?.Trim() ?? "";
        }

        public SearchNodeList Parse()
        {
            var ret = new SearchNodeList();

            if (Source == "")
                return ret;
            
            var parts = Source.Split('/');
            const string noIndex = @"^(\*|[a-z]+)$";
            const string withIndex = @"^(\*|[a-z]+)\[[0-9]+\]$";
            const string withAttribute = @"^(\*|[a-z]+)\[@.+\]$";
            const string withReturnType = @"^(\*|[a-z]+)\[#.+\]$";
            const string withName = @"^(\*|[a-z]+)\[.+\]$";

            foreach (var part in parts)
            {
                if (Regex.IsMatch(part, noIndex))
                {
                    ret.Add(SearchNode.CreateSearchByType(KeywordToEnum(part)));
                }
                else if (Regex.IsMatch(part, withIndex))
                {
                    var open = part.IndexOf('[');
                    var close = part.IndexOf(']');
                    var indexString = part.Substring(open + 1, close - (open + 1)).Trim();
                    var index = int.Parse(indexString);
                    ret.Add(SearchNode.CreateSearchByTypeAndIndex(KeywordToEnum(part.Substring(0, open)), index));
                }
                else if (Regex.IsMatch(part, withAttribute))
                {
                    var open = part.IndexOf('[');
                    var close = part.IndexOf(']');
                    var attributeName = part.Substring(open + 2, close - (open + 2)).Trim();
                    ret.Add(SearchNode.CreateSearchByTypeAndAttribute(KeywordToEnum(part.Substring(0, open)), attributeName));
                }
                else if (Regex.IsMatch(part, withReturnType))
                {
                    var open = part.IndexOf('[');
                    var close = part.IndexOf(']');
                    var returnType = part.Substring(open + 2, close - (open + 2)).Trim();
                    ret.Add(SearchNode.CreateSearchByTypeAndReturnType(KeywordToEnum(part.Substring(0, open)), returnType));
                }
                else if (Regex.IsMatch(part, withName))
                {
                    var open = part.IndexOf('[');
                    var close = part.IndexOf(']');
                    var name = part.Substring(open + 1, close - (open + 1)).Trim();
                    ret.Add(SearchNode.CreateSearchByTypeAndName(KeywordToEnum(part.Substring(0, open)), name));
                }
                else
                    throw new Exception("Query expression contains errors.");
            }
            return ret;
        }

        private static NodeType KeywordToEnum(string keyword) =>
            SearchExpressionTranslationLists.KeywordToNodeType.Translate(keyword);

        internal static string NodeTypeToKeyword(NodeType n) =>
            SearchExpressionTranslationLists.NodeTypeToKeyword.Translate(n);
    }
}