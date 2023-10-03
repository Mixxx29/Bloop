﻿namespace Bloop.CodeAnalysis.Binding
{
    public enum BoundNodeType
    {
        LITERAL_EXPRESSION,
        UNARY_EXPRESSION,
        BINARY_EXPRESSION,
        VARIABLE_EXPRESSION,
        ASSIGNMENT_EXPRESSION,
        FUNCTION_CALL_EXPRESSION,
        ERROR_EXPRESSION,

        MAIN_STATEMENT,
        BLOCK_STATEMENT,
        EXPRESSION_STATEMENT,
        VARIABLE_DECLARATION_STATEMENT,
        IF_STATEMENT,
        ELSE_STATEMENT,
        WHILE_STATEMENT,
        FOR_STATEMENT,
        GOTO_STATEMENT,
        LABEL_STATEMENT,
        CONDITIONAL_GOTO_STATEMENT,
    }
}
