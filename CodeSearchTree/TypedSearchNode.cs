﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSearchTree
{
    public class TypedSearchNode : ITypedChild
    {
        //Typed search.
        public TypedSearchNode Assign => new TypedSearchNode(NodeType.AssignmentExpressionSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Block => new TypedSearchNode(NodeType.BlockSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Cls => new TypedSearchNode(NodeType.ClassDeclarationSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Constructor => new TypedSearchNode(NodeType.ConstructorDeclarationSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode EqualsValue => new TypedSearchNode(NodeType.EqualsValueClauseSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Expression => new TypedSearchNode(NodeType.ExpressionStatementSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Field => new TypedSearchNode(NodeType.FieldDeclarationSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Literal => new TypedSearchNode(NodeType.LiteralExpressionSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode MemberAccess => new TypedSearchNode(NodeType.MemberAccessExpressionSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Id => new TypedSearchNode(NodeType.IdentifierNameSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Invocation => new TypedSearchNode(NodeType.InvocationExpressionSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode Ns => new TypedSearchNode(NodeType.NamespaceDeclarationSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode UsingDirective => new TypedSearchNode(NodeType.UsingDirectiveSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode VarDeclaration => new TypedSearchNode(NodeType.VariableDeclarationSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        public TypedSearchNode VarDeclarator => new TypedSearchNode(NodeType.VariableDeclaratorSyntaxNode, OwnerNode.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch)));
        //End typed search.

        public Node SearchResult => OwnerNode?.GetChild(SearchNode.CreateSearchByType(NodeTypeSearch));

        private NodeType NodeTypeSearch { get; }
        private ITypedSearch OwnerNode { get; }

        internal TypedSearchNode(NodeType nodeTypeSearch, ITypedSearch ownerNode) { NodeTypeSearch = nodeTypeSearch; OwnerNode = ownerNode; }

        public Node this[int index] => OwnerNode?.GetChild(SearchNode.CreateSearchByTypeAndIndex(NodeTypeSearch, index));

        public Node this[string name] => OwnerNode?.GetChild(SearchNode.CerateSearchByTypeAndName(NodeTypeSearch, name));
    }
}