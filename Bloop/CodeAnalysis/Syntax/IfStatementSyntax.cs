namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, StatementSyntax statement, ElseStatementSyntax? elseStatement)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            Statement = statement;
            ElseStatement = elseStatement;
        }

        public override SyntaxType Type => SyntaxType.IF_STATEMENT;

        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Statement { get; }
        public ElseStatementSyntax? ElseStatement { get; }
    }
}
