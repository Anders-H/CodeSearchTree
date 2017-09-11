using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeSearchTree
{
    public class NodeList : List<Node>, ITypedSearch, ITypedChild, INode
    {
        [Category("Meta")]
        public bool EntityIsNode => false;
        [Category("Meta")]
        public bool EntityIsNodeList => true;
        [Category("Meta")]
        public bool HasChildren => Count > 0;
        //Typed search.
        [Browsable(false)]
        public TypedSearchNode Arg => new TypedSearchNode(NodeType.ArgumentSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode ArgList => new TypedSearchNode(NodeType.ArgumentListSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Assign => new TypedSearchNode(NodeType.AssignmentExpressionSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode BaseList => new TypedSearchNode(NodeType.BaseListSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode BaseType => new TypedSearchNode(NodeType.SimpleBaseTypeSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Block => new TypedSearchNode(NodeType.BlockSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Cls => new TypedSearchNode(NodeType.ClassDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Constructor => new TypedSearchNode(NodeType.ConstructorDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode EqualsValue => new TypedSearchNode(NodeType.EqualsValueClauseSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Expression => new TypedSearchNode(NodeType.ExpressionStatementSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Field => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Id => new TypedSearchNode(NodeType.IdentifierNameSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode If => new TypedSearchNode(NodeType.IfStatementSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Invocation => new TypedSearchNode(NodeType.InvocationExpressionSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Literal => new TypedSearchNode(NodeType.LiteralExpressionSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode MemberAccess => new TypedSearchNode(NodeType.MemberAccessExpressionSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Method => new TypedSearchNode(NodeType.MethodDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode New => new TypedSearchNode(NodeType.ObjectCreationExpressionSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Ns => new TypedSearchNode(NodeType.NamespaceDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Property => new TypedSearchNode(NodeType.PropertyDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode Try => new TypedSearchNode(NodeType.TryStatementSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode UsingDirective => new TypedSearchNode(NodeType.UsingDirectiveSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode VarDeclaration => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode VarDeclarator => new TypedSearchNode(NodeType.VariableDeclaratorSyntaxNode, this);
        //End typed search.

        /// <summary>
        /// Creates a parsed C# tree from a given .cs file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static NodeList CreateTreeFromFile(string filename)
        {
            string code;
            using (var sr = new System.IO.StreamReader(filename, Encoding.UTF8))
                code = sr.ReadToEnd();
            return CreateTreeFromCode(code);
        }

        /// <summary>
        /// Creates a parsed C# tree from a string of C# code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static NodeList CreateTreeFromCode(string code)
        {
            var ret = new NodeList();
            var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
            foreach (var n in tree.ChildNodes())
            {
                var codeNode = new Node(n, n.ToString(), null, Node.GetNodeType(n)) { ParentListIfNoParent = ret };
                Node.StoreTrivia(codeNode, n);
                ret.Add(codeNode);
                Node.CreateChildren(codeNode, n);
            }
            //First iteration: Read out names.
            ret.ForEach(x => x.CheckName());
            //Second iteration: Read out property types and method return types.
            ret.ForEach(x => x.CheckReturnType());
            //Third iteration: Read out attribute names. No support for parameters.
            ret.ForEach(x => x.CheckAttributes());
            return ret;
        }

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
            foreach (var list in this.Select(n => n.Children.GetNodes(predicate)).Where(list => list.Count > 0))
                ret.AddRange(list);
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
            foreach (var list in this.Select(n => n.Children.GetNodesByType(type, predicate)).Where(list => list.Count > 0))
                ret.AddRange(list);
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
        public NodeList DeepSearch(SearchNodeList searchExpression) => DeepSearch(searchExpression.ToArray());

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

        private static void DeepSearchChild(SearchNode[] parsedSearchExpression, ref NodeList result, Node n)
        {
            result.AddRange(n.Children.Select(child => child.GetChild(parsedSearchExpression)).Where(childSvar => childSvar != null));
            foreach (var child in n.Children)
                DeepSearchChild(parsedSearchExpression, ref result, child);
        }

        /// <summary>
        /// Returns all child nodes of the given nodetypes.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NodeList GetChildren(NodeType[] type) => DoGetChildren(type, this);

        /// <summary>
        /// Returns the first child node of any of the given types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Node GetChild(params NodeType[] type) => GetChildren(type).FirstOrDefault();

        /// <summary>
        /// Returns the first child node that matches the given search expression.
        /// </summary>
        /// <param name="searchExpression"></param>
        /// <returns></returns>
        public Node GetChild(string searchExpression) => GetChild(new SearchExpressionParser(searchExpression).Parse().ToArray());

        /// <summary>
        /// Returns the first child node that matches the given search expression.
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public Node GetChild(params SearchNode[] sn) => DoGetChild(this, sn);

        internal static NodeList DoGetChildren(NodeType[] type, NodeList searchList)
        {
            switch (type.Length)
            {
                case 0:
                    return new NodeList();
                case 1:
                    return searchList.Filter(SearchNode.CreateSearchByType(type[0]));
                case 2:
                {
                    var ret = new NodeList();
                    var item = searchList.Filter(SearchNode.CreateSearchByType(type[0])).FirstOrDefault();
                    if (item == null)
                        return ret;
                    ret.AddRange(item.Children.Filter(SearchNode.CreateSearchByType(type[1])));
                    return ret;
                }
                default:
                {
                    var item = searchList.Filter(SearchNode.CreateSearchByType(type[0])).FirstOrDefault();
                    for (var i = 1; i < type.Length - 1; i++)
                    {
                        if (item == null)
                            return new NodeList();
                        item = item.Children.Filter(SearchNode.CreateSearchByType(type[i])).FirstOrDefault();
                    }
                    return item == null ? new NodeList() : item.Children.Filter(SearchNode.CreateSearchByType(type[type.Length - 1]));
                }
            }
        }

        internal static Node DoGetChild(NodeList searchList, params SearchNode[] sn)
        {
            switch (sn.Length)
            {
                case 0:
                    return null;
                case 1:
                    return searchList.Filter(sn[0]).FirstOrDefault();
                case 2:
                {
                    var item = searchList.Filter(sn[0]).FirstOrDefault();
                    return item?.Children.Filter(sn[1]).FirstOrDefault();
                }
                default:
                {
                    var item = searchList.Filter(sn[0]).FirstOrDefault();
                    for (var i = 1; i < sn.Length - 1; i++)
                    {
                        if (item == null)
                            return null;
                        item = item.Children.Filter(sn[i]).FirstOrDefault();
                    }
                    return item?.Children.Filter(sn[sn.Length - 1]).FirstOrDefault();
                }
            }
        }
    }
}
