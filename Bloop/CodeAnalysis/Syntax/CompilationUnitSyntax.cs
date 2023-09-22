namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    { 
        public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken)
        {
            Statement = statement;
            EndOfFileToken = endOfFileToken;
        }
        public override SyntaxType Type => SyntaxType.COMPILATION_UNIT;

        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }

    }
}
