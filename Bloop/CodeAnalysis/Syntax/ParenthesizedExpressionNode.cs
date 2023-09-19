namespace Bloop.CodeAnalysis.Syntax
{
    sealed class ParenthesizedExpressionNode : ExpressionNode
    {
        public ParenthesizedExpressionNode(SyntaxToken openParenthesisToken, ExpressionNode expressionNode, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            ExpressionNode = expressionNode;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionNode ExpressionNode { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override SyntaxType Type => SyntaxType.PARENTHESIZED_EXPRESSION;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return ExpressionNode;
            yield return CloseParenthesisToken;
        }
    }
}
