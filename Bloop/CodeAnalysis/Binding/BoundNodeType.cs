namespace Bloop.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        UNARY_EXPRESSION,
        LITERAL_EXPRESSION,
        BINARY_EXPRESSION,
        VARIABLE_EXPRESSION,
        ASSIGNMENT_EXPRESSION,

        BLOCK_STATEMENT,
        EXPRESSION_STATEMENT,
    }
}
