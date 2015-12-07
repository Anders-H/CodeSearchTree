using System;
using System.Text.RegularExpressions;

namespace CodeSearchTree
{
    internal sealed class SearchExpressionParser
    {
        public SearchExpressionParser(string source)
        {
            Source = source?.Trim() ?? "";
        }

        private string Source { get; }

        public SearchNodeList Parse()
        {
            var ret = new SearchNodeList();
            if (Source != "")
            {
                var parts = Source.Split('/');
                const string noIndex = @"^(\*|[a-z]+)$";
                const string withIndex = @"^(\*|[a-z]+)\[[0-9]+\]$";
                const string withAttribute = @"^(\*|[a-z]+)\[@.+\]$";
                const string withName = @"^(\*|[a-z]+)\[.+\]$";
                foreach (var part in parts)
                {
                    if (Regex.IsMatch(part, noIndex))
                        ret.Add(new SearchNode(KeywordToEnum(part)));
                    else if (Regex.IsMatch(part, withIndex))
                    {
                        var open = part.IndexOf('[');
                        var close = part.IndexOf(']');
                        var indexString = part.Substring(open + 1, close - (open + 1)).Trim();
                        var index = int.Parse(indexString);
                        ret.Add(new SearchNode(KeywordToEnum(part.Substring(0, open)), index));
                    }
                    else if (Regex.IsMatch(part, withAttribute))
                    {
                        var open = part.IndexOf('[');
                        var close = part.IndexOf(']');
                        var attributeName = part.Substring(open + 2, close - (open + 2)).Trim();
                        ret.Add(new SearchNode(KeywordToEnum(part.Substring(0, open))) { AttributeName = attributeName });
                    }
                    else if (Regex.IsMatch(part, withName))
                    {
                        var open = part.IndexOf('[');
                        var close = part.IndexOf(']');
                        var name = part.Substring(open + 1, close - (open + 1)).Trim();
                        ret.Add(new SearchNode(KeywordToEnum(part.Substring(0, open)), name));
                    }
                    else
                        throw new Exception("Query expression contains errors.");
                }
            }
            return ret;
        }

        private NodeType KeywordToEnum(string keyword)
        {
            switch (keyword) //Ändra här om enumen i filen Node.cs ändras.
            {
                case "*":
                    return NodeType.Any;
                case "usingdirective":
                case "using_directive":
                    return NodeType.UsingDirectiveSyntaxNode;
                case "namespace":
                case "ns":
                    return NodeType.NamespaceDeclarationSyntaxNode;
                case "class":
                case "cls":
                    return NodeType.ClassDeclarationSyntaxNode;
                case "id":
                case "identifier":
                    return NodeType.IdentifierNameSyntaxNode;
                case "name":
                    return NodeType.QualifiedNameSyntaxNode;
                case "field":
                    return NodeType.FieldDeclarationSyntaxNode;
                case "vardeclaration":
                case "variabledeclaration":
                case "var_declaration":
                case "variable_declaration":
                    return NodeType.VariableDeclarationSyntaxNode;
                case "vardeclarator":
                case "variabledeclarator":
                case "var_declarator":
                case "variable_declarator":
                    return NodeType.VariableDeclaratorSyntaxNode;
                case "property":
                    return NodeType.PropertyDeclarationSyntaxNode;
                case "accessorlist":
                case "accessor_list":
                    return NodeType.AccessorListSyntaxNode;
                case "accessordeclaration":
                case "accessor_declaration":
                    return NodeType.AccessorDeclarationSyntaxNode;
                case "attlist":
                case "att_list":
                case "attributelist":
                case "attribute_list":
                    return NodeType.AttributeListSyntaxNode;
                case "attribute":
                    return NodeType.AttributeSyntaxNode;
                case "attarg":
                case "att_arg":
                case "attributeargument":
                case "attribute_argument":
                    return NodeType.AttributeArgumentListSyntaxNode;
                case "block":
                    return NodeType.BlockSyntaxNode;
                case "return":
                    return NodeType.ReturnStatementSyntaxNode;
                case "method":
                    return NodeType.MethodDeclarationSyntaxNode;
                case "predeftype":
                case "pre_def_type":
                case "predefinedtype":
                case "predefined_type":
                    return NodeType.PredefinedTypeSyntaxNode;
                case "paramlist":
                case "parameterlist":
                case "param_list":
                case "parameter_list":
                    return NodeType.ParameterListSyntaxNode;
                case "expression":
                    return NodeType.ExpressionStatementSyntaxNode;
                case "invocation":
                    return NodeType.InvocationExpressionSyntaxNode;
                case "arglist":
                case "argumentlist":
                case "arg_list":
                case "argument_list":
                    return NodeType.ArgumentListSyntaxNode;
                case "assignment":
                case "assign":
                    return NodeType.AssignmentExpressionSyntaxNode;
                case "memberaccess":
                case "membershipaccess":
                case "member_access":
                case "membership_access":
                    return NodeType.MemberAccessExpressionSyntaxNode;
                case "switch":
                    return NodeType.SwitchStatementSyntaxNode;
                case "arg":
                case "argument":
                    return NodeType.ArgumentSyntaxNode;
                case "literal":
                    return NodeType.LiteralExpressionSyntaxNode;
                case "if":
                    return NodeType.IfStatementSyntaxNode;
                case "prefix":
                    return NodeType.PrefixUnaryExpressionSyntaxNode;
                case "parenthesizedexpression":
                case "parenthesized_expression":
                    return NodeType.ParenthesizedExpressionSyntaxNode;
                case "binaryexpression":
                case "binary_expression":
                    return NodeType.BinaryExpressionSyntaxNode;
                case "else":
                    return NodeType.ElseClauseSyntaxNode;
                case "while":
                    return NodeType.WhileStatementSyntaxNode;
                case "break":
                    return NodeType.BreakStatementSyntaxNode;
                case "using":
                    return NodeType.UsingStatementSyntaxNode;
                case "for":
                    return NodeType.ForStatementSyntaxNode;
                case "labeledstatement":
                case "labeled_statement":
                    return NodeType.LabeledStatementSyntaxNode;
                case "baselist":
                case "base_list":
                    return NodeType.BaseListSyntaxNode;
                case "basetype":
                case "base_type":
                    return NodeType.SimpleBaseTypeSyntaxNode;
                case "genericname":
                case "generic_name":
                    return NodeType.GenericNameSyntaxNode;
                case "typearg":
                case "typeargument":
                case "type_arg":
                case "type_argument":
                    return NodeType.TypeArgumentListSyntaxNode;
                case "param":
                case "parameter":
                    return NodeType.ParameterSyntaxNode;
                case "localdeclaration":
                case "local_declaration":
                    return NodeType.LocalDeclarationStatementSyntaxNode;
                case "equalsvalue":
                case "equals_value":
                    return NodeType.EqualsValueClauseSyntaxNode;
                case "new":
                    return NodeType.ObjectCreationExpressionSyntaxNode;
                case "typeof":
                    return NodeType.TypeOfExpressionSyntaxNode;
                case "throw":
                    return NodeType.ThrowStatementSyntaxNode;
                case "this":
                    return NodeType.ThisExpressionSyntaxNode;
                case "lambda":
                    return NodeType.SimpleLambdaExpressionSyntaxNode;
                case "foreach":
                    return NodeType.ForEachStatementSyntaxNode;
                case "try":
                    return NodeType.TryStatementSyntaxNode;
                case "catch":
                    return NodeType.CatchClauseSyntaxNode;
                case "switchselection":
                case "switch_selection":
                    return NodeType.SwitchSectionSyntaxNode;
                case "case":
                    return NodeType.CaseSwitchLabelSyntaxNode;
                case "default":
                    return NodeType.DefaultSwitchLabelSyntaxNode;
                case "arraytype":
                case "array_type":
                    return NodeType.ArrayTypeSyntaxNode;
                case "arrayrank":
                case "array_rank":
                    return NodeType.ArrayRankSpecifierSyntaxNode;
                case "ommittedarraysize":
                case "ommitted_array_size":
                    return NodeType.OmittedArraySizeExpressionSyntaxNode;
                case "elementaccess":
                case "element_access":
                    return NodeType.ElementAccessExpressionSyntaxNode;
                case "brackedarglist":
                case "brackedargumentlist":
                case "bracked_arg_list":
                case "bracked_argument_list":
                    return NodeType.BracketedArgumentListSyntaxNode;
                case "conditionalexpression":
                case "conditional_expression":
                    return NodeType.ConditionalExpressionSyntaxNode;
                case "unaryexpression":
                case "unary_expression":
                    return NodeType.PostfixUnaryExpressionSyntaxNode;
                case "continue":
                    return NodeType.ContinueStatementSyntaxNode;
                case "constructor":
                    return NodeType.ConstructorDeclarationSyntaxNode;
                case "query":
                    return NodeType.QueryExpressionSyntaxNode;
                case "from":
                    return NodeType.FromClauseSyntaxNode;
                case "querybody":
                case "query_body":
                    return NodeType.QueryBodySyntaxNode;
                case "where":
                    return NodeType.WhereClauseSyntaxNode;
                case "select":
                    return NodeType.SelectClauseSyntaxNode;
                case "do":
                    return NodeType.DoStatementSyntaxNode;
                case "nameequals":
                case "name_equals":
                    return NodeType.NameEqualsSyntaxNode;
                case "enum":
                    return NodeType.EnumDeclarationSyntaxNode;
                case "constructorinit":
                case "constructor_init":
                case "constructorinitializer":
                case "constructor_initializer":
                    return NodeType.ConstructorInitializerSyntaxNode;
                case "empty":
                    return NodeType.EmptyStatementSyntaxNode;
                case "initexp":
                case "init_exp":
                    return NodeType.InitializerExpressionSyntaxNode;
                case "await":
                    return NodeType.AwaitExpressionSyntaxNode;
                case "anonymousobjectcreation":
                case "anonymous_object_creation":
                    return NodeType.AnonymousObjectCreationExpressionSyntaxNode;
                case "anonymousobjectmemberdeclarator":
                case "anonymous_object_member_declarator":
                    return NodeType.AnonymousObjectMemberDeclaratorSyntaxNode;
                case "typeparamlist":
                case "type_param_list":
                case "typeparameterlist":
                case "type_parameter_list":
                    return NodeType.TypeParameterListSyntaxNode;
                case "typeparam":
                case "type_param":
                case "typeparameter":
                case "type_parameter":
                    return NodeType.TypeParameterSyntaxNode;
                case "defaultexpression":
                case "default_expression":
                    return NodeType.DefaultExpressionSyntaxNode;
                case "interface":
                    return NodeType.InterfaceDeclarationSyntaxNode;
                case "cast":
                    return NodeType.CastExpressionSyntaxNode;
                case "base":
                    return NodeType.BaseExpressionSyntaxNode;
                case "atttarget":
                case "att_target":
                case "attributetarget":
                case "attribute_target":
                    return NodeType.AttributeTargetSpecifierSyntaxNode;
                case "alias":
                    return NodeType.AliasQualifiedNameSyntaxNode;
                case "explicitinterfacespecifier":
                case "explicit_interface_specifier":
                    return NodeType.ExplicitInterfaceSpecifierSyntaxNode;
                case "catchdeclaration":
                case "catch_declaration":
                    return NodeType.CatchDeclarationSyntaxNode;
                case "arrowexpression":
                case "arrow_expression":
                    return NodeType.ArrowExpressionClauseSyntaxNode;
                case "conditionalaccess":
                case "conditional_access":
                    return NodeType.ConditionalAccessExpressionSyntaxNode;
                case "memberbinding":
                case "member_binding":
                    return NodeType.MemberBindingExpressionSyntaxNode;
                case "interpolatedstring":
                case "interpolated_string":
                    return NodeType.InterpolatedStringExpressionSyntaxNode;
                case "interpolation":
                    return NodeType.InterpolationSyntaxNode;
                case "interpolatedtext":
                case "interpolated_text":
                    return NodeType.InterpolatedStringTextSyntaxNode;
                default:
                    return NodeType.UnknownNode;
            }
        }

        internal static string NodeTypeToKeyword(NodeType n)
        {
            switch (n)
            {
                case NodeType.UnknownNode:
                    return "unknown";
                case NodeType.UsingDirectiveSyntaxNode:
                    return "usingdirective";
                case NodeType.NamespaceDeclarationSyntaxNode:
                    return "ns";
                case NodeType.ClassDeclarationSyntaxNode:
                    return "cls";
                case NodeType.IdentifierNameSyntaxNode:
                    return "id";
                case NodeType.QualifiedNameSyntaxNode:
                    return "name";
                case NodeType.FieldDeclarationSyntaxNode:
                    return "field";
                case NodeType.VariableDeclarationSyntaxNode:
                    return "vardeclaration";
                case NodeType.VariableDeclaratorSyntaxNode:
                    return "vardeclarator";
                case NodeType.PropertyDeclarationSyntaxNode:
                    return "property";
                case NodeType.AccessorListSyntaxNode:
                    return "accessorlist";
                case NodeType.AccessorDeclarationSyntaxNode:
                    return "accessordeclaration";
                case NodeType.AttributeListSyntaxNode:
                    return "attlist";
                case NodeType.AttributeSyntaxNode:
                    return "attribute";
                case NodeType.AttributeArgumentListSyntaxNode:
                    return "attarg";
                case NodeType.BlockSyntaxNode:
                    return "block";
                case NodeType.ReturnStatementSyntaxNode:
                    return "return";
                case NodeType.MethodDeclarationSyntaxNode:
                    return "method";
                case NodeType.PredefinedTypeSyntaxNode:
                    return "predeftype";
                case NodeType.ParameterListSyntaxNode:
                    return "paramlist";
                case NodeType.ExpressionStatementSyntaxNode:
                    return "expression";
                case NodeType.InvocationExpressionSyntaxNode:
                    return "invocation";
                case NodeType.ArgumentListSyntaxNode:
                    return "arglist";
                case NodeType.AssignmentExpressionSyntaxNode:
                    return "assign";
                case NodeType.MemberAccessExpressionSyntaxNode:
                    return "memberaccess";
                case NodeType.SwitchStatementSyntaxNode:
                    return "switch";
                case NodeType.ArgumentSyntaxNode:
                    return "arg";
                case NodeType.LiteralExpressionSyntaxNode:
                    return "literal";
                case NodeType.IfStatementSyntaxNode:
                    return "if";
                case NodeType.PrefixUnaryExpressionSyntaxNode:
                    return "prefix";
                case NodeType.ParenthesizedExpressionSyntaxNode:
                    return "parenthesizedexpression";
                case NodeType.BinaryExpressionSyntaxNode:
                    return "binaryexpression";
                case NodeType.ElseClauseSyntaxNode:
                    return "else";
                case NodeType.WhileStatementSyntaxNode:
                    return "while";
                case NodeType.BreakStatementSyntaxNode:
                    return "break";
                case NodeType.UsingStatementSyntaxNode:
                    return "using";
                case NodeType.ForStatementSyntaxNode:
                    return "for";
                case NodeType.LabeledStatementSyntaxNode:
                    return "labeledstatement";
                case NodeType.BaseListSyntaxNode:
                    return "baselist";
                case NodeType.SimpleBaseTypeSyntaxNode:
                    return "basetype";
                case NodeType.GenericNameSyntaxNode:
                    return "genericname";
                case NodeType.TypeArgumentListSyntaxNode:
                    return "typearg";
                case NodeType.ParameterSyntaxNode:
                    return "param";
                case NodeType.LocalDeclarationStatementSyntaxNode:
                    return "localdeclaration";
                case NodeType.EqualsValueClauseSyntaxNode:
                    return "equalsvalue";
                case NodeType.ObjectCreationExpressionSyntaxNode:
                    return "new";
                case NodeType.TypeOfExpressionSyntaxNode:
                    return "typeof";
                case NodeType.ThrowStatementSyntaxNode:
                    return "throw";
                case NodeType.ThisExpressionSyntaxNode:
                    return "this";
                case NodeType.SimpleLambdaExpressionSyntaxNode:
                    return "lambda";
                case NodeType.ForEachStatementSyntaxNode:
                    return "foreach";
                case NodeType.TryStatementSyntaxNode:
                    return "try";
                case NodeType.CatchClauseSyntaxNode:
                    return "catch";
                case NodeType.SwitchSectionSyntaxNode:
                    return "switchselection";
                case NodeType.CaseSwitchLabelSyntaxNode:
                    return "case";
                case NodeType.DefaultSwitchLabelSyntaxNode:
                    return "default";
                case NodeType.ArrayTypeSyntaxNode:
                    return "arraytype";
                case NodeType.ArrayRankSpecifierSyntaxNode:
                    return "arrayrank";
                case NodeType.OmittedArraySizeExpressionSyntaxNode:
                    return "ommittedarraysize";
                case NodeType.ElementAccessExpressionSyntaxNode:
                    return "elementaccess";
                case NodeType.BracketedArgumentListSyntaxNode:
                    return "brackedarglist";
                case NodeType.ConditionalExpressionSyntaxNode:
                    return "conditionalexpression";
                case NodeType.PostfixUnaryExpressionSyntaxNode:
                    return "unaryexpression";
                case NodeType.ContinueStatementSyntaxNode:
                    return "continue";
                case NodeType.ConstructorDeclarationSyntaxNode:
                    return "constructor";
                case NodeType.QueryExpressionSyntaxNode:
                    return "query";
                case NodeType.FromClauseSyntaxNode:
                    return "from";
                case NodeType.QueryBodySyntaxNode:
                    return "querybody";
                case NodeType.WhereClauseSyntaxNode:
                    return "where";
                case NodeType.SelectClauseSyntaxNode:
                    return "select";
                case NodeType.DoStatementSyntaxNode:
                    return "do";
                case NodeType.NameEqualsSyntaxNode:
                    return "nameequals";
                case NodeType.EnumDeclarationSyntaxNode:
                    return "enum";
                case NodeType.AttributeArgumentSyntaxNode:
                    return "attarg";
                case NodeType.ConstructorInitializerSyntaxNode:
                    return "constructorinit";
                case NodeType.EmptyStatementSyntaxNode:
                    return "empty";
                case NodeType.InitializerExpressionSyntaxNode:
                    return "initexp";
                case NodeType.AwaitExpressionSyntaxNode:
                    return "await";
                case NodeType.AnonymousObjectCreationExpressionSyntaxNode:
                    return "anonymousobjectcreation";
                case NodeType.AnonymousObjectMemberDeclaratorSyntaxNode:
                    return "anonymousobjectmemberdeclarator";
                case NodeType.TypeParameterListSyntaxNode:
                    return "typeparamlist";
                case NodeType.TypeParameterSyntaxNode:
                    return "typeparam";
                case NodeType.DefaultExpressionSyntaxNode:
                    return "defaultexpression";
                case NodeType.InterfaceDeclarationSyntaxNode:
                    return "interface";
                case NodeType.CastExpressionSyntaxNode:
                    return "cast";
                case NodeType.BaseExpressionSyntaxNode:
                    return "base";
                case NodeType.AttributeTargetSpecifierSyntaxNode:
                    return "atttarget";
                case NodeType.AliasQualifiedNameSyntaxNode:
                    return "alias";
                case NodeType.ExplicitInterfaceSpecifierSyntaxNode:
                    return "explicitinterfacespecifier";
                case NodeType.CatchDeclarationSyntaxNode:
                    return "catchdeclaration";
                case NodeType.ArrowExpressionClauseSyntaxNode:
                    return "arrowexpression";
                case NodeType.ConditionalAccessExpressionSyntaxNode:
                    return "conditionalaccess";
                case NodeType.MemberBindingExpressionSyntaxNode:
                    return "memberbinding";
                case NodeType.InterpolatedStringExpressionSyntaxNode:
                    return "interpolatedstring";
                case NodeType.InterpolationSyntaxNode:
                    return "interpolation";
                case NodeType.InterpolatedStringTextSyntaxNode:
                    return "interpolatedtext";
                default:
                    throw new Exception("Jag fattar inget.");
            }
        }
    }
}