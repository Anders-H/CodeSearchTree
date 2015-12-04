using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
   public class NodeList : List<Node>
   {

      /// <summary>
      /// Returns direct child at given index.
      /// </summary>
      /// <param name="index"></param>
      /// <returns></returns>
      internal NodeList FilterByTypeAndNameOrIndex(Node.NodeTypes type, int index)
      {
         var type_filtered = this.FilterByNameOrIndexOrType(type);
         var ret = new NodeList();
         if (type_filtered.Count > index)
            ret.Add(type_filtered[index]);
         return ret;
      }

      /// <summary>
      /// Returns all direct children with given name.
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
      internal NodeList FilterByTypeAndNameOrIndex(Node.NodeTypes type, string name)
      {
         var type_filtered = this.FilterByNameOrIndexOrType(type);
         var ret = new NodeList();
         ret.AddRange(type_filtered.Where(x => string.Compare(x.Name, name, true) == 0).ToArray());
         return ret;
      }

      /// <summary>
      /// Returns direct children by type.
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      internal NodeList FilterByNameOrIndexOrType(Node.NodeTypes type)
      {
         var ret = new NodeList();
         ret.AddRange(this.Where(x => type == Node.NodeTypes.Any || x.NodeType == type));
         return ret;
      }

      /// <summary>
      /// Returns direct children by index. If no index is given, by name. If no name is given, by type.
      /// </summary>
      /// <param name="name"></param>
      /// <param name="index"></param>
      /// <param name="type"></param>
      /// <returns></returns>
      internal NodeList FilterByTypeAndNameOrIndex(int index, string name, Node.NodeTypes type)
      {
         if (index >= 0)
            return this.FilterByTypeAndNameOrIndex(type, index);
         if (!(string.IsNullOrEmpty(name)))
            return this.FilterByTypeAndNameOrIndex(type, name);
         else if (!(type == Node.NodeTypes.UnknownNode))
            return this.FilterByNameOrIndexOrType(type);
         throw new Exception("Must provide name (!= \"\") or index (>= 0) or given node type.");
      }

      /// <summary>
      /// Returns direct children by index. If no index is given, by name. If no name is given, by type.
      /// </summary>
      /// <param name="sn"></param>
      /// <returns></returns>
      internal NodeList FilterByTypeAndNameOrIndex(SearchNode sn) =>
         this.FilterByTypeAndNameOrIndex(sn.Index, sn.Name, sn.NodeType);

      public NodeList GetNodes(Func<Node, bool> predicate)
      {
         var ret = new NodeList();
         foreach (var n in this)
            if (predicate(n))
               ret.Add(n);
         foreach (var n in this)
         {
            var list = n.Children.GetNodes(predicate);
            if (list.Count > 0)
               foreach (var child in list)
                  ret.Add(child);
         }
         return ret;
      }

      /// <summary>
      /// Recursive search for nodes of specific type.
      /// </summary>
      /// <param name="type"></param>
      /// <returns></returns>
      public NodeList GetByNodesType(Node.NodeTypes type) =>
         this.GetByNodesType(type, true);

      /// <summary>
      /// Search for nodes of specific type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="recursive"></param>
      /// <returns></returns>
      public NodeList GetByNodesType(Node.NodeTypes type, bool recursive)
      {
         var ret = new NodeList();
         ret.AddRange(this.Where(x => type == Node.NodeTypes.Any || x.NodeType == type));
         if (recursive)
            foreach (var n in this)
            {
               var list = n.Children.GetByNodesType(type, true);
               if (list.Count > 0)
                  foreach(var child in list)
                     ret.Add(child);
            }
         return ret;
      }

      /// <summary>
      /// Recursive search for nodes of specific type, with custom predicate.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="predicate"></param>
      /// <returns></returns>
      public NodeList GetNodesByType(Node.NodeTypes type, Func<Node, bool> predicate)
      {
         var ret = new NodeList();
         ret.AddRange(this.Where(x => (type == Node.NodeTypes.Any || x.NodeType == type) && predicate(x)));
         foreach (var n in this)
         {
            var list = n.Children.GetNodesByType(type, predicate);
            if (list.Count > 0)
               foreach (var child in list)
                  ret.Add(child);
         }
         return ret;
      }

      public Node GetNodeByType(Node.NodeTypes type, int index)
      {
         if (type == Node.NodeTypes.Any)
            return this.Count > index ? this[index] : null;
         else
         {
            var ret = (from n in this where n.NodeType == type select n).ToList();
            return ret.Count > index ? ret[index] : null;
         }
      }

      public Node GetNodeByType(Node.NodeTypes type, string name) => (from n in this
                 where (type == Node.NodeTypes.Any || n.NodeType == type) &&
                 string.Compare(n.Name, name) == 0
                 select n).FirstOrDefault();

      public Node GetNodeByType(SearchNode search_node)
      {
         if (search_node.Index >= 0)
            return this.GetNodeByType(search_node.NodeType, search_node.Index);
         else if (!(string.IsNullOrEmpty(search_node.Name)))
            return this.GetNodeByType(search_node.NodeType, search_node.Name);
         else if (!(search_node.NodeType == Node.NodeTypes.UnknownNode))
            return this.GetByNodesType(search_node.NodeType, false).FirstOrDefault();
         throw new Exception("Confusion!!!");
      }

      public Node GetChild(string search_expression) =>
         //Använd första uttrycket för att hitta rätt i den egna samlingen,
         //skicka eventuell rest till den första träffen för vidare sökning.
         this.GetChild(new SearchExpressionParser(search_expression).Parse().ToArray());

      public Node GetChild(params SearchNode[] parsed_search_expression)
      {
         if (parsed_search_expression.Length == 0)
            return null;
         else if (parsed_search_expression.Length == 1)
            return this.GetNodeByType(parsed_search_expression[0]);
         else
         {
            var rest_parts = new SearchNodeList();
            rest_parts.AddRange(parsed_search_expression);
            var first_part = rest_parts[0];
            rest_parts.RemoveAt(0);
            var first_result = this.GetNodeByType(first_part);
            if (first_result == null)
               return null;
            return first_result.GetChild(rest_parts.ToArray());
         }
      }

      public override string ToString()
      {
         var s = new StringBuilder();
         this.ForEach(x => s.Append(x.Source));
         return s.ToString();
      }

      public bool ParentExists(Node n)
      {
         if (n == null)
            return false;
         do
         {
            if (this.Exists(x => x == n))
               return true;
            n = n.Parent;
         } while (!(n == null));
         return false;
      }

      public Node GetNextSibling(Node node)
      {
         var index = this.IndexOf(node);
         if (index >= 0 && index < (this.Count - 1))
            return this[index + 1];
         return null;
      }

      public Node GetPreviousSibling(Node node)
      {
         var index = this.IndexOf(node);
         if (index >= 1 && index < this.Count)
            return this[index - 1];
         return null;
      }

      /// <summary>
      /// Returns the first representation of a matching node at level in the code tree.
      /// </summary>
      /// <param name="search_expression"></param>
      /// <returns></returns>
      public NodeList DeepSearch(string search_expression)
      {
         var separser = new SearchExpressionParser(search_expression);
         var parts = separser.Parse();
         return this.DeepSearch(parts.ToArray());
      }

      /// <summary>
      /// Returns the first representation of a matching node at level in the code tree.
      /// </summary>
      /// <param name="parsed_search_expression"></param>
      /// <returns></returns>
      public NodeList DeepSearch(params SearchNode[] parsed_search_expression)
      {
         var ret = new NodeList();
         var svar = this.GetChild(parsed_search_expression);
         if (!(svar == null))
            ret.Add(svar);
         foreach (var child in this)
         {
            var child_svar = child.GetChild(parsed_search_expression);
            if (!(child_svar == null))
               ret.Add(child_svar);
         }
         this.ForEach(x => this.DeepSearchChild(parsed_search_expression, ref ret, x));
         return ret;
      }

      private void DeepSearchChild(SearchNode[] parsed_search_expression, ref NodeList result, Node n)
      {
         foreach (var child in n.Children)
         {
            var child_svar = child.GetChild(parsed_search_expression);
            if (!(child_svar == null))
               result.Add(child_svar);
         }
         foreach (var child in n.Children)
            this.DeepSearchChild(parsed_search_expression, ref result, child);
      }
   }
}
