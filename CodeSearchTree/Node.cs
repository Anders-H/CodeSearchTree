using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.ComponentModel;

namespace CodeSearchTree
{
   public class Node
   {
      public enum NodeTypes //Ändra även i filen SearchExpresionParser.cs om denna ändras.
      {
         Any,
         UnknownNode,
         UsingDirectiveSyntaxNode,
         NamespaceDeclarationSyntaxNode,
         ClassDeclarationSyntaxNode,
         IdentifierNameSyntaxNode,
         QualifiedNameSyntaxNode,
         FieldDeclarationSyntaxNode,
         VariableDeclarationSyntaxNode,
         VariableDeclaratorSyntaxNode,
         PropertyDeclarationSyntaxNode,
         AccessorListSyntaxNode,
         AccessorDeclarationSyntaxNode,
         AttributeListSyntaxNode,
         AttributeSyntaxNode,
         AttributeArgumentListSyntaxNode,
         BlockSyntaxNode,
         ReturnStatementSyntaxNode,
         MethodDeclarationSyntaxNode,
         PredefinedTypeSyntaxNode,
         ParameterListSyntaxNode,
         ExpressionStatementSyntaxNode,
         InvocationExpressionSyntaxNode,
         ArgumentListSyntaxNode,
         AssignmentExpressionSyntaxNode,
         MemberAccessExpressionSyntaxNode,
         SwitchStatementSyntaxNode,
         ArgumentSyntaxNode,
         LiteralExpressionSyntaxNode,
         IfStatementSyntaxNode,
         PrefixUnaryExpressionSyntaxNode,
         ParenthesizedExpressionSyntaxNode,
         BinaryExpressionSyntaxNode,
         ElseClauseSyntaxNode,
         WhileStatementSyntaxNode,
         BreakStatementSyntaxNode,
         UsingStatementSyntaxNode,
         ForStatementSyntaxNode,
         LabeledStatementSyntaxNode,
         BaseListSyntaxNode,
         SimpleBaseTypeSyntaxNode,
         GenericNameSyntaxNode,
         TypeArgumentListSyntaxNode,
         ParameterSyntaxNode,
         LocalDeclarationStatementSyntaxNode,
         EqualsValueClauseSyntaxNode,
         ObjectCreationExpressionSyntaxNode,
         TypeOfExpressionSyntaxNode,
         ThrowStatementSyntaxNode,
         ThisExpressionSyntaxNode,
         SimpleLambdaExpressionSyntaxNode,
         ForEachStatementSyntaxNode,
         TryStatementSyntaxNode,
         CatchClauseSyntaxNode,
         SwitchSectionSyntaxNode,
         CaseSwitchLabelSyntaxNode,
         DefaultSwitchLabelSyntaxNode,
         ArrayTypeSyntaxNode,
         ArrayRankSpecifierSyntaxNode,
         OmittedArraySizeExpressionSyntaxNode,
         ElementAccessExpressionSyntaxNode,
         BracketedArgumentListSyntaxNode,
         ConditionalExpressionSyntaxNode,
         PostfixUnaryExpressionSyntaxNode,
         ContinueStatementSyntaxNode,
         ConstructorDeclarationSyntaxNode,
         QueryExpressionSyntaxNode, //LINQ
         FromClauseSyntaxNode, //LINQ
         QueryBodySyntaxNode, //LINQ
         WhereClauseSyntaxNode, //LINQ
         SelectClauseSyntaxNode, //LINQ
         DoStatementSyntaxNode,
         NameEqualsSyntaxNode,
         EnumDeclarationSyntaxNode,
         EnumMemberDeclarationSyntaxNode,
         AttributeArgumentSyntaxNode,
         ConstructorInitializerSyntaxNode,
         EmptyStatementSyntaxNode,
         InitializerExpressionSyntaxNode,
         AwaitExpressionSyntaxNode,
         AnonymousObjectCreationExpressionSyntaxNode,
         AnonymousObjectMemberDeclaratorSyntaxNode,
         TypeParameterListSyntaxNode,
         TypeParameterSyntaxNode,
         DefaultExpressionSyntaxNode,
         InterfaceDeclarationSyntaxNode,
         CastExpressionSyntaxNode,
         BaseExpressionSyntaxNode,
         AttributeTargetSpecifierSyntaxNode,
         AliasQualifiedNameSyntaxNode,
         ExplicitInterfaceSpecifierSyntaxNode
      }

      [Category("Meta"), Description("Enumeration of Roslyn types.")]
      public NodeTypes NodeType { get; internal set; }
      [Category("Relatives"), Description("List of child nodes.")]
      public NodeList Children { get; private set; }
      [Category("Main"), Description("Original source code.")]
      public string Source { get; internal set; }
      [Category("Main"), Description("Start character position.")]
      public int StartPosition { get; internal set; }
      [Category("Main"), Description("End character position.")]
      public int EndPosition { get; internal set; }
      [Category("Meta"), Description("List of leading trivia.")]
      public TriviaList LeadingTrivia { get; private set; }
      [Category("Meta"), Description("List of trailing trivia.")]
      public TriviaList TrailingTrivia { get; private set; }
      [Category("Meta"), Description("Name or identifier of node.")]
      public string Name { get; private set; }
      [Category("Relatives"), Description("Reference to parent node, if available.")]
      public Node Parent { get; private set; }
      internal NodeList ParentListIfNoParent { get; set; }
      public object RoslynNode { get; private set; }

      protected internal Node(object roslyn_node, string source) : this(roslyn_node, source, null, NodeTypes.NamespaceDeclarationSyntaxNode)
      {
      }

      protected internal Node(object roslyn_node, string source, Node parent, NodeTypes node_type)
      {
         this.RoslynNode = roslyn_node;
         this.StartPosition = (roslyn_node as SyntaxNode).FullSpan.Start;
         this.EndPosition = (roslyn_node as SyntaxNode).FullSpan.End;
         this.NodeType = node_type;
         this.Children = new NodeList();
         this.LeadingTrivia = new TriviaList();
         this.TrailingTrivia = new TriviaList();
         this.Parent = parent;
         this.Source = source;
      }

      [Category("Main"), Description("Original source length in characters.")]
      public int Length { get { return this.Source.Length; } }
      [Category("Relatives"), Description("Number of child nodes.")]
      public int ChildCount { get { return this.Children.Count; } }
      [Category("Relatives"), Description("Type of parant node, if available.")]
      public NodeTypes ParentType { get { return this.Parent == null ? NodeTypes.UnknownNode : this.Parent.NodeType; } }
      [Category("Meta"), Description("String representation of leading trivia.")]
      public string LeadingTriviaString { get { var s = new StringBuilder(); this.LeadingTrivia.ForEach(x => s.Append(x.Source)); return s.ToString(); } }
      [Category("Meta"), Description("String representation of trailing trivia.")]
      public string TrailingTriviaString { get { var s = new StringBuilder(); this.TrailingTrivia.ForEach(x => s.Append(x.Source)); return s.ToString(); } }

      [Category("Roslyn"), Description("Properties of the underlying Roslyn SyntaxNode.")]
      public List<Property> RoslynNodeProperties
      {
         get
         {
            var ret = new List<Property>();
            var n = this.RoslynNode as SyntaxNode;
            if (!(n == null))
            {
               var properties = n.GetType().GetProperties();
               properties.ToList().ForEach(x => ret.Add(new Property(x.Name, x.GetValue(n))));
            }
            return ret;
         }
      }

      [Category("Roslyn"), Description("String representation of the properties of the underlying Roslyn SyntaxNode.")]
      public string RoslynNodePropertiesString
      {
         get
         {
            var s = new StringBuilder();
            this.RoslynNodeProperties.ForEach(x => s.AppendLine(x.ToString()));
            return s.ToString();
         }
      }

      public static NodeList CreateTreeFromFile(string filename)
      {
         var code = "";
         using (var sr = new System.IO.StreamReader(filename, Encoding.UTF8))
            code = sr.ReadToEnd();
         return CreateTreeFromCode(code);
      }

      public static NodeList CreateTreeFromCode(string code)
      {
         var ret = new NodeList();
         var tree = CSharpSyntaxTree.ParseText(code).GetRoot();
         foreach (var n in tree.ChildNodes())
         {
            var code_node = new Node(n, n.ToString(), null, GetNodeType(n));
            code_node.ParentListIfNoParent = ret;
            StoreTrivia(code_node, n);
            ret.Add(code_node);
            CreateChildren(code_node, n);
         }
         foreach (var n in ret)
            n.CheckName();
         return ret;
      }

      private void CheckName()
      {
         var n = this.RoslynNode as SyntaxNode;
         if (!(n == null))
         {
            if (n is ClassDeclarationSyntax)
               this.Name = (n as ClassDeclarationSyntax).Identifier.ToString();
            else if (n is NamespaceDeclarationSyntax)
               this.Name = (n as NamespaceDeclarationSyntax).Name.ToString();
            else if (n is FieldDeclarationSyntax)
            {
               var v = this.GetChild("vardeclaration/id");
               this.Name = v == null ? "" : (v.RoslynNode as IdentifierNameSyntax).ToString();
            }
            else if (n is VariableDeclarationSyntax || n is PropertyDeclarationSyntax)
            {
               var v = this.GetChild("id");
               this.Name = v == null ? "" : (v.RoslynNode as IdentifierNameSyntax).ToString();
            }
            else if (n is IdentifierNameSyntax)
               this.Name = n.ToString();
            else if (n is MethodDeclarationSyntax)
               this.Name = (n as MethodDeclarationSyntax).Identifier.ToString();
            else if (n is UsingDirectiveSyntax)
            {
               var id = this.GetChild("name");
               if (id == null)
               {
                  id = this.GetChild("id");
                  this.Name = id == null ? "" : (id.RoslynNode as IdentifierNameSyntax).ToString();
               }
               else
                  this.Name = (id.RoslynNode as QualifiedNameSyntax).ToString();
               
            }
         }
         if (string.IsNullOrWhiteSpace(this.Name))
            this.Name = "";
         foreach (var child in this.Children)
            child.CheckName();
      }

      private static void CreateChildren(Node node, SyntaxNode roslyn_node)
      {
         foreach (var n in roslyn_node.ChildNodes())
         {
            var code_node = new Node(n, n.ToString(), node, GetNodeType(n));
            StoreTrivia(code_node, n);
            node.Children.Add(code_node);
            CreateChildren(code_node, n);
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
            var current_parent = this;
            var current_this = this;
            do
            {
               //Växla till nästa förälder.
               current_parent = current_this.Parent;
               if (current_parent == null)
               {
                  //Ingen mer parent. Kolla var vi ligger i root-listan.
                  if (current_this.ParentListIfNoParent == null)
                     ret = string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                      ret == "" ? "" : "/",
                                                      ret);
                  else
                  {
                     //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                     var index = current_this.ParentListIfNoParent.Where(x => x.NodeType == current_this.NodeType).ToList().IndexOf(current_this);
                     //Om index är > 0, presentera det som en [vakt].
                     ret = index <= 0
                              ? string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                           ret == "" ? "" : "/",
                                                           ret)
                              : string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                                index,
                                                                ret == "" ? "" : "/",
                                                                ret);
                  }
               }
               else
               {
                  var index = current_parent.Children.Where(x => x.NodeType == current_this.NodeType).ToList().IndexOf(current_this);
                  //Om index är > 0, presentera det som en [vakt].
                  ret = index <= 0
                           ? string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                        ret == "" ? "" : "/",
                                                        ret)
                           : string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                             index,
                                                             ret == "" ? "" : "/",
                                                             ret);
               }
               //Växla till nästa child.
               current_this = current_parent;
            } while (!(current_parent == null));
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
            var current_parent = this;
            var current_this = this;
            do
            {
               //Växla till nästa förälder.
               current_parent = current_this.Parent;
               if (current_parent == null)
               {
                  //Ingen mer parent. Kolla var vi ligger i root-listan.
                  if (current_this.Name == "")
                  {
                     if (current_this.ParentListIfNoParent == null)
                        ret = string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                         ret == "" ? "" : "/",
                                                         ret);
                     else
                     {
                        //Filtrera ut så att vi har rätt typ, därefter ta reda på index.
                        var index = current_this.ParentListIfNoParent.Where(x => x.NodeType == current_this.NodeType).ToList().IndexOf(current_this);
                        //Om index är > 0, presentera det som en [vakt].
                        ret = index <= 0
                                 ? string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                              ret == "" ? "" : "/",
                                                              ret)
                                 : string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                                   index,
                                                                   ret == "" ? "" : "/",
                                                                   ret);
                     }
                  }
                  else
                  {
                     ret = string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                           current_this.Name,
                                                           ret == "" ? "" : "/",
                                                           ret);
                  }
               }
               else
               {
                  if (current_this.Name == "")
                  {
                     var index = current_parent.Children.Where(x => x.NodeType == current_this.NodeType).ToList().IndexOf(current_this);
                     //Om index är > 0, presentera det som en [vakt].
                     ret = index <= 0
                              ? string.Format("{0}{1}{2}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                           ret == "" ? "" : "/",
                                                           ret)
                              : string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                                index,
                                                                ret == "" ? "" : "/",
                                                                ret);
                  }
                  else
                  {
                     ret = string.Format("{0}[{1}]{2}{3}", SearchExpressionParser.NodeTypeToKeyword(current_this.NodeType),
                                                           current_this.Name,
                                                           ret == "" ? "" : "/",
                                                           ret);
                  }
               }
               //Växla till nästa child.
               current_this = current_parent;
            } while (!(current_parent == null));
            return ret;
         }
      }

      private static NodeTypes GetNodeType(SyntaxNode n)
      {
         if (n is UsingDirectiveSyntax)
            return NodeTypes.UsingDirectiveSyntaxNode;
         else if (n is NamespaceDeclarationSyntax)
            return NodeTypes.NamespaceDeclarationSyntaxNode;
         else if (n is ClassDeclarationSyntax)
            return NodeTypes.ClassDeclarationSyntaxNode;
         else if (n is IdentifierNameSyntax)
            return NodeTypes.IdentifierNameSyntaxNode;
         else if (n is QualifiedNameSyntax)
            return NodeTypes.QualifiedNameSyntaxNode;
         else if (n is FieldDeclarationSyntax)
            return NodeTypes.FieldDeclarationSyntaxNode;
         else if (n is VariableDeclarationSyntax)
            return NodeTypes.VariableDeclarationSyntaxNode;
         else if (n is VariableDeclaratorSyntax)
            return NodeTypes.VariableDeclaratorSyntaxNode;
         else if (n is PropertyDeclarationSyntax)
            return NodeTypes.PropertyDeclarationSyntaxNode;
         else if (n is AccessorListSyntax)
            return NodeTypes.AccessorListSyntaxNode;
         else if (n is AccessorDeclarationSyntax)
            return NodeTypes.AccessorDeclarationSyntaxNode;
         else if (n is AttributeListSyntax)
            return NodeTypes.AttributeListSyntaxNode;
         else if (n is AttributeSyntax)
            return NodeTypes.AttributeSyntaxNode;
         else if (n is AttributeArgumentListSyntax)
            return NodeTypes.AttributeArgumentListSyntaxNode;
         else if (n is BlockSyntax)
            return NodeTypes.BlockSyntaxNode;
         else if (n is ReturnStatementSyntax)
            return NodeTypes.ReturnStatementSyntaxNode;
         else if (n is MethodDeclarationSyntax)
            return NodeTypes.MethodDeclarationSyntaxNode;
         else if (n is PredefinedTypeSyntax)
            return NodeTypes.PredefinedTypeSyntaxNode;
         else if (n is ParameterListSyntax)
            return NodeTypes.ParameterListSyntaxNode;
         else if (n is ExpressionStatementSyntax)
            return NodeTypes.ExpressionStatementSyntaxNode;
         else if (n is InvocationExpressionSyntax)
            return NodeTypes.InvocationExpressionSyntaxNode;
         else if (n is ArgumentListSyntax)
            return NodeTypes.ArgumentListSyntaxNode;
         else if (n is AssignmentExpressionSyntax)
            return NodeTypes.AssignmentExpressionSyntaxNode;
         else if (n is MemberAccessExpressionSyntax)
            return NodeTypes.MemberAccessExpressionSyntaxNode;
         else if (n is SwitchStatementSyntax)
            return NodeTypes.SwitchStatementSyntaxNode;
         else if (n is ArgumentSyntax)
            return NodeTypes.ArgumentSyntaxNode;
         else if (n is LiteralExpressionSyntax)
            return NodeTypes.LiteralExpressionSyntaxNode;
         else if (n is IfStatementSyntax)
            return NodeTypes.IfStatementSyntaxNode;
         else if (n is PrefixUnaryExpressionSyntax)
            return NodeTypes.PrefixUnaryExpressionSyntaxNode;
         else if (n is ParenthesizedExpressionSyntax)
            return NodeTypes.ParenthesizedExpressionSyntaxNode;
         else if (n is BinaryExpressionSyntax)
            return NodeTypes.BinaryExpressionSyntaxNode;
         else if (n is ElseClauseSyntax)
            return NodeTypes.ElseClauseSyntaxNode;
         else if (n is WhileStatementSyntax)
            return NodeTypes.WhileStatementSyntaxNode;
         else if (n is BreakStatementSyntax)
            return NodeTypes.BreakStatementSyntaxNode;
         else if (n is UsingStatementSyntax)
            return NodeTypes.UsingStatementSyntaxNode;
         else if (n is ForStatementSyntax)
            return NodeTypes.ForStatementSyntaxNode;
         else if (n is LabeledStatementSyntax)
            return NodeTypes.LabeledStatementSyntaxNode;
         else if (n is BaseListSyntax)
            return NodeTypes.BaseListSyntaxNode;
         else if (n is SimpleBaseTypeSyntax)
            return NodeTypes.SimpleBaseTypeSyntaxNode;
         else if (n is GenericNameSyntax)
            return NodeTypes.GenericNameSyntaxNode;
         else if (n is TypeArgumentListSyntax)
            return NodeTypes.TypeArgumentListSyntaxNode;
         else if (n is ParameterSyntax)
            return NodeTypes.ParameterSyntaxNode;
         else if (n is LocalDeclarationStatementSyntax)
            return NodeTypes.LocalDeclarationStatementSyntaxNode;
         else if (n is EqualsValueClauseSyntax)
            return NodeTypes.EqualsValueClauseSyntaxNode;
         else if (n is ObjectCreationExpressionSyntax)
            return NodeTypes.ObjectCreationExpressionSyntaxNode;
         else if (n is TypeOfExpressionSyntax)
            return NodeTypes.TypeOfExpressionSyntaxNode;
         else if (n is ThrowStatementSyntax)
            return NodeTypes.ThrowStatementSyntaxNode;
         else if (n is ThisExpressionSyntax)
            return NodeTypes.ThisExpressionSyntaxNode;
         else if (n is SimpleLambdaExpressionSyntax)
            return NodeTypes.SimpleLambdaExpressionSyntaxNode;
         else if (n is ForEachStatementSyntax)
            return NodeTypes.ForEachStatementSyntaxNode;
         else if (n is TryStatementSyntax)
            return NodeTypes.TryStatementSyntaxNode;
         else if (n is CatchClauseSyntax)
            return NodeTypes.CatchClauseSyntaxNode;
         else if (n is SwitchSectionSyntax)
            return NodeTypes.SwitchSectionSyntaxNode;
         else if (n is CaseSwitchLabelSyntax)
            return NodeTypes.CaseSwitchLabelSyntaxNode;
         else if (n is DefaultSwitchLabelSyntax)
            return NodeTypes.DefaultSwitchLabelSyntaxNode;
         else if (n is ArrayTypeSyntax)
            return NodeTypes.ArrayTypeSyntaxNode;
         else if (n is ArrayRankSpecifierSyntax)
            return NodeTypes.ArrayRankSpecifierSyntaxNode;
         else if (n is OmittedArraySizeExpressionSyntax)
            return NodeTypes.OmittedArraySizeExpressionSyntaxNode;
         else if (n is ElementAccessExpressionSyntax)
            return NodeTypes.ElementAccessExpressionSyntaxNode;
         else if (n is BracketedArgumentListSyntax)
            return NodeTypes.BracketedArgumentListSyntaxNode;
         else if (n is ConditionalExpressionSyntax)
            return NodeTypes.ConditionalExpressionSyntaxNode;
         else if (n is PostfixUnaryExpressionSyntax)
            return NodeTypes.PostfixUnaryExpressionSyntaxNode;
         else if (n is ContinueStatementSyntax)
            return NodeTypes.ContinueStatementSyntaxNode;
         else if (n is ConstructorDeclarationSyntax)
            return NodeTypes.ConstructorDeclarationSyntaxNode;
         else if (n is QueryExpressionSyntax)
            return NodeTypes.QueryExpressionSyntaxNode;
         else if (n is FromClauseSyntax)
            return NodeTypes.FromClauseSyntaxNode;
         else if (n is QueryBodySyntax)
            return NodeTypes.QueryBodySyntaxNode;
         else if (n is WhereClauseSyntax)
            return NodeTypes.WhereClauseSyntaxNode;
         else if (n is SelectClauseSyntax)
            return NodeTypes.SelectClauseSyntaxNode;
         else if (n is DoStatementSyntax)
            return NodeTypes.DoStatementSyntaxNode;
         else if (n is NameEqualsSyntax)
            return NodeTypes.NameEqualsSyntaxNode;
         else if (n is EnumDeclarationSyntax)
            return NodeTypes.EnumDeclarationSyntaxNode;
         else if (n is EnumMemberDeclarationSyntax)
            return NodeTypes.EnumMemberDeclarationSyntaxNode;
         else if (n is AttributeArgumentSyntax)
            return NodeTypes.AttributeArgumentSyntaxNode;
         else if (n is ConstructorInitializerSyntax)
            return NodeTypes.ConstructorInitializerSyntaxNode;
         else if (n is EmptyStatementSyntax)
            return NodeTypes.EmptyStatementSyntaxNode;
         else if (n is InitializerExpressionSyntax)
            return NodeTypes.InitializerExpressionSyntaxNode;
         else if (n is AwaitExpressionSyntax)
            return NodeTypes.AwaitExpressionSyntaxNode;
         else if (n is AnonymousObjectCreationExpressionSyntax)
            return NodeTypes.AnonymousObjectCreationExpressionSyntaxNode;
         else if (n is AnonymousObjectMemberDeclaratorSyntax)
            return NodeTypes.AnonymousObjectMemberDeclaratorSyntaxNode;
         else if (n is TypeParameterListSyntax)
            return NodeTypes.TypeParameterListSyntaxNode;
         else if (n is TypeParameterSyntax)
            return NodeTypes.TypeParameterSyntaxNode;
         else if (n is DefaultExpressionSyntax)
            return NodeTypes.DefaultExpressionSyntaxNode;
         else if (n is InterfaceDeclarationSyntax)
            return NodeTypes.InterfaceDeclarationSyntaxNode;
         else if (n is CastExpressionSyntax)
            return NodeTypes.CastExpressionSyntaxNode;
         else if (n is BaseExpressionSyntax)
            return NodeTypes.BaseExpressionSyntaxNode;
         else if (n is AttributeTargetSpecifierSyntax)
            return NodeTypes.AttributeTargetSpecifierSyntaxNode;
         else if (n is AliasQualifiedNameSyntax)
            return NodeTypes.AliasQualifiedNameSyntaxNode;
         else if (n is ExplicitInterfaceSpecifierSyntax)
            return NodeTypes.ExplicitInterfaceSpecifierSyntaxNode;
         else
         {
#if DEBUG
            Console.WriteLine(n.GetType().Name);
            var code = n.ToString().Length > 40 ? n.ToString().Substring(0, 40) : n.ToString();
            Console.WriteLine(code);
            throw new Exception(n.GetType().Name);
#else
            return NodeTypes.UnknownNode;
#endif
         }
      }

      private static void StoreTrivia(Node node, SyntaxNode roslyn_node)
      {
         
         foreach (var t in roslyn_node.GetLeadingTrivia())
         {
            var s = t.ToString().Trim();
            if (!(s == ""))
               node.LeadingTrivia.Add(new Trivia(GetTriviaType(t), s));
         }
         foreach (var t in roslyn_node.GetTrailingTrivia())
         {
            var s = t.ToString().Trim();
            if (!(s == ""))
               node.TrailingTrivia.Add(new Trivia(GetTriviaType(t), s));
         }

      }

      public NodeList GetChildren(params NodeTypes[] type)
      {
         if (type.Length <= 0)
            return new NodeList(); ;
         if (type.Length == 1)
            return this.Children.FilterByNameOrIndexOrType(type[0]);
         else if (type.Length == 2)
         {
            var ret = new NodeList();
            var item = this.Children.FilterByNameOrIndexOrType(type[0]).FirstOrDefault();
            if (item == null)
               return ret;
            ret.AddRange(item.Children.FilterByNameOrIndexOrType(type[1]));
            return ret;
         }
         else
         {
            var item = this.Children.FilterByNameOrIndexOrType(type[0]).FirstOrDefault();
            for (int i = 1; i < type.Length - 1; i++)
            {
               if (item == null)
                  return new NodeList();
               item = item.Children.FilterByNameOrIndexOrType(type[i]).FirstOrDefault();
            }
            if (item == null)
               return new NodeList();
            return item.Children.FilterByNameOrIndexOrType(type[type.Length - 1]);
         }
      }

      public Node GetChild(params NodeTypes[] type) => this.GetChildren(type).FirstOrDefault();

      public Node GetChild(params SearchNode[] sn)
      {
         if (sn.Length <= 0)
            return null;
         if (sn.Length == 1)
            return this.Children.FilterByTypeAndNameOrIndex(sn[0]).FirstOrDefault();
         else if (sn.Length == 2)
         {
            var item = this.Children.FilterByTypeAndNameOrIndex(sn[0]).FirstOrDefault();
            if (item == null)
               return null;
            return item.Children.FilterByTypeAndNameOrIndex(sn[1]).FirstOrDefault();
         }
         else
         {
            var item = this.Children.FilterByTypeAndNameOrIndex(sn[0]).FirstOrDefault();
            for (int i = 1; i < (sn.Length - 1); i++)
            {
               if (item == null)
                  return null;
               item = item.Children.FilterByTypeAndNameOrIndex(sn[i]).FirstOrDefault();
            }
            return item.Children.FilterByTypeAndNameOrIndex(sn[sn.Length - 1]).FirstOrDefault();
         }
      }

      public static SearchNodeList ParseSearchExpression(string search_expression, out bool success)
      {
         success = true;
#if !DEBUG
         try
         {
#endif
            return new SearchExpressionParser(search_expression).Parse();
#if !DEBUG
         }
         catch
         {
            success = false;
            return null;
         }
#endif
      }

      public static SearchNodeList ParseSearchExpression(string search_expression) =>
         new SearchExpressionParser(search_expression).Parse();

      public Node GetChild(string search_expression) =>
         this.GetChild(new SearchExpressionParser(search_expression).Parse().ToArray());
      
      public Node GetNextSibling() =>
         this.Parent == null ? null : this.Parent.Children.GetNextSibling(this);

      public Node GetPreviousSibling() =>
         this.Parent == null ? null : this.Parent.Children.GetPreviousSibling(this);

      private static Trivia.TriviaTypes GetTriviaType(SyntaxTrivia t)
      {
         if (t.Kind() == SyntaxKind.RegionDirectiveTrivia)
            return Trivia.TriviaTypes.RegionDirectiveTriviaSyntaxType;
         else if (t.Kind() == SyntaxKind.SingleLineCommentTrivia)
            return Trivia.TriviaTypes.SingleLineCommentTriviaType;
         else if (t.Kind() == SyntaxKind.EndRegionDirectiveTrivia)
            return Trivia.TriviaTypes.EndRegionDirectiveTriviaType;
         else if (t.Kind() == SyntaxKind.MultiLineCommentTrivia)
            return Trivia.TriviaTypes.MultiLineCommentTriviaType;
         else if (t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
            return Trivia.TriviaTypes.SingleLineDocumentationCommentTriviaType;
         else if (t.Kind() == SyntaxKind.IfDirectiveTrivia)
            return Trivia.TriviaTypes.IfDirectiveTriviaType;
         else if (t.Kind() == SyntaxKind.DisabledTextTrivia)
            return Trivia.TriviaTypes.DisabledTextTriviaType;
         else if (t.Kind() == SyntaxKind.ElseDirectiveTrivia)
            return Trivia.TriviaTypes.ElseDirectiveTriviaType;
         else if (t.Kind() == SyntaxKind.PragmaChecksumDirectiveTrivia)
            return Trivia.TriviaTypes.PragmaChecksumDirectiveTriviaType;
         else if (t.Kind() == SyntaxKind.LineDirectiveTrivia)
            return Trivia.TriviaTypes.LineDirectiveTriviaType;
         else
         {
#if DEBUG
            Console.WriteLine(t.GetType().Name);
            var code = t.ToString().Length > 50 ? t.ToString().Substring(0, 50) : t.ToString();
            Console.WriteLine(code);
            throw new Exception(t.Kind().ToString());
#else
            return Trivia.TriviaTypes.UnknownTriviaSyntaxType;
#endif
         }
      }

      public override string ToString() =>
         string.IsNullOrEmpty(this.Name) ? this.NodeType.ToString() : $"{this.NodeType}[{this.Name}]";
   }
}
