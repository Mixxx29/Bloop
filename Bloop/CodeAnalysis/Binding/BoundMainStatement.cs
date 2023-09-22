using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundMainStatement : BoundStatement
    {
        public BoundMainStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public override BoundNodeType NodeType => BoundNodeType.MAIN_STATEMENT;

        public ImmutableArray<BoundStatement> Statements { get; }
    }
}