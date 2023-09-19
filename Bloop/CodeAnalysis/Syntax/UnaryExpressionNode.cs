namespace Bloop.CodeAnalysis.Syntax
{
    sealed class UnaryExpressionNode : ExpressionNode
    {
        public UnaryExpressionNode(SyntaxToken operatorToken, ExpressionNode operandNode)
        {
            OperatorToken = operatorToken;
            OperandNode = operandNode;
        }

        public override SyntaxType Type => SyntaxType.UNARY_EXPRESSION;

        public SyntaxToken OperatorToken { get; }
        public ExpressionNode OperandNode { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return OperandNode;
        }
    }
}
