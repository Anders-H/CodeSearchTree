using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
    public interface INode
    {
        bool EntityIsNode { get; }
        bool EntityIsNodeList { get; }
        NodeList GetChildren(NodeType[] type);
        Node GetChild(params NodeType[] type);
        Node GetChild(string searchExpression);
        Node GetChild(params SearchNode[] sn);
    }
}
