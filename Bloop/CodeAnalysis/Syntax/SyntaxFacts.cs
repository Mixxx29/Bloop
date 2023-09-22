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
                    return 6;

                default:
                    return 0;
            }
        }

        public static IEnumerable<SyntaxType> GetUnaryOperatorTypes()
        {
            var types = (SyntaxType[]) Enum.GetValues(typeof(SyntaxType));
            foreach (var type in types)
            {
                if (type.GetUnaryOperatorPresedence() > 0)
                    yield return type;
            }
        }

        public static int GetBinaryOperatorPresedence(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.ASTERIX_TOKEN:
                case SyntaxType.SLASH_TOKEN:
                    return 5;

                case SyntaxType.PLUS_TOKEN:
                case SyntaxType.MINUS_TOKEN:
                    return 4;

                case SyntaxType.DOUBLE_EQUALS_TOKEN:
                case SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN:
                    return 3;

                case SyntaxType.DOUBLE_AMPERSAND_TOKEN:
                    return 2;

                case SyntaxType.DOUBLE_PIPE_TOKEN:
                    return 1;

                default:
                    return 0;
            }
        }

        public static IEnumerable<SyntaxType> GetBinaryOperatorTypes()
        {
            var types = (SyntaxType[]) Enum.GetValues(typeof(SyntaxType));
            foreach (var type in types)
            {
                if (type.GetBinaryOperatorPresedence() > 0)
                    yield return type;
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

        public static string? GetText(this SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.PLUS_TOKEN:
                    return "+";

                case SyntaxType.MINUS_TOKEN:
                    return "-";

                case SyntaxType.ASTERIX_TOKEN:
                    return "*";

                case SyntaxType.SLASH_TOKEN:
                    return "/";

                case SyntaxType.OPEN_PARENTHESIS_TOKEN:
                    return "(";

                case SyntaxType.CLOSE_PARENTHESIS_TOKEN:
                    return ")";

                case SyntaxType.OPEN_BRACE_TOKEN:
                    return "{";

                case SyntaxType.CLOSE_BRACE_TOKEN:
                    return "}";

                case SyntaxType.EXCLAMATION_MARK_TOKEN:
                    return "!";

                case SyntaxType.DOUBLE_AMPERSAND_TOKEN:
                    return "&&";

                case SyntaxType.DOUBLE_PIPE_TOKEN:
                    return "||";

                case SyntaxType.EQUALS_TOKEN:
                    return "=";

                case SyntaxType.DOUBLE_EQUALS_TOKEN:
                    return "==";

                case SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN:
                    return "!=";

                case SyntaxType.TRUE_KEYWORD:
                    return "true";

                case SyntaxType.FALSE_KEYWORD:
                    return "false";
            }
            return null;
        }
    }
}