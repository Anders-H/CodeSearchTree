namespace CodeSearchTree
{
    public interface ITypedChild
    {
        TypedSearchNode Arg { get; }
        TypedSearchNode ArgList { get; }
        TypedSearchNode Assign { get; }
        TypedSearchNode BaseList { get; }
        TypedSearchNode BaseType { get; }
        TypedSearchNode Block { get; }
        TypedSearchNode Cls { get; }
        TypedSearchNode Constructor { get; }
        TypedSearchNode EqualsValue { get; }
        TypedSearchNode Expression { get; }
        TypedSearchNode Field { get; }
        TypedSearchNode Id { get; }
        TypedSearchNode If { get; }
        TypedSearchNode Invocation { get; }
        TypedSearchNode Literal { get; }
        TypedSearchNode MemberAccess { get; }
        TypedSearchNode Method { get; }
        TypedSearchNode New { get; }
        TypedSearchNode Ns { get; }
        TypedSearchNode Property { get; }
        TypedSearchNode Try { get; }
        TypedSearchNode UsingDirective { get; }
        TypedSearchNode VarDeclaration { get; }
        TypedSearchNode VarDeclarator { get; }
    }
}
