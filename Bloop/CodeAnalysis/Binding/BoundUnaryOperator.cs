using Bloop.CodeAnalysis.Symbol;
using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType type, TypeSymbol operandType)
            : this(syntaxType, type, operandType, operandType)
        {
        }

        private BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType type, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            Type = type;
            OperandType = operandType;
            ResultType = resultType;
        }

        public SyntaxType SyntaxType { get; }
        public BoundUnaryOperatorType Type { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        private static BoundUnaryOperator[] _operators =
        {
            new BoundUnaryOperator(SyntaxType.EXCLAMATION_MARK_TOKEN, BoundUnaryOperatorType.LOGIC_NEGATION, TypeSymbol.Bool),
            new BoundUnaryOperator(SyntaxType.PLUS_TOKEN, BoundUnaryOperatorType.IDENTITY, TypeSymbol.Number),
            new BoundUnaryOperator(SyntaxType.MINUS_TOKEN, BoundUnaryOperatorType.NEGATION, TypeSymbol.Number),
        };

        public static BoundUnaryOperator? Bind(SyntaxType syntaxType, TypeSymbol operandType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxType == syntaxType && op.OperandType == operandType)
                    return op;
            }

            return null;
        }
    }
}
