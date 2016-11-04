using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace CodeSearchTree
{
    public class Node : ITypedSearch, ITypedChild, INode
    {
        [Category("Meta")]
        public bool EntityIsNode => false;
        [Category("Meta")]
        public bool EntityIsNodeList => true;
        [Category("Meta")]
        public bool HasChildren => ChildCount > 0;

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
                        Name = vars.First()?.Identifier.ToString() ?? "";
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
        internal static NodeType GetNodeType(SyntaxNode n)
        {
            if (n is UsingDirectiveSyntax)
                return NodeType.UsingDirectiveSyntaxNode;
            if (n is NamespaceDeclarationSyntax)
                return NodeType.NamespaceDeclarationSyntaxNode;
            if (n is ClassDeclarationSyntax)
                return NodeType.ClassDeclarationSyntaxNode;
            if (n is IdentifierNameSyntax)
                return NodeType.IdentifierNameSyntaxNode;
            if (n is QualifiedNameSyntax)
                return NodeType.QualifiedNameSyntaxNode;
            if (n is FieldDeclarationSyntax)
                return NodeType.FieldDeclarationSyntaxNode;
            if (n is VariableDeclarationSyntax)
                return NodeType.VariableDeclarationSyntaxNode;
            if (n is VariableDeclaratorSyntax)
                return NodeType.VariableDeclaratorSyntaxNode;
            if (n is PropertyDeclarationSyntax)
                return NodeType.PropertyDeclarationSyntaxNode;
            if (n is AccessorListSyntax)
                return NodeType.AccessorListSyntaxNode;
            if (n is AccessorDeclarationSyntax)
                return NodeType.AccessorDeclarationSyntaxNode;
            if (n is AttributeListSyntax)
                return NodeType.AttributeListSyntaxNode;
            if (n is AttributeSyntax)
                return NodeType.AttributeSyntaxNode;
            if (n is AttributeArgumentListSyntax)
                return NodeType.AttributeArgumentListSyntaxNode;
            if (n is BlockSyntax)
                return NodeType.BlockSyntaxNode;
            if (n is ReturnStatementSyntax)
                return NodeType.ReturnStatementSyntaxNode;
            if (n is MethodDeclarationSyntax)
                return NodeType.MethodDeclarationSyntaxNode;
            if (n is PredefinedTypeSyntax)
                return NodeType.PredefinedTypeSyntaxNode;
            if (n is ParameterListSyntax)
                return NodeType.ParameterListSyntaxNode;
            if (n is ExpressionStatementSyntax)
                return NodeType.ExpressionStatementSyntaxNode;
            if (n is InvocationExpressionSyntax)
                return NodeType.InvocationExpressionSyntaxNode;
            if (n is ArgumentListSyntax)
                return NodeType.ArgumentListSyntaxNode;
            if (n is AssignmentExpressionSyntax)
                return NodeType.AssignmentExpressionSyntaxNode;
            if (n is MemberAccessExpressionSyntax)
                return NodeType.MemberAccessExpressionSyntaxNode;
            if (n is SwitchStatementSyntax)
                return NodeType.SwitchStatementSyntaxNode;
            if (n is ArgumentSyntax)
                return NodeType.ArgumentSyntaxNode;
            if (n is LiteralExpressionSyntax)
                return NodeType.LiteralExpressionSyntaxNode;
            if (n is IfStatementSyntax)
                return NodeType.IfStatementSyntaxNode;
            if (n is PrefixUnaryExpressionSyntax)
                return NodeType.PrefixUnaryExpressionSyntaxNode;
            if (n is ParenthesizedExpressionSyntax)
                return NodeType.ParenthesizedExpressionSyntaxNode;
            if (n is BinaryExpressionSyntax)
                return NodeType.BinaryExpressionSyntaxNode;
            if (n is ElseClauseSyntax)
                return NodeType.ElseClauseSyntaxNode;
            if (n is WhileStatementSyntax)
                return NodeType.WhileStatementSyntaxNode;
            if (n is BreakStatementSyntax)
                return NodeType.BreakStatementSyntaxNode;
            if (n is UsingStatementSyntax)
                return NodeType.UsingStatementSyntaxNode;
            if (n is ForStatementSyntax)
                return NodeType.ForStatementSyntaxNode;
            if (n is LabeledStatementSyntax)
                return NodeType.LabeledStatementSyntaxNode;
            if (n is BaseListSyntax)
                return NodeType.BaseListSyntaxNode;
            if (n is SimpleBaseTypeSyntax)
                return NodeType.SimpleBaseTypeSyntaxNode;
            if (n is GenericNameSyntax)
                return NodeType.GenericNameSyntaxNode;
            if (n is TypeArgumentListSyntax)
                return NodeType.TypeArgumentListSyntaxNode;
            if (n is ParameterSyntax)
                return NodeType.ParameterSyntaxNode;
            if (n is LocalDeclarationStatementSyntax)
                return NodeType.LocalDeclarationStatementSyntaxNode;
            if (n is EqualsValueClauseSyntax)
                return NodeType.EqualsValueClauseSyntaxNode;
            if (n is ObjectCreationExpressionSyntax)
                return NodeType.ObjectCreationExpressionSyntaxNode;
            if (n is TypeOfExpressionSyntax)
                return NodeType.TypeOfExpressionSyntaxNode;
            if (n is ThrowStatementSyntax)
                return NodeType.ThrowStatementSyntaxNode;
            if (n is ThisExpressionSyntax)
                return NodeType.ThisExpressionSyntaxNode;
            if (n is SimpleLambdaExpressionSyntax)
                return NodeType.SimpleLambdaExpressionSyntaxNode;
            if (n is ForEachStatementSyntax)
                return NodeType.ForEachStatementSyntaxNode;
            if (n is TryStatementSyntax)
                return NodeType.TryStatementSyntaxNode;
            if (n is CatchClauseSyntax)
                return NodeType.CatchClauseSyntaxNode;
            if (n is SwitchSectionSyntax)
                return NodeType.SwitchSectionSyntaxNode;
            if (n is CaseSwitchLabelSyntax)
                return NodeType.CaseSwitchLabelSyntaxNode;
            if (n is DefaultSwitchLabelSyntax)
                return NodeType.DefaultSwitchLabelSyntaxNode;
            if (n is ArrayTypeSyntax)
                return NodeType.ArrayTypeSyntaxNode;
            if (n is ArrayRankSpecifierSyntax)
                return NodeType.ArrayRankSpecifierSyntaxNode;
            if (n is OmittedArraySizeExpressionSyntax)
                return NodeType.OmittedArraySizeExpressionSyntaxNode;
            if (n is ElementAccessExpressionSyntax)
                return NodeType.ElementAccessExpressionSyntaxNode;
            if (n is BracketedArgumentListSyntax)
                return NodeType.BracketedArgumentListSyntaxNode;
            if (n is ConditionalExpressionSyntax)
                return NodeType.ConditionalExpressionSyntaxNode;
            if (n is PostfixUnaryExpressionSyntax)
                return NodeType.PostfixUnaryExpressionSyntaxNode;
            if (n is ContinueStatementSyntax)
                return NodeType.ContinueStatementSyntaxNode;
            if (n is ConstructorDeclarationSyntax)
                return NodeType.ConstructorDeclarationSyntaxNode;
            if (n is QueryExpressionSyntax)
                return NodeType.QueryExpressionSyntaxNode;
            if (n is FromClauseSyntax)
                return NodeType.FromClauseSyntaxNode;
            if (n is QueryBodySyntax)
                return NodeType.QueryBodySyntaxNode;
            if (n is WhereClauseSyntax)
                return NodeType.WhereClauseSyntaxNode;
            if (n is SelectClauseSyntax)
                return NodeType.SelectClauseSyntaxNode;
            if (n is DoStatementSyntax)
                return NodeType.DoStatementSyntaxNode;
            if (n is NameEqualsSyntax)
                return NodeType.NameEqualsSyntaxNode;
            if (n is EnumDeclarationSyntax)
                return NodeType.EnumDeclarationSyntaxNode;
            if (n is EnumMemberDeclarationSyntax)
                return NodeType.EnumMemberDeclarationSyntaxNode;
            if (n is AttributeArgumentSyntax)
                return NodeType.AttributeArgumentSyntaxNode;
            if (n is ConstructorInitializerSyntax)
                return NodeType.ConstructorInitializerSyntaxNode;
            if (n is EmptyStatementSyntax)
                return NodeType.EmptyStatementSyntaxNode;
            if (n is InitializerExpressionSyntax)
                return NodeType.InitializerExpressionSyntaxNode;
            if (n is AwaitExpressionSyntax)
                return NodeType.AwaitExpressionSyntaxNode;
            if (n is AnonymousObjectCreationExpressionSyntax)
                return NodeType.AnonymousObjectCreationExpressionSyntaxNode;
            if (n is AnonymousObjectMemberDeclaratorSyntax)
                return NodeType.AnonymousObjectMemberDeclaratorSyntaxNode;
            if (n is TypeParameterListSyntax)
                return NodeType.TypeParameterListSyntaxNode;
            if (n is TypeParameterSyntax)
                return NodeType.TypeParameterSyntaxNode;
            if (n is DefaultExpressionSyntax)
                return NodeType.DefaultExpressionSyntaxNode;
            if (n is InterfaceDeclarationSyntax)
                return NodeType.InterfaceDeclarationSyntaxNode;
            if (n is CastExpressionSyntax)
                return NodeType.CastExpressionSyntaxNode;
            if (n is BaseExpressionSyntax)
                return NodeType.BaseExpressionSyntaxNode;
            if (n is AttributeTargetSpecifierSyntax)
                return NodeType.AttributeTargetSpecifierSyntaxNode;
            if (n is AliasQualifiedNameSyntax)
                return NodeType.AliasQualifiedNameSyntaxNode;
            if (n is ExplicitInterfaceSpecifierSyntax)
                return NodeType.ExplicitInterfaceSpecifierSyntaxNode;
            if (n is CatchDeclarationSyntax)
                return NodeType.CatchDeclarationSyntaxNode;
            if (n is ArrowExpressionClauseSyntax)
                return NodeType.ArrowExpressionClauseSyntaxNode;
            if (n is ConditionalAccessExpressionSyntax)
                return NodeType.ConditionalAccessExpressionSyntaxNode;
            if (n is MemberBindingExpressionSyntax)
                return NodeType.MemberBindingExpressionSyntaxNode;
            if (n is InterpolatedStringExpressionSyntax)
                return NodeType.InterpolatedStringExpressionSyntaxNode;
            if (n is InterpolationSyntax)
                return NodeType.InterpolationSyntaxNode;
            if (n is InterpolatedStringTextSyntax)
                return NodeType.InterpolatedStringTextSyntaxNode;
            if (n is GotoStatementSyntax)
                return NodeType.GotoStatementSyntaxNode;
            if (n is LockStatementSyntax)
                return NodeType.LockStatementSyntaxNode;
            if (n is ArrayCreationExpressionSyntax)
                return NodeType.ArrayCreationExpressionSyntaxNode;
            if (n is FinallyClauseSyntax)
                return NodeType.FinallyClauseSyntaxNode;
            return NodeType.UnknownNode;
        }

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
        public NodeList GetChildren(NodeType[] type) => NodeList.DoGetChildren(type, Children);
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
        public Node GetChild(params SearchNode[] sn) => NodeList.DoGetChild(Children, sn);
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
        public static SearchNodeList ParseSearchExpression(string searchExpression) => new SearchExpressionParser(searchExpression).Parse();
        public Node GetNextSibling() => Parent?.Children.GetNextSibling(this);
        public Node GetPreviousSibling() => Parent?.Children.GetPreviousSibling(this);
        public Node GetFirstSibling() => Parent?.Children.FirstOrDefault();
        public Node GetLastSibling() => Parent?.Children.LastOrDefault();
        private static Trivia.TriviaTypes GetTriviaType(SyntaxTrivia t)
        {
            switch (t.Kind())
            {
                case SyntaxKind.RegionDirectiveTrivia:
                    return Trivia.TriviaTypes.RegionDirectiveTriviaSyntaxType;
                case SyntaxKind.SingleLineCommentTrivia:
                    return Trivia.TriviaTypes.SingleLineCommentTriviaType;
                case SyntaxKind.EndRegionDirectiveTrivia:
                    return Trivia.TriviaTypes.EndRegionDirectiveTriviaType;
                case SyntaxKind.MultiLineCommentTrivia:
                    return Trivia.TriviaTypes.MultiLineCommentTriviaType;
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                    return Trivia.TriviaTypes.SingleLineDocumentationCommentTriviaType;
                case SyntaxKind.IfDirectiveTrivia:
                    return Trivia.TriviaTypes.IfDirectiveTriviaType;
                case SyntaxKind.DisabledTextTrivia:
                    return Trivia.TriviaTypes.DisabledTextTriviaType;
                case SyntaxKind.ElseDirectiveTrivia:
                    return Trivia.TriviaTypes.ElseDirectiveTriviaType;
                case SyntaxKind.PragmaChecksumDirectiveTrivia:
                    return Trivia.TriviaTypes.PragmaChecksumDirectiveTriviaType;
                case SyntaxKind.LineDirectiveTrivia:
                    return Trivia.TriviaTypes.LineDirectiveTriviaType;
                case SyntaxKind.EndIfDirectiveTrivia:
                    return Trivia.TriviaTypes.EndIfDirectiveTriviaType;
                case SyntaxKind.None:
                    break;
                case SyntaxKind.List:
                    break;
                case SyntaxKind.TildeToken:
                    break;
                case SyntaxKind.ExclamationToken:
                    break;
                case SyntaxKind.DollarToken:
                    break;
                case SyntaxKind.PercentToken:
                    break;
                case SyntaxKind.CaretToken:
                    break;
                case SyntaxKind.AmpersandToken:
                    break;
                case SyntaxKind.AsteriskToken:
                    break;
                case SyntaxKind.OpenParenToken:
                    break;
                case SyntaxKind.CloseParenToken:
                    break;
                case SyntaxKind.MinusToken:
                    break;
                case SyntaxKind.PlusToken:
                    break;
                case SyntaxKind.EqualsToken:
                    break;
                case SyntaxKind.OpenBraceToken:
                    break;
                case SyntaxKind.CloseBraceToken:
                    break;
                case SyntaxKind.OpenBracketToken:
                    break;
                case SyntaxKind.CloseBracketToken:
                    break;
                case SyntaxKind.BarToken:
                    break;
                case SyntaxKind.BackslashToken:
                    break;
                case SyntaxKind.ColonToken:
                    break;
                case SyntaxKind.SemicolonToken:
                    break;
                case SyntaxKind.DoubleQuoteToken:
                    break;
                case SyntaxKind.SingleQuoteToken:
                    break;
                case SyntaxKind.LessThanToken:
                    break;
                case SyntaxKind.CommaToken:
                    break;
                case SyntaxKind.GreaterThanToken:
                    break;
                case SyntaxKind.DotToken:
                    break;
                case SyntaxKind.QuestionToken:
                    break;
                case SyntaxKind.HashToken:
                    break;
                case SyntaxKind.SlashToken:
                    break;
                case SyntaxKind.SlashGreaterThanToken:
                    break;
                case SyntaxKind.LessThanSlashToken:
                    break;
                case SyntaxKind.XmlCommentStartToken:
                    break;
                case SyntaxKind.XmlCommentEndToken:
                    break;
                case SyntaxKind.XmlCDataStartToken:
                    break;
                case SyntaxKind.XmlCDataEndToken:
                    break;
                case SyntaxKind.XmlProcessingInstructionStartToken:
                    break;
                case SyntaxKind.XmlProcessingInstructionEndToken:
                    break;
                case SyntaxKind.BarBarToken:
                    break;
                case SyntaxKind.AmpersandAmpersandToken:
                    break;
                case SyntaxKind.MinusMinusToken:
                    break;
                case SyntaxKind.PlusPlusToken:
                    break;
                case SyntaxKind.ColonColonToken:
                    break;
                case SyntaxKind.QuestionQuestionToken:
                    break;
                case SyntaxKind.MinusGreaterThanToken:
                    break;
                case SyntaxKind.ExclamationEqualsToken:
                    break;
                case SyntaxKind.EqualsEqualsToken:
                    break;
                case SyntaxKind.EqualsGreaterThanToken:
                    break;
                case SyntaxKind.LessThanEqualsToken:
                    break;
                case SyntaxKind.LessThanLessThanToken:
                    break;
                case SyntaxKind.LessThanLessThanEqualsToken:
                    break;
                case SyntaxKind.GreaterThanEqualsToken:
                    break;
                case SyntaxKind.GreaterThanGreaterThanToken:
                    break;
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                    break;
                case SyntaxKind.SlashEqualsToken:
                    break;
                case SyntaxKind.AsteriskEqualsToken:
                    break;
                case SyntaxKind.BarEqualsToken:
                    break;
                case SyntaxKind.AmpersandEqualsToken:
                    break;
                case SyntaxKind.PlusEqualsToken:
                    break;
                case SyntaxKind.MinusEqualsToken:
                    break;
                case SyntaxKind.CaretEqualsToken:
                    break;
                case SyntaxKind.PercentEqualsToken:
                    break;
                case SyntaxKind.BoolKeyword:
                    break;
                case SyntaxKind.ByteKeyword:
                    break;
                case SyntaxKind.SByteKeyword:
                    break;
                case SyntaxKind.ShortKeyword:
                    break;
                case SyntaxKind.UShortKeyword:
                    break;
                case SyntaxKind.IntKeyword:
                    break;
                case SyntaxKind.UIntKeyword:
                    break;
                case SyntaxKind.LongKeyword:
                    break;
                case SyntaxKind.ULongKeyword:
                    break;
                case SyntaxKind.DoubleKeyword:
                    break;
                case SyntaxKind.FloatKeyword:
                    break;
                case SyntaxKind.DecimalKeyword:
                    break;
                case SyntaxKind.StringKeyword:
                    break;
                case SyntaxKind.CharKeyword:
                    break;
                case SyntaxKind.VoidKeyword:
                    break;
                case SyntaxKind.ObjectKeyword:
                    break;
                case SyntaxKind.TypeOfKeyword:
                    break;
                case SyntaxKind.SizeOfKeyword:
                    break;
                case SyntaxKind.NullKeyword:
                    break;
                case SyntaxKind.TrueKeyword:
                    break;
                case SyntaxKind.FalseKeyword:
                    break;
                case SyntaxKind.IfKeyword:
                    break;
                case SyntaxKind.ElseKeyword:
                    break;
                case SyntaxKind.WhileKeyword:
                    break;
                case SyntaxKind.ForKeyword:
                    break;
                case SyntaxKind.ForEachKeyword:
                    break;
                case SyntaxKind.DoKeyword:
                    break;
                case SyntaxKind.SwitchKeyword:
                    break;
                case SyntaxKind.CaseKeyword:
                    break;
                case SyntaxKind.DefaultKeyword:
                    break;
                case SyntaxKind.TryKeyword:
                    break;
                case SyntaxKind.CatchKeyword:
                    break;
                case SyntaxKind.FinallyKeyword:
                    break;
                case SyntaxKind.LockKeyword:
                    break;
                case SyntaxKind.GotoKeyword:
                    break;
                case SyntaxKind.BreakKeyword:
                    break;
                case SyntaxKind.ContinueKeyword:
                    break;
                case SyntaxKind.ReturnKeyword:
                    break;
                case SyntaxKind.ThrowKeyword:
                    break;
                case SyntaxKind.PublicKeyword:
                    break;
                case SyntaxKind.PrivateKeyword:
                    break;
                case SyntaxKind.InternalKeyword:
                    break;
                case SyntaxKind.ProtectedKeyword:
                    break;
                case SyntaxKind.StaticKeyword:
                    break;
                case SyntaxKind.ReadOnlyKeyword:
                    break;
                case SyntaxKind.SealedKeyword:
                    break;
                case SyntaxKind.ConstKeyword:
                    break;
                case SyntaxKind.FixedKeyword:
                    break;
                case SyntaxKind.StackAllocKeyword:
                    break;
                case SyntaxKind.VolatileKeyword:
                    break;
                case SyntaxKind.NewKeyword:
                    break;
                case SyntaxKind.OverrideKeyword:
                    break;
                case SyntaxKind.AbstractKeyword:
                    break;
                case SyntaxKind.VirtualKeyword:
                    break;
                case SyntaxKind.EventKeyword:
                    break;
                case SyntaxKind.ExternKeyword:
                    break;
                case SyntaxKind.RefKeyword:
                    break;
                case SyntaxKind.OutKeyword:
                    break;
                case SyntaxKind.InKeyword:
                    break;
                case SyntaxKind.IsKeyword:
                    break;
                case SyntaxKind.AsKeyword:
                    break;
                case SyntaxKind.ParamsKeyword:
                    break;
                case SyntaxKind.ArgListKeyword:
                    break;
                case SyntaxKind.MakeRefKeyword:
                    break;
                case SyntaxKind.RefTypeKeyword:
                    break;
                case SyntaxKind.RefValueKeyword:
                    break;
                case SyntaxKind.ThisKeyword:
                    break;
                case SyntaxKind.BaseKeyword:
                    break;
                case SyntaxKind.NamespaceKeyword:
                    break;
                case SyntaxKind.UsingKeyword:
                    break;
                case SyntaxKind.ClassKeyword:
                    break;
                case SyntaxKind.StructKeyword:
                    break;
                case SyntaxKind.InterfaceKeyword:
                    break;
                case SyntaxKind.EnumKeyword:
                    break;
                case SyntaxKind.DelegateKeyword:
                    break;
                case SyntaxKind.CheckedKeyword:
                    break;
                case SyntaxKind.UncheckedKeyword:
                    break;
                case SyntaxKind.UnsafeKeyword:
                    break;
                case SyntaxKind.OperatorKeyword:
                    break;
                case SyntaxKind.ExplicitKeyword:
                    break;
                case SyntaxKind.ImplicitKeyword:
                    break;
                case SyntaxKind.YieldKeyword:
                    break;
                case SyntaxKind.PartialKeyword:
                    break;
                case SyntaxKind.AliasKeyword:
                    break;
                case SyntaxKind.GlobalKeyword:
                    break;
                case SyntaxKind.AssemblyKeyword:
                    break;
                case SyntaxKind.ModuleKeyword:
                    break;
                case SyntaxKind.TypeKeyword:
                    break;
                case SyntaxKind.FieldKeyword:
                    break;
                case SyntaxKind.MethodKeyword:
                    break;
                case SyntaxKind.ParamKeyword:
                    break;
                case SyntaxKind.PropertyKeyword:
                    break;
                case SyntaxKind.TypeVarKeyword:
                    break;
                case SyntaxKind.GetKeyword:
                    break;
                case SyntaxKind.SetKeyword:
                    break;
                case SyntaxKind.AddKeyword:
                    break;
                case SyntaxKind.RemoveKeyword:
                    break;
                case SyntaxKind.WhereKeyword:
                    break;
                case SyntaxKind.FromKeyword:
                    break;
                case SyntaxKind.GroupKeyword:
                    break;
                case SyntaxKind.JoinKeyword:
                    break;
                case SyntaxKind.IntoKeyword:
                    break;
                case SyntaxKind.LetKeyword:
                    break;
                case SyntaxKind.ByKeyword:
                    break;
                case SyntaxKind.SelectKeyword:
                    break;
                case SyntaxKind.OrderByKeyword:
                    break;
                case SyntaxKind.OnKeyword:
                    break;
                case SyntaxKind.EqualsKeyword:
                    break;
                case SyntaxKind.AscendingKeyword:
                    break;
                case SyntaxKind.DescendingKeyword:
                    break;
                case SyntaxKind.NameOfKeyword:
                    break;
                case SyntaxKind.AsyncKeyword:
                    break;
                case SyntaxKind.AwaitKeyword:
                    break;
                case SyntaxKind.WhenKeyword:
                    break;
                case SyntaxKind.ElifKeyword:
                    break;
                case SyntaxKind.EndIfKeyword:
                    break;
                case SyntaxKind.RegionKeyword:
                    break;
                case SyntaxKind.EndRegionKeyword:
                    break;
                case SyntaxKind.DefineKeyword:
                    break;
                case SyntaxKind.UndefKeyword:
                    break;
                case SyntaxKind.WarningKeyword:
                    break;
                case SyntaxKind.ErrorKeyword:
                    break;
                case SyntaxKind.LineKeyword:
                    break;
                case SyntaxKind.PragmaKeyword:
                    break;
                case SyntaxKind.HiddenKeyword:
                    break;
                case SyntaxKind.ChecksumKeyword:
                    break;
                case SyntaxKind.DisableKeyword:
                    break;
                case SyntaxKind.RestoreKeyword:
                    break;
                case SyntaxKind.ReferenceKeyword:
                    break;
                case SyntaxKind.LoadKeyword:
                    break;
                case SyntaxKind.InterpolatedStringStartToken:
                    break;
                case SyntaxKind.InterpolatedStringEndToken:
                    break;
                case SyntaxKind.InterpolatedVerbatimStringStartToken:
                    break;
                case SyntaxKind.OmittedTypeArgumentToken:
                    break;
                case SyntaxKind.OmittedArraySizeExpressionToken:
                    break;
                case SyntaxKind.EndOfDirectiveToken:
                    break;
                case SyntaxKind.EndOfDocumentationCommentToken:
                    break;
                case SyntaxKind.EndOfFileToken:
                    break;
                case SyntaxKind.BadToken:
                    break;
                case SyntaxKind.IdentifierToken:
                    break;
                case SyntaxKind.NumericLiteralToken:
                    break;
                case SyntaxKind.CharacterLiteralToken:
                    break;
                case SyntaxKind.StringLiteralToken:
                    break;
                case SyntaxKind.XmlEntityLiteralToken:
                    break;
                case SyntaxKind.XmlTextLiteralToken:
                    break;
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    break;
                case SyntaxKind.InterpolatedStringToken:
                    break;
                case SyntaxKind.InterpolatedStringTextToken:
                    break;
                case SyntaxKind.EndOfLineTrivia:
                    break;
                case SyntaxKind.WhitespaceTrivia:
                    break;
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    break;
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    break;
                case SyntaxKind.PreprocessingMessageTrivia:
                    break;
                case SyntaxKind.ElifDirectiveTrivia:
                    break;
                case SyntaxKind.DefineDirectiveTrivia:
                    break;
                case SyntaxKind.UndefDirectiveTrivia:
                    break;
                case SyntaxKind.ErrorDirectiveTrivia:
                    break;
                case SyntaxKind.WarningDirectiveTrivia:
                    break;
                case SyntaxKind.PragmaWarningDirectiveTrivia:
                    break;
                case SyntaxKind.ReferenceDirectiveTrivia:
                    break;
                case SyntaxKind.BadDirectiveTrivia:
                    break;
                case SyntaxKind.SkippedTokensTrivia:
                    break;
                case SyntaxKind.XmlElement:
                    break;
                case SyntaxKind.XmlElementStartTag:
                    break;
                case SyntaxKind.XmlElementEndTag:
                    break;
                case SyntaxKind.XmlEmptyElement:
                    break;
                case SyntaxKind.XmlTextAttribute:
                    break;
                case SyntaxKind.XmlCrefAttribute:
                    break;
                case SyntaxKind.XmlNameAttribute:
                    break;
                case SyntaxKind.XmlName:
                    break;
                case SyntaxKind.XmlPrefix:
                    break;
                case SyntaxKind.XmlText:
                    break;
                case SyntaxKind.XmlCDataSection:
                    break;
                case SyntaxKind.XmlComment:
                    break;
                case SyntaxKind.XmlProcessingInstruction:
                    break;
                case SyntaxKind.TypeCref:
                    break;
                case SyntaxKind.QualifiedCref:
                    break;
                case SyntaxKind.NameMemberCref:
                    break;
                case SyntaxKind.IndexerMemberCref:
                    break;
                case SyntaxKind.OperatorMemberCref:
                    break;
                case SyntaxKind.ConversionOperatorMemberCref:
                    break;
                case SyntaxKind.CrefParameterList:
                    break;
                case SyntaxKind.CrefBracketedParameterList:
                    break;
                case SyntaxKind.CrefParameter:
                    break;
                case SyntaxKind.IdentifierName:
                    break;
                case SyntaxKind.QualifiedName:
                    break;
                case SyntaxKind.GenericName:
                    break;
                case SyntaxKind.TypeArgumentList:
                    break;
                case SyntaxKind.AliasQualifiedName:
                    break;
                case SyntaxKind.PredefinedType:
                    break;
                case SyntaxKind.ArrayType:
                    break;
                case SyntaxKind.ArrayRankSpecifier:
                    break;
                case SyntaxKind.PointerType:
                    break;
                case SyntaxKind.NullableType:
                    break;
                case SyntaxKind.OmittedTypeArgument:
                    break;
                case SyntaxKind.ParenthesizedExpression:
                    break;
                case SyntaxKind.ConditionalExpression:
                    break;
                case SyntaxKind.InvocationExpression:
                    break;
                case SyntaxKind.ElementAccessExpression:
                    break;
                case SyntaxKind.ArgumentList:
                    break;
                case SyntaxKind.BracketedArgumentList:
                    break;
                case SyntaxKind.Argument:
                    break;
                case SyntaxKind.NameColon:
                    break;
                case SyntaxKind.CastExpression:
                    break;
                case SyntaxKind.AnonymousMethodExpression:
                    break;
                case SyntaxKind.SimpleLambdaExpression:
                    break;
                case SyntaxKind.ParenthesizedLambdaExpression:
                    break;
                case SyntaxKind.ObjectInitializerExpression:
                    break;
                case SyntaxKind.CollectionInitializerExpression:
                    break;
                case SyntaxKind.ArrayInitializerExpression:
                    break;
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    break;
                case SyntaxKind.ComplexElementInitializerExpression:
                    break;
                case SyntaxKind.ObjectCreationExpression:
                    break;
                case SyntaxKind.AnonymousObjectCreationExpression:
                    break;
                case SyntaxKind.ArrayCreationExpression:
                    break;
                case SyntaxKind.ImplicitArrayCreationExpression:
                    break;
                case SyntaxKind.StackAllocArrayCreationExpression:
                    break;
                case SyntaxKind.OmittedArraySizeExpression:
                    break;
                case SyntaxKind.InterpolatedStringExpression:
                    break;
                case SyntaxKind.ImplicitElementAccess:
                    break;
                case SyntaxKind.AddExpression:
                    break;
                case SyntaxKind.SubtractExpression:
                    break;
                case SyntaxKind.MultiplyExpression:
                    break;
                case SyntaxKind.DivideExpression:
                    break;
                case SyntaxKind.ModuloExpression:
                    break;
                case SyntaxKind.LeftShiftExpression:
                    break;
                case SyntaxKind.RightShiftExpression:
                    break;
                case SyntaxKind.LogicalOrExpression:
                    break;
                case SyntaxKind.LogicalAndExpression:
                    break;
                case SyntaxKind.BitwiseOrExpression:
                    break;
                case SyntaxKind.BitwiseAndExpression:
                    break;
                case SyntaxKind.ExclusiveOrExpression:
                    break;
                case SyntaxKind.EqualsExpression:
                    break;
                case SyntaxKind.NotEqualsExpression:
                    break;
                case SyntaxKind.LessThanExpression:
                    break;
                case SyntaxKind.LessThanOrEqualExpression:
                    break;
                case SyntaxKind.GreaterThanExpression:
                    break;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    break;
                case SyntaxKind.IsExpression:
                    break;
                case SyntaxKind.AsExpression:
                    break;
                case SyntaxKind.CoalesceExpression:
                    break;
                case SyntaxKind.SimpleMemberAccessExpression:
                    break;
                case SyntaxKind.PointerMemberAccessExpression:
                    break;
                case SyntaxKind.ConditionalAccessExpression:
                    break;
                case SyntaxKind.MemberBindingExpression:
                    break;
                case SyntaxKind.ElementBindingExpression:
                    break;
                case SyntaxKind.SimpleAssignmentExpression:
                    break;
                case SyntaxKind.AddAssignmentExpression:
                    break;
                case SyntaxKind.SubtractAssignmentExpression:
                    break;
                case SyntaxKind.MultiplyAssignmentExpression:
                    break;
                case SyntaxKind.DivideAssignmentExpression:
                    break;
                case SyntaxKind.ModuloAssignmentExpression:
                    break;
                case SyntaxKind.AndAssignmentExpression:
                    break;
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                    break;
                case SyntaxKind.OrAssignmentExpression:
                    break;
                case SyntaxKind.LeftShiftAssignmentExpression:
                    break;
                case SyntaxKind.RightShiftAssignmentExpression:
                    break;
                case SyntaxKind.UnaryPlusExpression:
                    break;
                case SyntaxKind.UnaryMinusExpression:
                    break;
                case SyntaxKind.BitwiseNotExpression:
                    break;
                case SyntaxKind.LogicalNotExpression:
                    break;
                case SyntaxKind.PreIncrementExpression:
                    break;
                case SyntaxKind.PreDecrementExpression:
                    break;
                case SyntaxKind.PointerIndirectionExpression:
                    break;
                case SyntaxKind.AddressOfExpression:
                    break;
                case SyntaxKind.PostIncrementExpression:
                    break;
                case SyntaxKind.PostDecrementExpression:
                    break;
                case SyntaxKind.AwaitExpression:
                    break;
                case SyntaxKind.ThisExpression:
                    break;
                case SyntaxKind.BaseExpression:
                    break;
                case SyntaxKind.ArgListExpression:
                    break;
                case SyntaxKind.NumericLiteralExpression:
                    break;
                case SyntaxKind.StringLiteralExpression:
                    break;
                case SyntaxKind.CharacterLiteralExpression:
                    break;
                case SyntaxKind.TrueLiteralExpression:
                    break;
                case SyntaxKind.FalseLiteralExpression:
                    break;
                case SyntaxKind.NullLiteralExpression:
                    break;
                case SyntaxKind.TypeOfExpression:
                    break;
                case SyntaxKind.SizeOfExpression:
                    break;
                case SyntaxKind.CheckedExpression:
                    break;
                case SyntaxKind.UncheckedExpression:
                    break;
                case SyntaxKind.DefaultExpression:
                    break;
                case SyntaxKind.MakeRefExpression:
                    break;
                case SyntaxKind.RefValueExpression:
                    break;
                case SyntaxKind.RefTypeExpression:
                    break;
                case SyntaxKind.QueryExpression:
                    break;
                case SyntaxKind.QueryBody:
                    break;
                case SyntaxKind.FromClause:
                    break;
                case SyntaxKind.LetClause:
                    break;
                case SyntaxKind.JoinClause:
                    break;
                case SyntaxKind.JoinIntoClause:
                    break;
                case SyntaxKind.WhereClause:
                    break;
                case SyntaxKind.OrderByClause:
                    break;
                case SyntaxKind.AscendingOrdering:
                    break;
                case SyntaxKind.DescendingOrdering:
                    break;
                case SyntaxKind.SelectClause:
                    break;
                case SyntaxKind.GroupClause:
                    break;
                case SyntaxKind.QueryContinuation:
                    break;
                case SyntaxKind.Block:
                    break;
                case SyntaxKind.LocalDeclarationStatement:
                    break;
                case SyntaxKind.VariableDeclaration:
                    break;
                case SyntaxKind.VariableDeclarator:
                    break;
                case SyntaxKind.EqualsValueClause:
                    break;
                case SyntaxKind.ExpressionStatement:
                    break;
                case SyntaxKind.EmptyStatement:
                    break;
                case SyntaxKind.LabeledStatement:
                    break;
                case SyntaxKind.GotoStatement:
                    break;
                case SyntaxKind.GotoCaseStatement:
                    break;
                case SyntaxKind.GotoDefaultStatement:
                    break;
                case SyntaxKind.BreakStatement:
                    break;
                case SyntaxKind.ContinueStatement:
                    break;
                case SyntaxKind.ReturnStatement:
                    break;
                case SyntaxKind.YieldReturnStatement:
                    break;
                case SyntaxKind.YieldBreakStatement:
                    break;
                case SyntaxKind.ThrowStatement:
                    break;
                case SyntaxKind.WhileStatement:
                    break;
                case SyntaxKind.DoStatement:
                    break;
                case SyntaxKind.ForStatement:
                    break;
                case SyntaxKind.ForEachStatement:
                    break;
                case SyntaxKind.UsingStatement:
                    break;
                case SyntaxKind.FixedStatement:
                    break;
                case SyntaxKind.CheckedStatement:
                    break;
                case SyntaxKind.UncheckedStatement:
                    break;
                case SyntaxKind.UnsafeStatement:
                    break;
                case SyntaxKind.LockStatement:
                    break;
                case SyntaxKind.IfStatement:
                    break;
                case SyntaxKind.ElseClause:
                    break;
                case SyntaxKind.SwitchStatement:
                    break;
                case SyntaxKind.SwitchSection:
                    break;
                case SyntaxKind.CaseSwitchLabel:
                    break;
                case SyntaxKind.DefaultSwitchLabel:
                    break;
                case SyntaxKind.TryStatement:
                    break;
                case SyntaxKind.CatchClause:
                    break;
                case SyntaxKind.CatchDeclaration:
                    break;
                case SyntaxKind.CatchFilterClause:
                    break;
                case SyntaxKind.FinallyClause:
                    break;
                case SyntaxKind.CompilationUnit:
                    break;
                case SyntaxKind.GlobalStatement:
                    break;
                case SyntaxKind.NamespaceDeclaration:
                    break;
                case SyntaxKind.UsingDirective:
                    break;
                case SyntaxKind.ExternAliasDirective:
                    break;
                case SyntaxKind.AttributeList:
                    break;
                case SyntaxKind.AttributeTargetSpecifier:
                    break;
                case SyntaxKind.Attribute:
                    break;
                case SyntaxKind.AttributeArgumentList:
                    break;
                case SyntaxKind.AttributeArgument:
                    break;
                case SyntaxKind.NameEquals:
                    break;
                case SyntaxKind.ClassDeclaration:
                    break;
                case SyntaxKind.StructDeclaration:
                    break;
                case SyntaxKind.InterfaceDeclaration:
                    break;
                case SyntaxKind.EnumDeclaration:
                    break;
                case SyntaxKind.DelegateDeclaration:
                    break;
                case SyntaxKind.BaseList:
                    break;
                case SyntaxKind.SimpleBaseType:
                    break;
                case SyntaxKind.TypeParameterConstraintClause:
                    break;
                case SyntaxKind.ConstructorConstraint:
                    break;
                case SyntaxKind.ClassConstraint:
                    break;
                case SyntaxKind.StructConstraint:
                    break;
                case SyntaxKind.TypeConstraint:
                    break;
                case SyntaxKind.ExplicitInterfaceSpecifier:
                    break;
                case SyntaxKind.EnumMemberDeclaration:
                    break;
                case SyntaxKind.FieldDeclaration:
                    break;
                case SyntaxKind.EventFieldDeclaration:
                    break;
                case SyntaxKind.MethodDeclaration:
                    break;
                case SyntaxKind.OperatorDeclaration:
                    break;
                case SyntaxKind.ConversionOperatorDeclaration:
                    break;
                case SyntaxKind.ConstructorDeclaration:
                    break;
                case SyntaxKind.BaseConstructorInitializer:
                    break;
                case SyntaxKind.ThisConstructorInitializer:
                    break;
                case SyntaxKind.DestructorDeclaration:
                    break;
                case SyntaxKind.PropertyDeclaration:
                    break;
                case SyntaxKind.EventDeclaration:
                    break;
                case SyntaxKind.IndexerDeclaration:
                    break;
                case SyntaxKind.AccessorList:
                    break;
                case SyntaxKind.GetAccessorDeclaration:
                    break;
                case SyntaxKind.SetAccessorDeclaration:
                    break;
                case SyntaxKind.AddAccessorDeclaration:
                    break;
                case SyntaxKind.RemoveAccessorDeclaration:
                    break;
                case SyntaxKind.UnknownAccessorDeclaration:
                    break;
                case SyntaxKind.ParameterList:
                    break;
                case SyntaxKind.BracketedParameterList:
                    break;
                case SyntaxKind.Parameter:
                    break;
                case SyntaxKind.TypeParameterList:
                    break;
                case SyntaxKind.TypeParameter:
                    break;
                case SyntaxKind.IncompleteMember:
                    break;
                case SyntaxKind.ArrowExpressionClause:
                    break;
                case SyntaxKind.Interpolation:
                    break;
                case SyntaxKind.InterpolatedStringText:
                    break;
                case SyntaxKind.InterpolationAlignmentClause:
                    break;
                case SyntaxKind.InterpolationFormatClause:
                    break;
                case SyntaxKind.ShebangDirectiveTrivia:
                    break;
                case SyntaxKind.LoadDirectiveTrivia:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Trivia.TriviaTypes.UnknownTriviaSyntaxType;
        }
        public override string ToString() => string.IsNullOrEmpty(Name) ? NodeType.ToString() : $"{NodeType}[{Name}]";
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
