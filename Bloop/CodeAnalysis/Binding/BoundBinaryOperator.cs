using Bloop.CodeAnalysis.Symbol;
using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, TypeSymbol operandType)
            : this(syntaxType, type, operandType, operandType, operandType)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxType, type, operandType, operandType, resultType)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType,
                                    BoundBinaryOperatorType type,
                                    TypeSymbol firstOperandType,
                                    TypeSymbol secondOperandType,
                                    TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            Type = type;
            FirstOperandType = firstOperandType;
            SecondOperandType = secondOperandType;
            ResultType = resultType;
        }


        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxType.PLUS_TOKEN, BoundBinaryOperatorType.ADDITION, TypeSymbol.Number),
            new BoundBinaryOperator(SyntaxType.PLUS_TOKEN, BoundBinaryOperatorType.ADDITION, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxType.PLUS_TOKEN, BoundBinaryOperatorType.ADDITION, TypeSymbol.String, TypeSymbol.Number, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxType.PLUS_TOKEN, BoundBinaryOperatorType.ADDITION, TypeSymbol.Number, TypeSymbol.String, TypeSymbol.String),
            new BoundBinaryOperator(SyntaxType.MINUS_TOKEN, BoundBinaryOperatorType.SUBSTRACTION, TypeSymbol.Number),
            new BoundBinaryOperator(SyntaxType.ASTERIX_TOKEN, BoundBinaryOperatorType.MULTIPLICATION, TypeSymbol.Number),
            new BoundBinaryOperator(SyntaxType.SLASH_TOKEN, BoundBinaryOperatorType.DIVISION, TypeSymbol.Number),
            new BoundBinaryOperator(SyntaxType.MODULO_KEYWORD, BoundBinaryOperatorType.MODULO, TypeSymbol.Number),
            new BoundBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, BoundBinaryOperatorType.EQUALS, TypeSymbol.Number, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN, BoundBinaryOperatorType.NOT_EQUALS, TypeSymbol.Number, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.LESS_THAN_TOKEN, BoundBinaryOperatorType.LESS_THAN, TypeSymbol.Number, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.LESS_THAN_OR_EQUALS_TOKEN, BoundBinaryOperatorType.LESS_THAN_EQUALS, TypeSymbol.Number, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GREATER_THAN_TOKEN, BoundBinaryOperatorType.GREATER_THAN, TypeSymbol.Number, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN, BoundBinaryOperatorType.GREATER_THAN_EQUALS, TypeSymbol.Number, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.DOUBLE_AMPERSAND_TOKEN, BoundBinaryOperatorType.LOGIC_AND, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.DOUBLE_PIPE_TOKEN, BoundBinaryOperatorType.LOGIC_OR, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, BoundBinaryOperatorType.EQUALS, TypeSymbol.Bool, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN, BoundBinaryOperatorType.NOT_EQUALS, TypeSymbol.Bool, TypeSymbol.Bool),
        };

        public SyntaxType SyntaxType { get; }
        public BoundBinaryOperatorType Type { get; }
        public TypeSymbol FirstOperandType { get; }
        public TypeSymbol SecondOperandType { get; }
        public TypeSymbol ResultType { get; }

        public static BoundBinaryOperator? Bind(SyntaxType syntaxType, TypeSymbol firstOperandType, TypeSymbol secondOperandType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxType == syntaxType && op.FirstOperandType == firstOperandType && op.SecondOperandType == secondOperandType)
                    return op;
            }

            return null;
        }
    }
}
