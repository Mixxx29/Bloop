namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpressionNode : BoundExpressionNode
    {
        public BoundUnaryExpressionNode(BoundUnaryOperatorType operatorType, BoundExpressionNode operand)
        {
            OperatorType = operatorType;
            Operand = operand;
        }

        public override BoundNodeType NodeType => BoundNodeType.UNARY_EXPRESSION;
        public override Type Type => Operand.Type;

        public BoundUnaryOperatorType OperatorType { get; }
        public BoundExpressionNode Operand { get; }
    }
}
