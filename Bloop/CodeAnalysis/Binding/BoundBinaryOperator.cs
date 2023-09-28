using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, Type operandType)
            : this(syntaxType, type, operandType, operandType, operandType)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType type, Type operandType, Type resultType)
            : this(syntaxType, type, operandType, operandType, resultType)
        {
        }

        private BoundBinaryOperator(SyntaxType syntaxType,
                                    BoundBinaryOperatorType type, 
                                    Type firstOperandType, 
                                    Type secondOperandType, 
                                    Type resultType)
        {
            SyntaxType = syntaxType;
            Type = type;
            FirstOperandType = firstOperandType;
            SecondOperandType = secondOperandType;
            ResultType = resultType;
        }


        private static BoundBinaryOperator[] _operators =
        {
            new BoundBinaryOperator(SyntaxType.PLUS_TOKEN, BoundBinaryOperatorType.ADDITION, typeof(int)),
            new BoundBinaryOperator(SyntaxType.MINUS_TOKEN, BoundBinaryOperatorType.SUBSTRACTION, typeof(int)),
            new BoundBinaryOperator(SyntaxType.ASTERIX_TOKEN, BoundBinaryOperatorType.MULTIPLICATION, typeof(int)),
            new BoundBinaryOperator(SyntaxType.SLASH_TOKEN, BoundBinaryOperatorType.DIVISION, typeof(int)),
            new BoundBinaryOperator(SyntaxType.MODULO_TOKEN, BoundBinaryOperatorType.MODULO, typeof(int)),
            new BoundBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, BoundBinaryOperatorType.EQUALS, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN, BoundBinaryOperatorType.NOT_EQUALS, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxType.LESS_THAN_TOKEN, BoundBinaryOperatorType.LESS_THAN, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.LESS_THAN_OR_EQUALS_TOKEN, BoundBinaryOperatorType.LESS_THAN_EQUALS, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.GREATER_THAN_TOKEN, BoundBinaryOperatorType.GREATER_THAN, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN, BoundBinaryOperatorType.GREATER_THAN_EQUALS, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxType.DOUBLE_AMPERSAND_TOKEN, BoundBinaryOperatorType.LOGIC_AND, typeof(bool)),
            new BoundBinaryOperator(SyntaxType.DOUBLE_PIPE_TOKEN, BoundBinaryOperatorType.LOGIC_OR, typeof(bool)),
            new BoundBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, BoundBinaryOperatorType.EQUALS, typeof(bool), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN, BoundBinaryOperatorType.NOT_EQUALS, typeof(bool), typeof(bool)),
        };

        public SyntaxType SyntaxType { get; }
        public BoundBinaryOperatorType Type { get; }
        public Type FirstOperandType { get; }
        public Type SecondOperandType { get; }
        public Type ResultType { get; }

        public static BoundBinaryOperator? Bind(SyntaxType syntaxType, Type firstOperandType, Type secondOperandType)
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
