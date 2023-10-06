﻿namespace Bloop.CodeAnalysis.Syntax
{
    public enum SyntaxType
    {
        IDENTIFIER_TOKEN,
        FUNCTION_IDENTIFIER_TOKEN,
        NUMBER_TOKEN,
        STRING_TOKEN,
        WHITE_SPACE_TOKEN,
        PLUS_TOKEN,
        MINUS_TOKEN,
        ASTERIX_TOKEN,
        SLASH_TOKEN,
        COLON_TOKEN,
        OPEN_PARENTHESIS_TOKEN,
        CLOSE_PARENTHESIS_TOKEN,
        OPEN_BRACE_TOKEN,
        CLOSE_BRACE_TOKEN,
        EXCLAMATION_MARK_TOKEN,
        DOUBLE_AMPERSAND_TOKEN,
        DOUBLE_PIPE_TOKEN,
        EQUALS_TOKEN,
        DOUBLE_EQUALS_TOKEN,
        EXCLAMATION_MARK_EQUALS_TOKEN,
        LESS_THAN_TOKEN,
        LESS_THAN_OR_EQUALS_TOKEN,
        GREATER_THAN_TOKEN,
        GREATER_THAN_OR_EQUALS_TOKEN,
        COMMA_TOKEN,
        END_OF_FILE_TOKEN,
        INVALID_TOKEN,

        TRUE_KEYWORD,
        FALSE_KEYWORD,
        MODULO_KEYWORD,
        VAR_KEYWORD,
        CONST_KEYWORD,
        NUMBER_KEYWORD,
        STRING_KEYWORD,
        BOOL_KEYWORD,
        AS_KEYWORD,
        IF_KEYWORD,
        ELSE_KEYWORD,
        WHILE_KEYWORD,
        FOR_KEYWORD,
        TO_KEYWORD,

        MISSING_EXPRESSION,
        NAME_EXPRESSION,
        ASSIGNMENT_EXPRESSION,
        LITERAL_EXPRESSION,
        BINARY_EXPRESSION,
        PARENTHESIZED_EXPRESSION,
        UNARY_EXPRESSION,
        FUNCTION_CALL_EXPRESSION,
        CONVERSION_EXPRESSION,

        MAIN_STATEMENT,
        BLOCK_STATEMENT,
        EXPRESSION_STATEMENT,
        VARIABLE_DECLARATION_STATEMENT,
        IF_STATEMENT,
        ELSE_STATEMENT,
        WHILE_STATEMENT,
        FOR_STATEMENT,

        COMPILATION_UNIT,
        TYPE_CLAUSE,
    }
}
