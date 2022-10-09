using System.Globalization;

namespace CodeSearchTree
{
    public class SearchNode
    {
        public string AttributeName { get; }
        public string ReturnType { get; }
        public NodeType NodeType { get; }
        public int Index { get; }
        public string Name { get; }

        private SearchNode(NodeType nodeType, int index, string name, string returnType, string attributeName)
        {
            NodeType = nodeType;
            Index = index;
            Name = name;
            ReturnType = returnType;
            AttributeName = attributeName;
        }
        
        public static SearchNode Create(NodeType nodeType, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return CreateSearchByType(nodeType);
            
            if (expression.StartsWith("@"))
                return CreateSearchByTypeAndAttribute(nodeType, expression.Substring(1));
            
            if (expression.StartsWith("#"))
                return CreateSearchByTypeAndReturnType(nodeType, expression.Substring(1));
            
            if (int.TryParse(expression, NumberStyles.Any, CultureInfo.InvariantCulture, out var parseTest))
                return CreateSearchByTypeAndIndex(nodeType, parseTest);
            
            return CreateSearchByTypeAndName(nodeType, expression);
        }
        
        public static SearchNode CreateSearchByType(NodeType nodeType) =>
            new SearchNode(nodeType, -1, "", "", "");
        
        public static SearchNode CreateSearchByTypeAndIndex(NodeType nodeType, int index) =>
            new SearchNode(nodeType, index, "", "", "");
        
        public static SearchNode CreateSearchByTypeAndAttribute(NodeType nodeType, string attributeName) =>
            new SearchNode(nodeType, -1, "", "", attributeName);
        
        public static SearchNode CreateSearchByTypeAndReturnType(NodeType nodeType, string returnType) =>
            new SearchNode(nodeType, -1, "", returnType, "");
        
        public static SearchNode CreateSearchByTypeAndName(NodeType nodeType, string name) =>
            new SearchNode(nodeType, -1, name, "", "");
    }
}