namespace Bloop.CodeAnalysis.Syntax
{
    sealed class UnaryExpressionNode : ExpressionSyntax
    {
        public UnaryExpressionNode(SyntaxToken operatorToken, ExpressionSyntax operandNode)
        {
            OperatorToken = operatorToken;
            OperandNode = operandNode;
        }

        public override SyntaxType Type => SyntaxType.UNARY_EXPRESSION;

        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax OperandNode { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return OperandNode;
        }
    }
}
