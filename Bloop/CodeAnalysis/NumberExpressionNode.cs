namespace Bloop.CodeAnalysis
{
    sealed class NumberExpressionNode : ExpressionNode
    {
        public NumberExpressionNode(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override TokenType Type => TokenType.NUMBER_EXPRESSION;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}
