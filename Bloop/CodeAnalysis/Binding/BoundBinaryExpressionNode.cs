namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpressionNode : BoundExpressionNode
    {
        public BoundBinaryExpressionNode(BoundExpressionNode firstOperandNode, BoundBinaryOperator op, BoundExpressionNode secondOperandNode)
        {
            FirstOperandNode = firstOperandNode;
            Op = op;
            SecondOperandNode = secondOperandNode;
        }

        public override BoundNodeType NodeType => BoundNodeType.BINARY_EXPRESSION;
        public override Type Type => Op.ResultType;

        public BoundExpressionNode FirstOperandNode { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpressionNode SecondOperandNode { get; }
    }
}
