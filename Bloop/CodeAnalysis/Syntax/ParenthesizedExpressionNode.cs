namespace Bloop.CodeAnalysis.Syntax
{
    sealed class ParenthesizedExpressionNode : ExpressionSyntax
    {
        public ParenthesizedExpressionNode(SyntaxToken openParenthesisToken, ExpressionSyntax expressionNode, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            ExpressionNode = expressionNode;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax ExpressionNode { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override SyntaxType Type => SyntaxType.PARENTHESIZED_EXPRESSION;
    }
}
