using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPresedence(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.PLUS_TOKEN:
                case SyntaxType.MINUS_TOKEN:
                case SyntaxType.EXCLAMATION_MARK_TOKEN:
                    return 5;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPresedence(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.ASTERIX_TOKEN:
                case SyntaxType.SLASH_TOKEN:
                    return 4;

                case SyntaxType.PLUS_TOKEN:
                case SyntaxType.MINUS_TOKEN:
                    return 3;

                case SyntaxType.DOUBLE_AMPERSAND_TOKEN:
                    return 2;

                case SyntaxType.DOUBLE_PIPE_TOKEN:
                    return 1;

                default:
                    return 0;
            }
        }

        public static SyntaxType GetKeywordType(this string text)
        {
            switch (text)
            {
                case "true":
                    return SyntaxType.TRUE_KEYWORD;

                case "false":
                    return SyntaxType.FALSE_KEYWORD;

                default:
                    return SyntaxType.IDENTIFIER_TOKEN;
            }
        }
    }
}