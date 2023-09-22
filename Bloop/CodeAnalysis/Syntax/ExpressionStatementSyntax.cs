namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override SyntaxType Type => SyntaxType.EXPRESSION_STATEMENT;

        public ExpressionSyntax Expression { get; }
    }
}
