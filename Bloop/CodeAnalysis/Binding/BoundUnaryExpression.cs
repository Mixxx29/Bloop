using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public override BoundNodeType NodeType => BoundNodeType.UNARY_EXPRESSION;
        public override TypeSymbol Type => Operand.Type;

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
    }
}
