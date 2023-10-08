namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class GlobalStatementSyntax : MemberSyntax
    {
        public GlobalStatementSyntax(StatementSyntax statement)
        {
            Statement = statement;
        }

        public override SyntaxType Type => SyntaxType.GLOBAL_STATEMENT;

        public StatementSyntax Statement { get; }
    }
}
