using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSearchTree
{
    public class NodeList : List<Node>, ITypedSearch, ITypedChild
    {
        //Typed search.
        public TypedSearchNode Assign => new TypedSearchNode(NodeType.AssignmentExpressionSyntaxNode, this);
        public TypedSearchNode Block => new TypedSearchNode(NodeType.BlockSyntaxNode, this);
        public TypedSearchNode Cls => new TypedSearchNode(NodeType.ClassDeclarationSyntaxNode, this);
        public TypedSearchNode Constructor => new TypedSearchNode(NodeType.ConstructorDeclarationSyntaxNode, this);
        public TypedSearchNode EqualsValue => new TypedSearchNode(NodeType.EqualsValueClauseSyntaxNode, this);
        public TypedSearchNode Expression => new TypedSearchNode(NodeType.ExpressionStatementSyntaxNode, this);
        public TypedSearchNode Field => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, this);
        public TypedSearchNode Id => new TypedSearchNode(NodeType.IdentifierNameSyntaxNode, this);
        public TypedSearchNode Invocation => new TypedSearchNode(NodeType.InvocationExpressionSyntaxNode, this);
        public TypedSearchNode Literal => new TypedSearchNode(NodeType.LiteralExpressionSyntaxNode, this);
        public TypedSearchNode MemberAccess => new TypedSearchNode(NodeType.MemberAccessExpressionSyntaxNode, this);
        public TypedSearchNode Ns => new TypedSearchNode(NodeType.NamespaceDeclarationSyntaxNode, this);
        public TypedSearchNode UsingDirective => new TypedSearchNode(NodeType.UsingDirectiveSyntaxNode, this);
        public TypedSearchNode VarDeclaration => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, this);
        public TypedSearchNode VarDeclarator => new TypedSearchNode(NodeType.VariableDeclaratorSyntaxNode, this);
        //End typed search.

        /// <summary>
        ///     Returns subset of nodes that matches given SearchNode.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public NodeList Filter(SearchNode n)
        {
            var ret = new NodeList();
            ForEach(x => { if (x.IsMatch(n)) ret.Add(x); });
            return ret;
        }

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
            if (!recursive)
                return ret;
            foreach (var list in this.Select(n => n.Children.GetByNodesType(type, true)).Where(list => list.Count > 0))
                ret.AddRange(list);
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
            ret.AddRange(this.Select(child => child.GetChild(parsedSearchExpression)).Where(childSvar => childSvar != null));
            ForEach(x => DeepSearchChild(parsedSearchExpression, ref ret, x));
            return ret;
        }

        private void DeepSearchChild(SearchNode[] parsedSearchExpression, ref NodeList result, Node n)
        {
            result.AddRange(n.Children.Select(child => child.GetChild(parsedSearchExpression)).Where(childSvar => childSvar != null));
            foreach (var child in n.Children)
                DeepSearchChild(parsedSearchExpression, ref result, child);
        }
    }
}
