using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeSearchTree.SearchExpressionTranslationLists
{
    internal class SyntaxNodeToNodeType
    {
        internal static NodeType Translate(SyntaxNode n)
        {
            switch (n)
            {
                case UsingDirectiveSyntax _:
                    return NodeType.UsingDirectiveSyntaxNode;
                case NamespaceDeclarationSyntax _:
                    return NodeType.NamespaceDeclarationSyntaxNode;
                case ClassDeclarationSyntax _:
                    return NodeType.ClassDeclarationSyntaxNode;
                case IdentifierNameSyntax _:
                    return NodeType.IdentifierNameSyntaxNode;
                case QualifiedNameSyntax _:
                    return NodeType.QualifiedNameSyntaxNode;
                case FieldDeclarationSyntax _:
                    return NodeType.FieldDeclarationSyntaxNode;
                case VariableDeclarationSyntax _:
                    return NodeType.VariableDeclarationSyntaxNode;
                case VariableDeclaratorSyntax _:
                    return NodeType.VariableDeclaratorSyntaxNode;
                case PropertyDeclarationSyntax _:
                    return NodeType.PropertyDeclarationSyntaxNode;
                case AccessorListSyntax _:
                    return NodeType.AccessorListSyntaxNode;
                case AccessorDeclarationSyntax _:
                    return NodeType.AccessorDeclarationSyntaxNode;
                case AttributeListSyntax _:
                    return NodeType.AttributeListSyntaxNode;
                case AttributeSyntax _:
                    return NodeType.AttributeSyntaxNode;
                case AttributeArgumentListSyntax _:
                    return NodeType.AttributeArgumentListSyntaxNode;
                case BlockSyntax _:
                    return NodeType.BlockSyntaxNode;
                case ReturnStatementSyntax _:
                    return NodeType.ReturnStatementSyntaxNode;
                case MethodDeclarationSyntax _:
                    return NodeType.MethodDeclarationSyntaxNode;
                case PredefinedTypeSyntax _:
                    return NodeType.PredefinedTypeSyntaxNode;
                case ParameterListSyntax _:
                    return NodeType.ParameterListSyntaxNode;
                case ExpressionStatementSyntax _:
                    return NodeType.ExpressionStatementSyntaxNode;
                case InvocationExpressionSyntax _:
                    return NodeType.InvocationExpressionSyntaxNode;
                case ArgumentListSyntax _:
                    return NodeType.ArgumentListSyntaxNode;
                case AssignmentExpressionSyntax _:
                    return NodeType.AssignmentExpressionSyntaxNode;
                case MemberAccessExpressionSyntax _:
                    return NodeType.MemberAccessExpressionSyntaxNode;
                case SwitchStatementSyntax _:
                    return NodeType.SwitchStatementSyntaxNode;
                case ArgumentSyntax _:
                    return NodeType.ArgumentSyntaxNode;
                case LiteralExpressionSyntax _:
                    return NodeType.LiteralExpressionSyntaxNode;
                case IfStatementSyntax _:
                    return NodeType.IfStatementSyntaxNode;
                case PrefixUnaryExpressionSyntax _:
                    return NodeType.PrefixUnaryExpressionSyntaxNode;
                case ParenthesizedExpressionSyntax _:
                    return NodeType.ParenthesizedExpressionSyntaxNode;
                case BinaryExpressionSyntax _:
                    return NodeType.BinaryExpressionSyntaxNode;
                case ElseClauseSyntax _:
                    return NodeType.ElseClauseSyntaxNode;
                case WhileStatementSyntax _:
                    return NodeType.WhileStatementSyntaxNode;
                case BreakStatementSyntax _:
                    return NodeType.BreakStatementSyntaxNode;
                case UsingStatementSyntax _:
                    return NodeType.UsingStatementSyntaxNode;
                case ForStatementSyntax _:
                    return NodeType.ForStatementSyntaxNode;
                case LabeledStatementSyntax _:
                    return NodeType.LabeledStatementSyntaxNode;
                case BaseListSyntax _:
                    return NodeType.BaseListSyntaxNode;
                case SimpleBaseTypeSyntax _:
                    return NodeType.SimpleBaseTypeSyntaxNode;
                case GenericNameSyntax _:
                    return NodeType.GenericNameSyntaxNode;
                case TypeArgumentListSyntax _:
                    return NodeType.TypeArgumentListSyntaxNode;
                case ParameterSyntax _:
                    return NodeType.ParameterSyntaxNode;
                case LocalDeclarationStatementSyntax _:
                    return NodeType.LocalDeclarationStatementSyntaxNode;
                case EqualsValueClauseSyntax _:
                    return NodeType.EqualsValueClauseSyntaxNode;
                case ObjectCreationExpressionSyntax _:
                    return NodeType.ObjectCreationExpressionSyntaxNode;
                case TypeOfExpressionSyntax _:
                    return NodeType.TypeOfExpressionSyntaxNode;
                case ThrowStatementSyntax _:
                    return NodeType.ThrowStatementSyntaxNode;
                case ThisExpressionSyntax _:
                    return NodeType.ThisExpressionSyntaxNode;
                case SimpleLambdaExpressionSyntax _:
                    return NodeType.SimpleLambdaExpressionSyntaxNode;
                case ForEachStatementSyntax _:
                    return NodeType.ForEachStatementSyntaxNode;
                case TryStatementSyntax _:
                    return NodeType.TryStatementSyntaxNode;
                case CatchClauseSyntax _:
                    return NodeType.CatchClauseSyntaxNode;
                case SwitchSectionSyntax _:
                    return NodeType.SwitchSectionSyntaxNode;
                case CaseSwitchLabelSyntax _:
                    return NodeType.CaseSwitchLabelSyntaxNode;
                case DefaultSwitchLabelSyntax _:
                    return NodeType.DefaultSwitchLabelSyntaxNode;
                case ArrayTypeSyntax _:
                    return NodeType.ArrayTypeSyntaxNode;
                case ArrayRankSpecifierSyntax _:
                    return NodeType.ArrayRankSpecifierSyntaxNode;
                case OmittedArraySizeExpressionSyntax _:
                    return NodeType.OmittedArraySizeExpressionSyntaxNode;
                case ElementAccessExpressionSyntax _:
                    return NodeType.ElementAccessExpressionSyntaxNode;
                case BracketedArgumentListSyntax _:
                    return NodeType.BracketedArgumentListSyntaxNode;
                case ConditionalExpressionSyntax _:
                    return NodeType.ConditionalExpressionSyntaxNode;
                case PostfixUnaryExpressionSyntax _:
                    return NodeType.PostfixUnaryExpressionSyntaxNode;
                case ContinueStatementSyntax _:
                    return NodeType.ContinueStatementSyntaxNode;
                case ConstructorDeclarationSyntax _:
                    return NodeType.ConstructorDeclarationSyntaxNode;
                case QueryExpressionSyntax _:
                    return NodeType.QueryExpressionSyntaxNode;
                case FromClauseSyntax _:
                    return NodeType.FromClauseSyntaxNode;
                case QueryBodySyntax _:
                    return NodeType.QueryBodySyntaxNode;
                case WhereClauseSyntax _:
                    return NodeType.WhereClauseSyntaxNode;
                case SelectClauseSyntax _:
                    return NodeType.SelectClauseSyntaxNode;
                case DoStatementSyntax _:
                    return NodeType.DoStatementSyntaxNode;
                case NameEqualsSyntax _:
                    return NodeType.NameEqualsSyntaxNode;
                case EnumDeclarationSyntax _:
                    return NodeType.EnumDeclarationSyntaxNode;
                case EnumMemberDeclarationSyntax _:
                    return NodeType.EnumMemberDeclarationSyntaxNode;
                case AttributeArgumentSyntax _:
                    return NodeType.AttributeArgumentSyntaxNode;
                case ConstructorInitializerSyntax _:
                    return NodeType.ConstructorInitializerSyntaxNode;
                case EmptyStatementSyntax _:
                    return NodeType.EmptyStatementSyntaxNode;
                case InitializerExpressionSyntax _:
                    return NodeType.InitializerExpressionSyntaxNode;
                case AwaitExpressionSyntax _:
                    return NodeType.AwaitExpressionSyntaxNode;
                case AnonymousObjectCreationExpressionSyntax _:
                    return NodeType.AnonymousObjectCreationExpressionSyntaxNode;
                case AnonymousObjectMemberDeclaratorSyntax _:
                    return NodeType.AnonymousObjectMemberDeclaratorSyntaxNode;
                case TypeParameterListSyntax _:
                    return NodeType.TypeParameterListSyntaxNode;
                case TypeParameterSyntax _:
                    return NodeType.TypeParameterSyntaxNode;
                case DefaultExpressionSyntax _:
                    return NodeType.DefaultExpressionSyntaxNode;
                case InterfaceDeclarationSyntax _:
                    return NodeType.InterfaceDeclarationSyntaxNode;
                case CastExpressionSyntax _:
                    return NodeType.CastExpressionSyntaxNode;
                case BaseExpressionSyntax _:
                    return NodeType.BaseExpressionSyntaxNode;
                case AttributeTargetSpecifierSyntax _:
                    return NodeType.AttributeTargetSpecifierSyntaxNode;
                case AliasQualifiedNameSyntax _:
                    return NodeType.AliasQualifiedNameSyntaxNode;
                case ExplicitInterfaceSpecifierSyntax _:
                    return NodeType.ExplicitInterfaceSpecifierSyntaxNode;
                case CatchDeclarationSyntax _:
                    return NodeType.CatchDeclarationSyntaxNode;
                case ArrowExpressionClauseSyntax _:
                    return NodeType.ArrowExpressionClauseSyntaxNode;
                case ConditionalAccessExpressionSyntax _:
                    return NodeType.ConditionalAccessExpressionSyntaxNode;
                case MemberBindingExpressionSyntax _:
                    return NodeType.MemberBindingExpressionSyntaxNode;
                case InterpolatedStringExpressionSyntax _:
                    return NodeType.InterpolatedStringExpressionSyntaxNode;
                case InterpolationSyntax _:
                    return NodeType.InterpolationSyntaxNode;
                case InterpolatedStringTextSyntax _:
                    return NodeType.InterpolatedStringTextSyntaxNode;
                case GotoStatementSyntax _:
                    return NodeType.GotoStatementSyntaxNode;
                case LockStatementSyntax _:
                    return NodeType.LockStatementSyntaxNode;
                case ArrayCreationExpressionSyntax _:
                    return NodeType.ArrayCreationExpressionSyntaxNode;
                case FinallyClauseSyntax _:
                    return NodeType.FinallyClauseSyntaxNode;
                default:
                    return NodeType.UnknownNode;
            }
        }
    }
}