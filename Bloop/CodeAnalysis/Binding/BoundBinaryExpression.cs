namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression firstOperandNode, BoundBinaryOperator op, BoundExpression secondOperandNode)
        {
            FirstOperand = firstOperandNode;
            Op = op;
            SecondOperand = secondOperandNode;
        }

        public override BoundNodeType NodeType => BoundNodeType.BINARY_EXPRESSION;
        public override Type Type => Op.ResultType;

        public BoundExpression FirstOperand { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression SecondOperand { get; }
    }
}
