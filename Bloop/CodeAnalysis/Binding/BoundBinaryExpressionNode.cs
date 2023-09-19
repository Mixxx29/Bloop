namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpressionNode : BoundExpressionNode
    {
        public BoundBinaryExpressionNode(BoundExpressionNode firstOperandNode, BoundBinaryOperatorType operatorType, BoundExpressionNode secondOperandNode)
        {
            FirstOperandNode = firstOperandNode;
            OperatorType = operatorType;
            SecondOperandNode = secondOperandNode;
        }

        public override BoundNodeType NodeType => BoundNodeType.BINARY_EXPRESSION;
        public override Type Type => FirstOperandNode.Type;

        public BoundExpressionNode FirstOperandNode { get; }
        public BoundBinaryOperatorType OperatorType { get; }
        public BoundExpressionNode SecondOperandNode { get; }
    }
}
