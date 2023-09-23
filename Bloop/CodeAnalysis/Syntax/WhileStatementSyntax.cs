namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxToken whileKeyword, ExpressionSyntax condition, StatementSyntax statement)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            Statement = statement;
        }

        public override SyntaxType Type => SyntaxType.WHILE_STATEMENT;

        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Statement { get; }
    }

}
