﻿namespace Bloop.CodeAnalysis.Syntax
{
    public enum SyntaxType
    {
        IDENTIFIER_TOKEN,
        NUMBER_TOKEN,
        WHITE_SPACE_TOKEN,
        PLUS_TOKEN,
        MINUS_TOKEN,
        ASTERIX_TOKEN,
        SLASH_TOKEN,
        OPEN_PARENTHESIS_TOKEN,
        CLOSE_PARENTHESIS_TOKEN,
        EXCLAMATION_MARK_TOKEN,
        DOUBLE_AMPERSAND_TOKEN,
        DOUBLE_PIPE_TOKEN,
        END_OF_FILE_TOKEN,
        INVALID_TOKEN,

        TRUE_KEYWORD,
        FALSE_KEYWORD,

        LITERAL_EXPRESSION,
        BINARY_EXPRESSION,
        PARENTHESIZED_EXPRESSION,
        UNARY_EXPRESSION,
    }
}
