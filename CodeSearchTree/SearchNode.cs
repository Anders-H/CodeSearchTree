using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
   public class SearchNode
   {
      public Node.NodeTypes NodeType { get; set; }
      public int Index { get; set; }
      public string Name { get; set; }

      public SearchNode(Node.NodeTypes node_type)
      {
         this.NodeType = node_type;
         this.Index = -1;
         this.Name = "";
      }

      public SearchNode(Node.NodeTypes node_type, int index)
      {
         this.NodeType = node_type;
         this.Index = index;
         this.Name = "";
      }

      public SearchNode(Node.NodeTypes node_type, string name)
      {
         this.NodeType = node_type;
         this.Index = -1;
         this.Name = name;
      }
   }
}
