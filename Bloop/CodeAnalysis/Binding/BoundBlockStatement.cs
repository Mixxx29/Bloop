using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public override BoundNodeType NodeType => BoundNodeType.BLOCK_STATEMENT;

        public ImmutableArray<BoundStatement> Statements { get; }
    }
}