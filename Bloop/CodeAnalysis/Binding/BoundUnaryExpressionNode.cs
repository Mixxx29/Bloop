namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpressionNode : BoundExpressionNode
    {
        public BoundUnaryExpressionNode(BoundUnaryOperator op, BoundExpressionNode operand)
        {
            Op = op;
            Operand = operand;
        }

        public override BoundNodeType NodeType => BoundNodeType.UNARY_EXPRESSION;
        public override Type Type => Operand.Type;

        public BoundUnaryOperator Op { get; }
        public BoundExpressionNode Operand { get; }
    }
}
