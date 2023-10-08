using Bloop.CodeAnalysis.Symbol;
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
                case SyntaxType.MODULO_KEYWORD:
                    return 5;

                case SyntaxType.PLUS_TOKEN:
                case SyntaxType.MINUS_TOKEN:
                    return 4;

                case SyntaxType.DOUBLE_EQUALS_TOKEN:
                case SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN:
                case SyntaxType.LESS_THAN_TOKEN:
                case SyntaxType.LESS_THAN_OR_EQUALS_TOKEN:
                case SyntaxType.GREATER_THAN_TOKEN:
                case SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN:
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
                case "":
                    return SyntaxType.INVALID_TOKEN;

                case "true":
                    return SyntaxType.TRUE_KEYWORD;

                case "false":
                    return SyntaxType.FALSE_KEYWORD;
                
                case "function":
                    return SyntaxType.FUNCTION_KEYWORD;

                case "var":
                    return SyntaxType.VAR_KEYWORD;

                case "const":
                    return SyntaxType.CONST_KEYWORD;

                case "number":
                    return SyntaxType.NUMBER_KEYWORD;

                case "string":
                    return SyntaxType.STRING_KEYWORD;

                case "bool":
                    return SyntaxType.BOOL_KEYWORD;

                case "as":
                    return SyntaxType.AS_KEYWORD;

                case "if":
                    return SyntaxType.IF_KEYWORD;

                case "else":
                    return SyntaxType.ELSE_KEYWORD;

                case "while":
                    return SyntaxType.WHILE_KEYWORD;

                case "for":
                    return SyntaxType.FOR_KEYWORD;

                case "to":
                    return SyntaxType.TO_KEYWORD;

                case "mod":
                    return SyntaxType.MODULO_KEYWORD;

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

                case SyntaxType.COLON_TOKEN:
                    return ":";

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

                case SyntaxType.LESS_THAN_TOKEN:
                    return "<";

                case SyntaxType.LESS_THAN_OR_EQUALS_TOKEN:
                    return "<=";

                case SyntaxType.GREATER_THAN_TOKEN:
                    return ">";

                case SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN:
                    return ">=";

                case SyntaxType.TRUE_KEYWORD:
                    return "true";

                case SyntaxType.FALSE_KEYWORD:
                    return "false";

                case SyntaxType.FUNCTION_KEYWORD:
                    return "function";

                case SyntaxType.VAR_KEYWORD:
                    return "var";

                case SyntaxType.CONST_KEYWORD:
                    return "const";

                case SyntaxType.NUMBER_KEYWORD:
                    return "number";

                case SyntaxType.STRING_KEYWORD:
                    return "string";

                case SyntaxType.BOOL_KEYWORD:
                    return "bool";

                case SyntaxType.AS_KEYWORD:
                    return "as";

                case SyntaxType.IF_KEYWORD:
                    return "if";

                case SyntaxType.ELSE_KEYWORD:
                    return "else";

                case SyntaxType.WHILE_KEYWORD:
                    return "while";

                case SyntaxType.FOR_KEYWORD:
                    return "for";

                case SyntaxType.TO_KEYWORD:
                    return "to";

                case SyntaxType.MODULO_KEYWORD:
                    return "mod";
            }
            return null;
        }

        public static TypeSymbol? GetTypeSymbol(this SyntaxType syntaxType)
        {
            switch (syntaxType)
            {
                case SyntaxType.NUMBER_KEYWORD:
                    return TypeSymbol.Number;

                case SyntaxType.STRING_KEYWORD:
                    return TypeSymbol.String;

                case SyntaxType.BOOL_KEYWORD:
                    return TypeSymbol.Bool;

                default:
                    return null;
            }
        }

        public static ConsoleColor GetColor(SyntaxType type)
        {
            switch (type)
            {
                case SyntaxType.FUNCTION_IDENTIFIER_TOKEN:
                    return ConsoleColor.DarkYellow;

                case SyntaxType.STRING_TOKEN:
                    return ConsoleColor.Green;

                case SyntaxType.VAR_KEYWORD:
                case SyntaxType.CONST_KEYWORD:
                case SyntaxType.NUMBER_KEYWORD:
                case SyntaxType.STRING_KEYWORD:
                case SyntaxType.BOOL_KEYWORD:
                case SyntaxType.FUNCTION_KEYWORD:
                    return ConsoleColor.DarkCyan;

                case SyntaxType.TRUE_KEYWORD:
                case SyntaxType.FALSE_KEYWORD:
                case SyntaxType.NUMBER_TOKEN:
                    return ConsoleColor.Cyan;

                case SyntaxType.IF_KEYWORD:
                case SyntaxType.ELSE_KEYWORD:
                case SyntaxType.WHILE_KEYWORD:
                case SyntaxType.FOR_KEYWORD:
                case SyntaxType.TO_KEYWORD:
                case SyntaxType.AS_KEYWORD:
                    return ConsoleColor.Magenta;

                case SyntaxType.EQUALS_TOKEN:
                case SyntaxType.DOUBLE_EQUALS_TOKEN:
                case SyntaxType.PLUS_TOKEN:
                case SyntaxType.MINUS_TOKEN:
                case SyntaxType.ASTERIX_TOKEN:
                case SyntaxType.SLASH_TOKEN:
                case SyntaxType.COLON_TOKEN:
                case SyntaxType.MODULO_KEYWORD:
                case SyntaxType.GREATER_THAN_TOKEN:
                case SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN:
                case SyntaxType.LESS_THAN_TOKEN:
                case SyntaxType.LESS_THAN_OR_EQUALS_TOKEN:
                case SyntaxType.EXCLAMATION_MARK_TOKEN:
                case SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN:
                case SyntaxType.DOUBLE_AMPERSAND_TOKEN:
                case SyntaxType.DOUBLE_PIPE_TOKEN:
                case SyntaxType.COMMA_TOKEN:
                    return ConsoleColor.Yellow;

                default:
                    return ConsoleColor.White;
            }
        }
    }
}