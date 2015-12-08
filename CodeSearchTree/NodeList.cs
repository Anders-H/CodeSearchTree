using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSearchTree
{
    public class NodeList : List<Node>
    {
        /// <summary>
        ///     Returns direct child at given index.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndNameOrIndex(NodeType type, int index)
        {
            var typeFiltered = FilterByNameOrIndexOrType(type);
            var ret = new NodeList();
            if (typeFiltered.Count > index)
                ret.Add(typeFiltered[index]);
            return ret;
        }

        /// <summary>
        ///     Returns all direct children of given type with given name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndNameOrIndex(NodeType type, string name)
        {
            var typeFiltered = FilterByNameOrIndexOrType(type);
            var ret = new NodeList();
            ret.AddRange(typeFiltered.Where(x => string.Compare(x.Name, name, StringComparison.OrdinalIgnoreCase) == 0).ToArray());
            return ret;
        }

        /// <summary>
        ///     Returns all direct children of given type with given attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndAttribute(NodeType type, string attribute)
        {
            var typeFiltered = FilterByNameOrIndexOrType(type);
            var ret = new NodeList();
            ret.AddRange(typeFiltered.Where(x => x.Attributes.Contains(attribute)).ToArray());
            return ret;
        }

        /// <summary>
        ///     Returns all direct children of given type with given return type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndReturnType(NodeType type, string returnType)
        {
            var typeFiltered = FilterByNameOrIndexOrType(type);
            var ret = new NodeList();
            ret.AddRange(typeFiltered.Where(x => string.Compare(x.ReturnTypeName, returnType, StringComparison.OrdinalIgnoreCase) == 0).ToArray());
            return ret;
        }

        /// <summary>
        ///     Returns direct children by type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal NodeList FilterByNameOrIndexOrType(NodeType type)
        {
            var ret = new NodeList();
            ret.AddRange(this.Where(x => type == NodeType.Any || x.NodeType == type));
            return ret;
        }

        /// <summary>
        ///     Returns direct children by index. If no index is given, by name. If no name is given, by type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndNameOrIndex(int index, string name, string attribute, string returnType, NodeType type)
        {
            if (index >= 0)
                return FilterByTypeAndNameOrIndex(type, index);
            if (!string.IsNullOrEmpty(name))
                return FilterByTypeAndNameOrIndex(type, name);
            if (!string.IsNullOrEmpty(attribute))
                return FilterByTypeAndAttribute(type, attribute);
            if (!string.IsNullOrEmpty(returnType))
                return FilterByTypeAndReturnType(type, returnType);
            if (type != NodeType.UnknownNode)
                return FilterByNameOrIndexOrType(type);
            throw new Exception("Must provide name (!= \"\") or index (>= 0) or given node type.");
        }

        /// <summary>
        ///     Returns direct children by index. If no index is given, by name. If no name is given, by type.
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        internal NodeList FilterByTypeAndNameOrIndex(SearchNode sn) =>
            FilterByTypeAndNameOrIndex(sn.Index, sn.Name, sn.AttributeName, sn.ReturnType, sn.NodeType);

        public NodeList GetNodes(Func<Node, bool> predicate)
        {
            var ret = new NodeList();
            ret.AddRange(this.Where(predicate));
            foreach (var n in this)
            {
                var list = n.Children.GetNodes(predicate);
                if (list.Count > 0)
                    ret.AddRange(list);
            }
            return ret;
        }

        /// <summary>
        ///     Recursive search for nodes of specific type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NodeList GetByNodesType(NodeType type) =>
            GetByNodesType(type, true);

        /// <summary>
        ///     Search for nodes of specific type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public NodeList GetByNodesType(NodeType type, bool recursive)
        {
            var ret = new NodeList();
            ret.AddRange(this.Where(x => type == NodeType.Any || x.NodeType == type));
            if (recursive)
                foreach (var n in this)
                {
                    var list = n.Children.GetByNodesType(type, true);
                    if (list.Count > 0)
                        ret.AddRange(list);
                }
            return ret;
        }

        /// <summary>
        ///     Recursive search for nodes of specific type, with custom predicate.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public NodeList GetNodesByType(NodeType type, Func<Node, bool> predicate)
        {
            var ret = new NodeList();
            ret.AddRange(this.Where(x => (type == NodeType.Any || x.NodeType == type) && predicate(x)));
            foreach (var n in this)
            {
                var list = n.Children.GetNodesByType(type, predicate);
                if (list.Count > 0)
                    ret.AddRange(list);
            }
            return ret;
        }

        public Node GetNodeByType(NodeType type, int index)
        {
            if (type == NodeType.Any)
                return Count > index ? this[index] : null;
            var ret = (from n in this where n.NodeType == type select n).ToList();
            return ret.Count > index ? ret[index] : null;
        }

        public Node GetNodeByType(NodeType type, string name) => (from n in this
            where (type == NodeType.Any || n.NodeType == type) &&
                  string.CompareOrdinal(n.Name, name) == 0
            select n).FirstOrDefault();

        public Node GetNodeByType(SearchNode searchNode)
        {
            if (searchNode.Index >= 0)
                return GetNodeByType(searchNode.NodeType, searchNode.Index);
            if (!string.IsNullOrEmpty(searchNode.Name))
                return GetNodeByType(searchNode.NodeType, searchNode.Name);
            if (searchNode.NodeType != NodeType.UnknownNode)
                return GetByNodesType(searchNode.NodeType, false).FirstOrDefault();
            throw new Exception("Confusion!!!");
        }

        public Node GetChild(string searchExpression) =>
            //Använd första uttrycket för att hitta rätt i den egna samlingen,
            //skicka eventuell rest till den första träffen för vidare sökning.
            GetChild(new SearchExpressionParser(searchExpression).Parse().ToArray());

        public Node GetChild(params SearchNode[] parsedSearchExpression)
        {
            if (parsedSearchExpression.Length == 0)
                return null;
            if (parsedSearchExpression.Length == 1)
                return GetNodeByType(parsedSearchExpression[0]);
            var restParts = new SearchNodeList();
            restParts.AddRange(parsedSearchExpression);
            var firstPart = restParts[0];
            restParts.RemoveAt(0);
            var firstResult = GetNodeByType(firstPart);
            return firstResult?.GetChild(restParts.ToArray());
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            ForEach(x => s.Append(x.Source));
            return s.ToString();
        }

        public bool ParentExists(Node n)
        {
            if (n == null)
                return false;
            do
            {
                if (Exists(x => x == n))
                    return true;
                n = n.Parent;
            } while (n != null);
            return false;
        }

        public Node GetNextSibling(Node node)
        {
            var index = IndexOf(node);
            if (index >= 0 && index < Count - 1)
                return this[index + 1];
            return null;
        }

        public Node GetPreviousSibling(Node node)
        {
            var index = IndexOf(node);
            if (index >= 1 && index < Count)
                return this[index - 1];
            return null;
        }

        /// <summary>
        ///     Returns the first representation of a matching node at level in the code tree.
        /// </summary>
        /// <param name="searchExpression"></param>
        /// <returns></returns>
        public NodeList DeepSearch(string searchExpression)
        {
            var separser = new SearchExpressionParser(searchExpression);
            var parts = separser.Parse();
            return DeepSearch(parts.ToArray());
        }

        /// <summary>
        ///     Returns the first representation of a matching node at level in the code tree.
        /// </summary>
        /// <param name="parsedSearchExpression"></param>
        /// <returns></returns>
        public NodeList DeepSearch(params SearchNode[] parsedSearchExpression)
        {
            var ret = new NodeList();
            var svar = GetChild(parsedSearchExpression);
            if (svar != null)
                ret.Add(svar);
            foreach (var child in this)
            {
                var childSvar = child.GetChild(parsedSearchExpression);
                if (childSvar != null)
                    ret.Add(childSvar);
            }
            ForEach(x => DeepSearchChild(parsedSearchExpression, ref ret, x));
            return ret;
        }

        private void DeepSearchChild(SearchNode[] parsedSearchExpression, ref NodeList result, Node n)
        {
            foreach (var child in n.Children)
            {
                var childSvar = child.GetChild(parsedSearchExpression);
                if (childSvar != null)
                    result.Add(childSvar);
            }
            foreach (var child in n.Children)
                DeepSearchChild(parsedSearchExpression, ref result, child);
        }
    }
}