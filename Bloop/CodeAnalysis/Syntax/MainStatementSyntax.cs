using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class MainStatementSyntax : StatementSyntax
    {
        public MainStatementSyntax(ImmutableArray<StatementSyntax> statements)
        {
            Statements = statements;
        }

        public override SyntaxType Type => SyntaxType.MAIN_STATEMENT;

        public ImmutableArray<StatementSyntax> Statements { get; }
    }
}
