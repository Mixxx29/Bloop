using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }

        public override BoundNodeType NodeType => BoundNodeType.WHILE_STATEMENT;

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }
    }
}