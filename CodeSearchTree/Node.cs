using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.ComponentModel;
using CodeSearchTree.SearchExpressionTranslationLists;

namespace CodeSearchTree
{
    public class Node : ITypedSearch, ITypedChild, INode
    {
        [Category("Meta")]
        public bool EntityIsNode =>
            false;
        
        [Category("Meta")]
        public bool EntityIsNodeList =>
            true;
        
        [Category("Meta")]
        public bool HasChildren =>
            ChildCount > 0;

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
        public TypedSearchNode Field => new TypedSearchNode(NodeType.FieldDeclarationSyntaxNode, this);
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
        public TypedSearchNode VarDeclarator => new TypedSearchNode(NodeType.VariableDeclaratorSyntaxNode, this);
        [Browsable(false)]
        public TypedSearchNode VarDeclaration => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, this);
        //End typed search.
        [Category("Meta"), Description("Enumeration of Roslyn types.")]
        public NodeType NodeType { get; internal set; }
        [Category("Relatives"), Description("List of child nodes.")]
        public NodeList Children { get; } = new NodeList();
        [Category("Main"), Description("Return type name")]
        public string ReturnTypeName { get; internal set; } = "";
        [Category("Main"), Description("Original source code.")]
        public string Source { get; internal set; }
        [Category("Main"), Description("Start character position.")]
        public int StartPosition { get; internal set; }
        [Category("Main"), Description("End character position.")]
        public int EndPosition { get; internal set; }
        [Category("Meta"), Description("List of leading trivia.")]
        public TriviaList LeadingTrivia { get; } = new TriviaList();
        [Category("Meta"), Description("List of trailing trivia.")]
        public TriviaList TrailingTrivia { get; } = new TriviaList();
        [Category("Meta"), Description("Name or identifier of node.")]
        public string Name { get; private set; }
        [Category("Relatives"), Description("Reference to parent node, if available.")]
        public Node Parent { get; }
        internal NodeList ParentListIfNoParent { get; set; }
        [Category("Meta"), Description("List of attributes.")]
        public List<string> Attributes { get; } = new List<string>();
        public string Operator { get; private set; } = "";
        public OperatorType OperatorType { get; private set; } = OperatorType.None;
        public object RoslynNode { get; }
        protected internal Node(object roslynNode, string source) : this(roslynNode, source, null, NodeType.NamespaceDeclarationSyntaxNode)
        {
        }
        protected internal Node(object roslynNode, string source, Node parent, NodeType nodeType)
        {
            RoslynNode = roslynNode;
            StartPosition = ((SyntaxNode) roslynNode).FullSpan.Start;
            EndPosition = ((SyntaxNode) roslynNode).FullSpan.End;
            NodeType = nodeType;
            Parent = parent;
            Source = source;
        }
        [Category("Main"), Description("Original source length in characters.")]
        public int Length => Source.Length;
        [Category("Relatives"), Description("Number of child nodes.")]
        public int ChildCount => Children.Count;
        [Category("Relatives"), Description("Type of parant node, if available.")]
        public NodeType ParentType => Parent?.NodeType ?? NodeType.UnknownNode;
        [Category("Meta"), Description("String representation of node type.")]
        public string NodeTypeString => SearchExpressionParser.NodeTypeToKeyword(NodeType);
        [Category("Meta"), Description("Strign representing list of attributes.")]
        public string AttributesString
        {
            get
            {
                if (Attributes.Count == 0)
                    return "";
                var s = new StringBuilder();
                Attributes.ForEach(x => s.Append($"{x}{(x == Attributes.Last() ? "" : ", ")}"));
                return s.ToString();
            }
        }
        [Category("Meta"), Description("String representation of leading trivia.")]
        public string LeadingTriviaString
        {
            get
            {
                var s = new StringBuilder();
                LeadingTrivia.ForEach(x => s.Append(x.Source));
                return s.ToString();
            }
        }
        [Category("Meta"), Description("String representation of trailing trivia.")]
        public string TrailingTriviaString
        {
            get
            {
                var s = new StringBuilder();
                TrailingTrivia.ForEach(x => s.Append(x.Source));
                return s.ToString();
            }
        }
        [Category("Roslyn"), Description("Properties of the underlying Roslyn SyntaxNode.")]
        public List<Property> RoslynNodeProperties
        {
            get
            {
                var ret = new List<Property>();
                var n = RoslynNode as SyntaxNode;
                if (n == null)
                    return null;
                var properties = n.GetType().GetProperties();
                properties.ToList().ForEach(x => ret.Add(new Property(x.Name, x.GetValue(n))));
                return ret;
            }
        }
        [Category("Roslyn"), Description("String representation of the properties of the underlying Roslyn SyntaxNode.")]
        public string RoslynNodePropertiesString
        {
            get
            {
                var s = new StringBuilder();
                RoslynNodeProperties.ForEach(x => s.AppendLine(x.ToString()));
                return s.ToString();
            }
        }
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
                var codeNode = new Node(n, n.ToString(), null, GetNodeType(n)) {ParentListIfNoParent = ret};
                StoreTrivia(codeNode, n);
                ret.Add(codeNode);
                CreateChildren(codeNode, n);
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
        ///     Checks if this node confirms to a search node.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsMatch(SearchNode n)
        {
            if (n.NodeType != NodeType && n.NodeType != NodeType.Any)
                return false;
            //Check for index.
            if (n.Index >= 0 && TypeFilteredIndex == n.Index)
                return true;
            if (n.Index >= 0)
                return false;
            //Check for attribute name.
            if (!(string.IsNullOrEmpty(n.AttributeName)) && Attributes.Contains(n.AttributeName))
                return true;
            if (!(string.IsNullOrEmpty(n.Name)) && string.CompareOrdinal(Name, n.Name) == 0)
                return true;
            if (!(string.IsNullOrEmpty(n.ReturnType)) && string.CompareOrdinal(ReturnTypeName, n.ReturnType) == 0)
                return true;
            return n.Index < 0
                && string.IsNullOrEmpty(n.AttributeName)
                && string.IsNullOrEmpty(n.Name)
                && string.IsNullOrEmpty(n.ReturnType);
        }
        /// <summary>
        /// Calculates the index of this node in it's containing collection, when the collection is filtered on the same node type as this node.
        /// </summary>
        public int TypeFilteredIndex
        {
            get
            {
                var parent = Parent == null ? ParentListIfNoParent : Parent.Children;
                var subset = parent.Filter(SearchNode.CreateSearchByType(NodeType));
                return subset.IndexOf(this);
            }
        }
        /// <summary>
        /// Calculates the index of this node in it's containing collection.
        /// </summary>
        public int ActualIndex => (Parent == null ? ParentListIfNoParent : Parent.Children).IndexOf(this);
        internal void CheckName()
        {
            var n = RoslynNode as SyntaxNode;
            if (n != null)
            {
                if (n is ClassDeclarationSyntax)
                    Name = (n as ClassDeclarationSyntax).Identifier.ToString();
                else if (n is NamespaceDeclarationSyntax)
                    Name = (n as NamespaceDeclarationSyntax).Name.ToString();
                else if (n is FieldDeclarationSyntax)
                {
                    var field = (n as FieldDeclarationSyntax);
                    var v = field.ChildNodes().FirstOrDefault(x => x is VariableDeclarationSyntax);
                    var vd = v?.ChildNodes().FirstOrDefault(x => x is VariableDeclaratorSyntax);
                    if (vd != null)
                        Name = (vd as VariableDeclaratorSyntax)?.Identifier.ToString();
                }
                else if (n is PropertyDeclarationSyntax)
                    Name = (n as PropertyDeclarationSyntax).Identifier.ToString();
                else if (n is VariableDeclarationSyntax)
                {
                    var vars = (n as VariableDeclarationSyntax).Variables;
                    if (vars.Count > 0)
                        Name = vars.First().Identifier.ToString();
                }
                else if (n is IdentifierNameSyntax)
                    Name = n.ToString();
                else if (n is MethodDeclarationSyntax)
                    Name = (n as MethodDeclarationSyntax).Identifier.ToString();
                else if (n is UsingDirectiveSyntax)
                {
                    var id = GetChild("name");
                    if (id == null)
                    {
                        id = GetChild("id");
                        Name = (id?.RoslynNode as IdentifierNameSyntax)?.ToString() ?? "";
                    }
                    else
                        Name = (id.RoslynNode as QualifiedNameSyntax)?.ToString();
                }
                else if (n is AttributeSyntax)
                    Name = (n as AttributeSyntax).Name.ToString();
                else if (n is ConstructorDeclarationSyntax)
                    Name = (n as ConstructorDeclarationSyntax).Identifier.ToString();
                else if (n is ParameterSyntax)
                    Name = (n as ParameterSyntax).Identifier.ToString();
            }
            if (string.IsNullOrWhiteSpace(Name))
                Name = "";
            //Check for operator.
            var binaryExpression = RoslynNode as BinaryExpressionSyntax;
            if (binaryExpression?.OperatorToken != null)
            {
                Operator = binaryExpression.OperatorToken.Text;
                OperatorType = OperatorType.Binary;
            }
            var unaryPostfixExpression = RoslynNode as PostfixUnaryExpressionSyntax;
            if (unaryPostfixExpression?.OperatorToken != null)
            {
                Operator = unaryPostfixExpression.OperatorToken.Text;
                OperatorType = OperatorType.UnaryPostfix;
            }
            var unaryPrefixExpression = RoslynNode as PrefixUnaryExpressionSyntax;
            if (unaryPrefixExpression?.OperatorToken != null)
            {
                Operator = unaryPrefixExpression.OperatorToken.Text;
                OperatorType = OperatorType.UnaryPrefix;
            }
            //End check for operator.
            foreach (var child in Children)
                child.CheckName();
        }
        internal void CheckReturnType()
        {
            var n = RoslynNode as SyntaxNode;
            if (n != null)
            {
                if (n is GenericNameSyntax)
                    Parent.ReturnTypeName = (n as GenericNameSyntax).ToString();
                else if (n is PredefinedTypeSyntax)
                    Parent.ReturnTypeName = (n as PredefinedTypeSyntax).ToString();
                else if (n is IdentifierNameSyntax)
                    Parent.ReturnTypeName = (n as IdentifierNameSyntax).ToString();
            }
            foreach (var child in Children)
                child.CheckReturnType();
        }
        internal void CheckAttributes()
        {
            //Checking for attributes only on classes, methods and properties.
            if (NodeType == NodeType.ClassDeclarationSyntaxNode || NodeType == NodeType.MethodDeclarationSyntaxNode || NodeType == NodeType.PropertyDeclarationSyntaxNode)
            {
                SearchNode[] searchExpression = {
                    SearchNode.CreateSearchByType(NodeType.AttributeListSyntaxNode),
                    SearchNode.CreateSearchByType(NodeType.AttributeSyntaxNode)
                };
                var att = GetChild(searchExpression);
                if (att != null)
                {
                    Attributes.Add(att.Name);
                    do
                    {
                        att = att.GetNextSibling();
                        if (att != null && att.NodeType == NodeType.AttributeSyntaxNode)
                            Attributes.Add(att.Name);
                    } while (att != null);
                }
            }
            foreach (var child in Children)
                child.CheckAttributes();
        }

        internal static void CreateChildren(Node node, SyntaxNode roslynNode)
        {
            foreach (var n in roslynNode.ChildNodes())
            {
                var codeNode = new Node(n, n.ToString(), node, GetNodeType(n));
                StoreTrivia(codeNode, n);
                node.Children.Add(codeNode);
                CreateChildren(codeNode, n);
            }
        }
        /// <summary>
        /// Search expression that allocates this node from root.
        /// </summary>
        [Category("Allocation")]
        public string FullPath
        {
            get
            {
                var ret = "";
                //Hämta sökvägen från parent tills dess att parent är null.
                Node currentParent;
                var currentThis = this;
                do
                {
                    //Växla till nästa förälder.
                    currentParent = currentThis.Parent;
                    if (currentParent == null)
                    {
                        //Ingen mer parent. Kolla var vi ligger i root-listan.
                        if (currentThis.ParentListIfNoParent == null)
                            ret =
                                $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}";
                        else
                        {
                            //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                            var index = currentThis.TypeFilteredIndex;
                            //Om index är > 0, presentera det som en [vakt].
                            ret = currentThis.TypeFilteredIndex <= 0
                                ? $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}"
                                : $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{index}]{(ret == "" ? "" : "/")}{ret}";
                        }
                    }
                    else
                    {
                        //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                        var index = currentThis.TypeFilteredIndex;
                        //Om index är > 0, presentera det som en [vakt].
                        ret = index <= 0
                            ? $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}"
                            : $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{index}]{(ret == "" ? "" : "/")}{ret}";
                    }
                    //Växla till nästa child.
                    currentThis = currentParent;
                } while (currentParent != null);
                return ret;
            }
        }
        /// <summary>
        /// Same as FullPath but with name guards where possible.
        /// </summary>
        [Category("Allocation")]
        public string PossibleAlternativePath
        {
            get
            {
                var ret = "";
                //Hämta sökvägen från parent tills dess att parent är null.
                Node currentParent;
                var currentThis = this;
                do
                {
                    //Växla till nästa förälder.
                    currentParent = currentThis.Parent;
                    if (currentParent == null)
                    {
                        //Ingen mer parent. Kolla var vi ligger i root-listan.
                        if (currentThis.Name == "")
                        {
                            if (currentThis.ParentListIfNoParent == null)
                                ret =
                                    $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}";
                            else
                            {
                                //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                                var index = currentThis.TypeFilteredIndex;
                                //Om index är > 0, presentera det som en [vakt].
                                ret = index <= 0
                                    ? $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}"
                                    : $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{index}]{(ret == "" ? "" : "/")}{ret}";
                            }
                        }
                        else
                            ret = $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{currentThis.Name}]{(ret == "" ? "" : "/")}{ret}";
                    }
                    else
                    {
                        if (currentThis.Name == "")
                        {
                            //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                            var index = currentThis.TypeFilteredIndex;
                            //Om index är > 0, presentera det som en [vakt].
                            ret = index <= 0
                                ? $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}{(ret == "" ? "" : "/")}{ret}"
                                : $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{index}]{(ret == "" ? "" : "/")}{ret}";
                        }
                        else
                            ret = $"{SearchExpressionParser.NodeTypeToKeyword(currentThis.NodeType)}[{currentThis.Name}]{(ret == "" ? "" : "/")}{ret}";
                    }
                    //Växla till nästa child.
                    currentThis = currentParent;
                } while (currentParent != null);
                return ret;
            }
        }

        internal static NodeType GetNodeType(SyntaxNode n) =>
            SyntaxNodeToNodeType.Translate(n);

        internal static void StoreTrivia(Node node, SyntaxNode roslynNode)
        {
            foreach (var t in roslynNode.GetLeadingTrivia())
            {
                var s = t.ToString().Trim();
                if (s != "")
                    node.LeadingTrivia.Add(new Trivia(GetTriviaType(t), s));
            }
            foreach (var t in roslynNode.GetTrailingTrivia())
            {
                var s = t.ToString().Trim();
                if (s != "")
                    node.TrailingTrivia.Add(new Trivia(GetTriviaType(t), s));
            }
        }

        /// <summary>
        /// Returns all child nodes of the given nodetypes.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NodeList GetChildren(NodeType[] type) =>
            NodeList.DoGetChildren(type, Children);
        
        /// <summary>
        /// Returns the first child node of any of the given types.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Node GetChild(params NodeType[] type) =>
            GetChildren(type).FirstOrDefault();
        
        /// <summary>
        /// Returns the first child node that matches the given search expression.
        /// </summary>
        /// <param name="searchExpression"></param>
        /// <returns></returns>
        public Node GetChild(string searchExpression) =>
            GetChild(new SearchExpressionParser(searchExpression).Parse().ToArray());

        /// <summary>
        /// Returns the first child node that matches the given search expression.
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public Node GetChild(params SearchNode[] sn) =>
            NodeList.DoGetChild(Children, sn);
        
        public static SearchNodeList ParseSearchExpression(string searchExpression, out bool success)
        {
            success = true;
#if !DEBUG
         try
         {
#endif
            return new SearchExpressionParser(searchExpression).Parse();
#if !DEBUG
         }
         catch
         {
            success = false;
            return null;
         }
#endif
        }

        public static SearchNodeList ParseSearchExpression(string searchExpression) =>
            new SearchExpressionParser(searchExpression).Parse();
        
        public Node GetNextSibling() =>
            Parent?.Children.GetNextSibling(this);
        
        public Node GetPreviousSibling() =>
            Parent?.Children.GetPreviousSibling(this);
        
        public Node GetFirstSibling() =>
            Parent?.Children.FirstOrDefault();
        
        public Node GetLastSibling() =>
            Parent?.Children.LastOrDefault();

        private static TriviaType GetTriviaType(SyntaxTrivia t) =>
            TriviaToTriviaType.Translate(t);

        public override string ToString() =>
            string.IsNullOrEmpty(Name) ? NodeType.ToString() : $"{NodeType}[{Name}]";
        
        /// <summary>
        /// Returns all siblings of the same type.
        /// </summary>
        /// <returns></returns>
        public List<Node> WithSiblings()
        {
            var ret = new List<Node>();
            var x = GetFirstSibling();
            while (x != null)
            {
                if (x.NodeType == NodeType)
                    ret.Add(x);
                x = x.GetNextSibling();
            }
            return ret;
        }
    }
}