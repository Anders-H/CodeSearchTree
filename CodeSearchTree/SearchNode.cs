namespace CodeSearchTree
{
    public class SearchNode
    {
        private SearchNode(NodeType nodeType, int index, string name, string returnType, string attributeName)
        {
            NodeType = nodeType;
            Index = index;
            Name = name;
            ReturnType = returnType;
            AttributeName = attributeName;
        }

        public static SearchNode CreateSearchByType(NodeType nodeType)
            => new SearchNode(nodeType, -1, "", "", "");

        public static SearchNode CreateSearchByTypeAndIndex(NodeType nodeType, int index)
            => new SearchNode(nodeType, index, "", "", "");

        public static SearchNode CerateSearchByTypeAndAttribute(NodeType nodeType, string attributeName)
            => new SearchNode(nodeType, -1, "", "", attributeName);

        public static SearchNode CerateSearchByTypeAndReturnType(NodeType nodeType, string returnType)
            => new SearchNode(nodeType, -1, "", returnType, "");

        public static SearchNode CerateSearchByTypeAndName(NodeType nodeType, string name)
            => new SearchNode(nodeType, -1, name, "", "");

        public string AttributeName { get; private set; }
        public string ReturnType { get; private set; }
        public NodeType NodeType { get; private set; }
        public int Index { get; private set; }
        public string Name { get; private set; }
    }
}