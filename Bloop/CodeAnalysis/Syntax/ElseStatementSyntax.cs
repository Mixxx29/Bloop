namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class ElseStatementSyntax : StatementSyntax
    {
        public ElseStatementSyntax(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            ElseKeyword = elseKeyword;
            Statement = statement;
        }

        public override SyntaxType Type => SyntaxType.ELSE_STATEMENT;

        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax Statement { get; }
    }
}
