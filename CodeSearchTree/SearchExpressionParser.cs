using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeSearchTree
{
   internal sealed class SearchExpressionParser
   {
      private string Source { get; set; }

      public SearchExpressionParser(string source)
      {
         this.Source = source == null ? "" : source.Trim();
      }

      public SearchNodeList Parse()
      {
         var ret = new SearchNodeList();
         if (!(this.Source == ""))
         {
            string[] parts = this.Source.Split('/');
            const string NO_INDEX = @"^(\*|[a-z]+)$";
            const string WITH_INDEX = @"^(\*|[a-z]+)\[[0-9]+\]$";
            const string WITH_NAME = @"^(\*|[a-z]+)\[.+\]$";
            foreach (var part in parts)
            {
               if (Regex.IsMatch(part, NO_INDEX))
                  ret.Add(new SearchNode(this.KeywordToEnum(part)));
               else if (Regex.IsMatch(part, WITH_INDEX))
               {
                  var _open = part.IndexOf('[');
                  var _close = part.IndexOf(']');
                  var index_string = part.Substring(_open + 1, _close - (_open + 1)).Trim();
                  var index = int.Parse(index_string);
                  ret.Add(new SearchNode(this.KeywordToEnum(part.Substring(0, _open)), index));
               }
               else if (Regex.IsMatch(part, WITH_NAME))
               {
                  var _open = part.IndexOf('[');
                  var _close = part.IndexOf(']');
                  var name = part.Substring(_open + 1, _close - (_open + 1)).Trim();
                  ret.Add(new SearchNode(this.KeywordToEnum(part.Substring(0, _open)), name));
               }
               else
                  throw new Exception("Query expression contains errors.");
            }
         }
         return ret;
      }

      private Node.NodeTypes KeywordToEnum(string keyword)
      {
         switch (keyword) //Ändra här om enumen i filen Node.cs ändras.
         {
            case "*":
               return Node.NodeTypes.Any;
            case "usingdirective":
            case "using_directive":
               return Node.NodeTypes.UsingDirectiveSyntaxNode;
            case "namespace":
            case "ns":
               return Node.NodeTypes.NamespaceDeclarationSyntaxNode;
            case "class":
            case "cls":
               return Node.NodeTypes.ClassDeclarationSyntaxNode;
            case "id":
            case "identifier":
               return Node.NodeTypes.IdentifierNameSyntaxNode;
            case "name":
               return Node.NodeTypes.QualifiedNameSyntaxNode;
            case "field":
               return Node.NodeTypes.FieldDeclarationSyntaxNode;
            case "vardeclaration":
            case "variabledeclaration":
            case "var_declaration":
            case "variable_declaration":
               return Node.NodeTypes.VariableDeclarationSyntaxNode;
            case "vardeclarator":
            case "variabledeclarator":
            case "var_declarator":
            case "variable_declarator":
               return Node.NodeTypes.VariableDeclaratorSyntaxNode;
            case "property":
               return Node.NodeTypes.PropertyDeclarationSyntaxNode;
            case "accessorlist":
            case "accessor_list":
               return Node.NodeTypes.AccessorListSyntaxNode;
            case "accessordeclaration":
            case "accessor_declaration":
               return Node.NodeTypes.AccessorDeclarationSyntaxNode;
            case "attlist":
            case "att_list":
            case "attributelist":
            case "attribute_list":
               return Node.NodeTypes.AttributeListSyntaxNode;
            case "attribute":
               return Node.NodeTypes.AttributeSyntaxNode;
            case "attarg":
            case "att_arg":
            case "attributeargument":
            case "attribute_argument":
               return Node.NodeTypes.AttributeArgumentListSyntaxNode;
            case "block":
               return Node.NodeTypes.BlockSyntaxNode;
            case "return":
               return Node.NodeTypes.ReturnStatementSyntaxNode;
            case "method":
               return Node.NodeTypes.MethodDeclarationSyntaxNode;
            case "predeftype":
            case "pre_def_type":
            case "predefinedtype":
            case "predefined_type":
               return Node.NodeTypes.PredefinedTypeSyntaxNode;
            case "paramlist":
            case "parameterlist":
            case "param_list":
            case "parameter_list":
               return Node.NodeTypes.ParameterListSyntaxNode;
            case "expression":
               return Node.NodeTypes.ExpressionStatementSyntaxNode;
            case "invocation":
               return Node.NodeTypes.InvocationExpressionSyntaxNode;
            case "arglist":
            case "argumentlist":
            case "arg_list":
            case "argument_list":
               return Node.NodeTypes.ArgumentListSyntaxNode;
            case "assignment":
            case "assign":
               return Node.NodeTypes.AssignmentExpressionSyntaxNode;
            case "memberaccess":
            case "membershipaccess":
            case "member_access":
            case "membership_access":
               return Node.NodeTypes.MemberAccessExpressionSyntaxNode;
            case "switch":
               return Node.NodeTypes.SwitchStatementSyntaxNode;
            case "arg":
            case "argument":
               return Node.NodeTypes.ArgumentSyntaxNode;
            case "literal":
               return Node.NodeTypes.LiteralExpressionSyntaxNode;
            case "if":
               return Node.NodeTypes.IfStatementSyntaxNode;
            case "prefix":
               return Node.NodeTypes.PrefixUnaryExpressionSyntaxNode;
            case "parenthesizedexpression":
            case "parenthesized_expression":
               return Node.NodeTypes.ParenthesizedExpressionSyntaxNode;
            case "binaryexpression":
            case "binary_expression":
               return Node.NodeTypes.BinaryExpressionSyntaxNode;
            case "else":
               return Node.NodeTypes.ElseClauseSyntaxNode;
            case "while":
               return Node.NodeTypes.WhileStatementSyntaxNode;
            case "break":
               return Node.NodeTypes.BreakStatementSyntaxNode;
            case "using":
               return Node.NodeTypes.UsingStatementSyntaxNode;
            case "for":
               return Node.NodeTypes.ForStatementSyntaxNode;
            case "labeledstatement":
            case "labeled_statement":
               return Node.NodeTypes.LabeledStatementSyntaxNode;
            case "baselist":
            case "base_list":
               return Node.NodeTypes.BaseListSyntaxNode;
            case "basetype":
            case "base_type":
               return Node.NodeTypes.SimpleBaseTypeSyntaxNode;
            case "genericname":
            case "generic_name":
               return Node.NodeTypes.GenericNameSyntaxNode;
            case "typearg":
            case "typeargument":
            case "type_arg":
            case "type_argument":
               return Node.NodeTypes.TypeArgumentListSyntaxNode;
            case "param":
            case "parameter":
               return Node.NodeTypes.ParameterSyntaxNode;
            case "localdeclaration":
            case "local_declaration":
               return Node.NodeTypes.LocalDeclarationStatementSyntaxNode;
            case "equalsvalue":
            case "equals_value":
               return Node.NodeTypes.EqualsValueClauseSyntaxNode;
            case "new":
               return Node.NodeTypes.ObjectCreationExpressionSyntaxNode;
            case "typeof":
               return Node.NodeTypes.TypeOfExpressionSyntaxNode;
            case "throw":
               return Node.NodeTypes.ThrowStatementSyntaxNode;
            case "this":
               return Node.NodeTypes.ThisExpressionSyntaxNode;
            case "lambda":
               return Node.NodeTypes.SimpleLambdaExpressionSyntaxNode;
            case "foreach":
               return Node.NodeTypes.ForEachStatementSyntaxNode;
            case "try":
               return Node.NodeTypes.TryStatementSyntaxNode;
            case "catch":
               return Node.NodeTypes.CatchClauseSyntaxNode;
            case "switchselection":
            case "switch_selection":
               return Node.NodeTypes.SwitchSectionSyntaxNode;
            case "case":
               return Node.NodeTypes.CaseSwitchLabelSyntaxNode;
            case "default":
               return Node.NodeTypes.DefaultSwitchLabelSyntaxNode;
            case "arraytype":
            case "array_type":
               return Node.NodeTypes.ArrayTypeSyntaxNode;
            case "arrayrank":
            case "array_rank":
               return Node.NodeTypes.ArrayRankSpecifierSyntaxNode;
            case "ommittedarraysize":
            case "ommitted_array_size":
               return Node.NodeTypes.OmittedArraySizeExpressionSyntaxNode;
            case "elementaccess":
            case "element_access":
               return Node.NodeTypes.ElementAccessExpressionSyntaxNode;
            case "brackedarglist":
            case "brackedargumentlist":
            case "bracked_arg_list":
            case "bracked_argument_list":
               return Node.NodeTypes.BracketedArgumentListSyntaxNode;
            case "conditionalexpression":
            case "conditional_expression":
               return Node.NodeTypes.ConditionalExpressionSyntaxNode;
            case "unaryexpression":
            case "unary_expression":
               return Node.NodeTypes.PostfixUnaryExpressionSyntaxNode;
            case "continue":
               return Node.NodeTypes.ContinueStatementSyntaxNode;
            case "constructor":
               return Node.NodeTypes.ConstructorDeclarationSyntaxNode;
            case "query":
               return Node.NodeTypes.QueryExpressionSyntaxNode;
            case "from":
               return Node.NodeTypes.FromClauseSyntaxNode;
            case "querybody":
            case "query_body":
               return Node.NodeTypes.QueryBodySyntaxNode;
            case "where":
               return Node.NodeTypes.WhereClauseSyntaxNode;
            case "select":
               return Node.NodeTypes.SelectClauseSyntaxNode;
            case "do":
               return Node.NodeTypes.DoStatementSyntaxNode;
            case "nameequals":
            case "name_equals":
               return Node.NodeTypes.NameEqualsSyntaxNode;
            case "enum":
               return Node.NodeTypes.EnumDeclarationSyntaxNode;
            case "constructorinit":
            case "constructor_init":
            case "constructorinitializer":
            case "constructor_initializer":
               return Node.NodeTypes.ConstructorInitializerSyntaxNode;
            case "empty":
               return Node.NodeTypes.EmptyStatementSyntaxNode;
            case "initexp":
            case "init_exp":
               return Node.NodeTypes.InitializerExpressionSyntaxNode;
            case "await":
               return Node.NodeTypes.AwaitExpressionSyntaxNode;
            case "anonymousobjectcreation":
            case "anonymous_object_creation":
               return Node.NodeTypes.AnonymousObjectCreationExpressionSyntaxNode;
            case "anonymousobjectmemberdeclarator":
            case "anonymous_object_member_declarator":
               return Node.NodeTypes.AnonymousObjectMemberDeclaratorSyntaxNode;
            case "typeparamlist":
            case "type_param_list":
            case "typeparameterlist":
            case "type_parameter_list":
               return Node.NodeTypes.TypeParameterListSyntaxNode;
            case "typeparam":
            case "type_param":
            case "typeparameter":
            case "type_parameter":
               return Node.NodeTypes.TypeParameterSyntaxNode;
            case "defaultexpression":
            case "default_expression":
               return Node.NodeTypes.DefaultExpressionSyntaxNode;
            case "interface":
               return Node.NodeTypes.InterfaceDeclarationSyntaxNode;
            case "cast":
               return Node.NodeTypes.CastExpressionSyntaxNode;
            case "base":
               return Node.NodeTypes.BaseExpressionSyntaxNode;
            case "atttarget":
            case "att_target":
            case "attributetarget":
            case "attribute_target":
               return Node.NodeTypes.AttributeTargetSpecifierSyntaxNode;
            case "alias":
               return Node.NodeTypes.AliasQualifiedNameSyntaxNode;
            case "explicitinterfacespecifier":
            case "explicit_interface_specifier":
               return Node.NodeTypes.ExplicitInterfaceSpecifierSyntaxNode;
            default:
               return Node.NodeTypes.UnknownNode;
         }
      }

      internal static string NodeTypeToKeyword(Node.NodeTypes n)
      {
         switch (n)
         {
            case Node.NodeTypes.UnknownNode:
               return "unknown";
            case Node.NodeTypes.UsingDirectiveSyntaxNode:
               return "usingdirective";
            case Node.NodeTypes.NamespaceDeclarationSyntaxNode:
               return "ns";
            case Node.NodeTypes.ClassDeclarationSyntaxNode:
               return "cls";
            case Node.NodeTypes.IdentifierNameSyntaxNode:
               return "id";
            case Node.NodeTypes.QualifiedNameSyntaxNode:
               return "name";
            case Node.NodeTypes.FieldDeclarationSyntaxNode:
               return "field";
            case Node.NodeTypes.VariableDeclarationSyntaxNode:
               return "vardeclaration";
            case Node.NodeTypes.VariableDeclaratorSyntaxNode:
               return "vardeclarator";
            case Node.NodeTypes.PropertyDeclarationSyntaxNode:
               return "property";
            case Node.NodeTypes.AccessorListSyntaxNode:
               return "accessorlist";
            case Node.NodeTypes.AccessorDeclarationSyntaxNode:
               return "accessordeclaration";
            case Node.NodeTypes.AttributeListSyntaxNode:
               return "attlist";
            case Node.NodeTypes.AttributeSyntaxNode:
               return "attribute";
            case Node.NodeTypes.AttributeArgumentListSyntaxNode:
               return "attarg";
            case Node.NodeTypes.BlockSyntaxNode:
               return "block";
            case Node.NodeTypes.ReturnStatementSyntaxNode:
               return "return";
            case Node.NodeTypes.MethodDeclarationSyntaxNode:
               return "method";
            case Node.NodeTypes.PredefinedTypeSyntaxNode:
               return "predeftype";
            case Node.NodeTypes.ParameterListSyntaxNode:
               return "paramlist";
            case Node.NodeTypes.ExpressionStatementSyntaxNode:
               return "expression";
            case Node.NodeTypes.InvocationExpressionSyntaxNode:
               return "invocation";
            case Node.NodeTypes.ArgumentListSyntaxNode:
               return "arglist";
            case Node.NodeTypes.AssignmentExpressionSyntaxNode:
               return "assign";
            case Node.NodeTypes.MemberAccessExpressionSyntaxNode:
               return "memberaccess";
            case Node.NodeTypes.SwitchStatementSyntaxNode:
               return "switch";
            case Node.NodeTypes.ArgumentSyntaxNode:
               return "arg";
            case Node.NodeTypes.LiteralExpressionSyntaxNode:
               return "literal";
            case Node.NodeTypes.IfStatementSyntaxNode:
               return "if";
            case Node.NodeTypes.PrefixUnaryExpressionSyntaxNode:
               return "prefix";
            case Node.NodeTypes.ParenthesizedExpressionSyntaxNode:
               return "parenthesizedexpression";
            case Node.NodeTypes.BinaryExpressionSyntaxNode:
               return "binaryexpression";
            case Node.NodeTypes.ElseClauseSyntaxNode:
               return "else";
            case Node.NodeTypes.WhileStatementSyntaxNode:
               return "while";
            case Node.NodeTypes.BreakStatementSyntaxNode:
               return "break";
            case Node.NodeTypes.UsingStatementSyntaxNode:
               return "using";
            case Node.NodeTypes.ForStatementSyntaxNode:
               return "for";
            case Node.NodeTypes.LabeledStatementSyntaxNode:
               return "labeledstatement";
            case Node.NodeTypes.BaseListSyntaxNode:
               return "baselist";
            case Node.NodeTypes.SimpleBaseTypeSyntaxNode:
               return "basetype";
            case Node.NodeTypes.GenericNameSyntaxNode:
               return "genericname";
            case Node.NodeTypes.TypeArgumentListSyntaxNode:
               return "typearg";
            case Node.NodeTypes.ParameterSyntaxNode:
               return "param";
            case Node.NodeTypes.LocalDeclarationStatementSyntaxNode:
               return "localdeclaration";
            case Node.NodeTypes.EqualsValueClauseSyntaxNode:
               return "equalsvalue";
            case Node.NodeTypes.ObjectCreationExpressionSyntaxNode:
               return "new";
            case Node.NodeTypes.TypeOfExpressionSyntaxNode:
               return "typeof";
            case Node.NodeTypes.ThrowStatementSyntaxNode:
               return "throw";
            case Node.NodeTypes.ThisExpressionSyntaxNode:
               return "this";
            case Node.NodeTypes.SimpleLambdaExpressionSyntaxNode:
               return "lambda";
            case Node.NodeTypes.ForEachStatementSyntaxNode:
               return "foreach";
            case Node.NodeTypes.TryStatementSyntaxNode:
               return "try";
            case Node.NodeTypes.CatchClauseSyntaxNode:
               return "catch";
            case Node.NodeTypes.SwitchSectionSyntaxNode:
               return "switchselection";
            case Node.NodeTypes.CaseSwitchLabelSyntaxNode:
               return "case";
            case Node.NodeTypes.DefaultSwitchLabelSyntaxNode:
               return "default";
            case Node.NodeTypes.ArrayTypeSyntaxNode:
               return "arraytype";
            case Node.NodeTypes.ArrayRankSpecifierSyntaxNode:
               return "arrayrank";
            case Node.NodeTypes.OmittedArraySizeExpressionSyntaxNode:
               return "ommittedarraysize";
            case Node.NodeTypes.ElementAccessExpressionSyntaxNode:
               return "elementaccess";
            case Node.NodeTypes.BracketedArgumentListSyntaxNode:
               return "brackedarglist";
            case Node.NodeTypes.ConditionalExpressionSyntaxNode:
               return "conditionalexpression";
            case Node.NodeTypes.PostfixUnaryExpressionSyntaxNode:
               return "unaryexpression";
            case Node.NodeTypes.ContinueStatementSyntaxNode:
               return "continue";
            case Node.NodeTypes.ConstructorDeclarationSyntaxNode:
               return "constructor";
            case Node.NodeTypes.QueryExpressionSyntaxNode:
               return "query";
            case Node.NodeTypes.FromClauseSyntaxNode:
               return "from";
            case Node.NodeTypes.QueryBodySyntaxNode:
               return "querybody";
            case Node.NodeTypes.WhereClauseSyntaxNode:
               return "where";
            case Node.NodeTypes.SelectClauseSyntaxNode:
               return "select";
            case Node.NodeTypes.DoStatementSyntaxNode:
               return "do";
            case Node.NodeTypes.NameEqualsSyntaxNode:
               return "nameequals";
            case Node.NodeTypes.EnumDeclarationSyntaxNode:
               return "enum";
            case Node.NodeTypes.AttributeArgumentSyntaxNode:
               return "attarg";
            case Node.NodeTypes.ConstructorInitializerSyntaxNode:
               return "constructorinit";
            case Node.NodeTypes.EmptyStatementSyntaxNode:
               return "empty";
            case Node.NodeTypes.InitializerExpressionSyntaxNode:
               return "initexp";
            case Node.NodeTypes.AwaitExpressionSyntaxNode:
               return "await";
            case Node.NodeTypes.AnonymousObjectCreationExpressionSyntaxNode:
               return "anonymousobjectcreation";
            case Node.NodeTypes.AnonymousObjectMemberDeclaratorSyntaxNode:
               return "anonymousobjectmemberdeclarator";
            case Node.NodeTypes.TypeParameterListSyntaxNode:
               return "typeparamlist";
            case Node.NodeTypes.TypeParameterSyntaxNode:
               return "typeparam";
            case Node.NodeTypes.DefaultExpressionSyntaxNode:
               return "defaultexpression";
            case Node.NodeTypes.InterfaceDeclarationSyntaxNode:
               return "interface";
            case Node.NodeTypes.CastExpressionSyntaxNode:
               return "cast";
            case Node.NodeTypes.BaseExpressionSyntaxNode:
               return "base";
            case Node.NodeTypes.AttributeTargetSpecifierSyntaxNode:
               return "atttarget";
            case Node.NodeTypes.AliasQualifiedNameSyntaxNode:
               return "alias";
            case Node.NodeTypes.ExplicitInterfaceSpecifierSyntaxNode:
               return "explicitinterfacespecifier";
            default:
               throw new Exception("Jag fattar inget.");
         }
      }

   }
}
