namespace Bloop.CodeAnalysis.Syntax
{
    sealed class LiteralExpressionNode : ExpressionSyntax
    {
        public LiteralExpressionNode(SyntaxToken literalToken, object? value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public LiteralExpressionNode(SyntaxToken literalToken) 
            : this(literalToken, literalToken.Value)
        {
             
        }

        public override SyntaxType Type => SyntaxType.LITERAL_EXPRESSION;

        public SyntaxToken LiteralToken { get; }
        public object? Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}
