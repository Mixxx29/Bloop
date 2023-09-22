namespace Bloop.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        UNARY_EXPRESSION,
        LITERAL_EXPRESSION,
        BINARY_EXPRESSION,
        VARIABLE_EXPRESSION,
        ASSIGNMENT_EXPRESSION,

        MAIN_STATEMENT,
        BLOCK_STATEMENT,
        EXPRESSION_STATEMENT,
        VARIABLE_DECLARATION_STATEMENT,
    }
}
