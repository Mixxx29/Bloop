namespace Bloop.CodeAnalysis
{
    sealed class BinaryExpressionNode : ExpressionNode
    {
        public BinaryExpressionNode(ExpressionNode firstNode, SyntaxToken operatorToken, ExpressionNode secondNode)
        {
            FirstNode = firstNode;
            OperatorToken = operatorToken;
            SecondNode = secondNode;
        }

        public override TokenType Type => TokenType.BINARY_EXPRESSION;

        public ExpressionNode FirstNode { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionNode SecondNode { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return FirstNode;
            yield return OperatorToken;
            yield return SecondNode;
        }
    }
}
