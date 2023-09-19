namespace Bloop.CodeAnalysis.Syntax
{
    sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax expressionSyntax)
        {
            OperatorToken = operatorToken;
            ExpressionSyntax = expressionSyntax;
        }

        public override SyntaxType Type => SyntaxType.UNARY_EXPRESSION;

        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax ExpressionSyntax { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return ExpressionSyntax;
        }
    }
}
