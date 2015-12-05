namespace CodeSearchTree
{
    public class SearchNode
    {
        public SearchNode(NodeType nodeType)
        {
            NodeType = nodeType;
            Index = -1;
            Name = "";
        }

        public SearchNode(NodeType nodeType, int index)
        {
            NodeType = nodeType;
            Index = index;
            Name = "";
        }

        public SearchNode(NodeType nodeType, string name)
        {
            NodeType = nodeType;
            Index = -1;
            Name = name;
        }

        public NodeType NodeType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
}