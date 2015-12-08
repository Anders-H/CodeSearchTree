namespace CodeSearchTree
{
    public class SearchNode
    {
        public SearchNode(NodeType nodeType)
        {
            NodeType = nodeType;
            Index = -1;
            Name = "";
            AttributeName = "";
        }

        public SearchNode(NodeType nodeType, int index)
        {
            NodeType = nodeType;
            Index = index;
            Name = "";
            AttributeName = "";
        }

        public SearchNode(NodeType nodeType, string name)
        {
            NodeType = nodeType;
            Index = -1;
            Name = name;
            AttributeName = "";
        }

        public string AttributeName { get; set; }
        public string ReturnType { get; set; }
        public NodeType NodeType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
}