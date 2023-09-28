﻿namespace Bloop.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        MISSING_EXPRESSION,
        UNARY_EXPRESSION,
        LITERAL_EXPRESSION,
        BINARY_EXPRESSION,
        VARIABLE_EXPRESSION,
        ASSIGNMENT_EXPRESSION,

        MAIN_STATEMENT,
        BLOCK_STATEMENT,
        EXPRESSION_STATEMENT,
        VARIABLE_DECLARATION_STATEMENT,
        IF_STATEMENT,
        ELSE_STATEMENT,
        WHILE_STATEMENT,
        FOR_STATEMENT,
    }
}
