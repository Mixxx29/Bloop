namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpressionNode : BoundExpression
    {
        public BoundUnaryExpressionNode(BoundUnaryOperator op, BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public override BoundNodeType NodeType => BoundNodeType.UNARY_EXPRESSION;
        public override Type Type => Operand.Type;

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
    }
}
