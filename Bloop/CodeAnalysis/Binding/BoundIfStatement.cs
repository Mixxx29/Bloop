namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement thenStatement, BoundStatement? elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public override BoundNodeType NodeType => BoundNodeType.IF_STATEMENT;

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement? ElseStatement { get; }
    }

    internal sealed class BoundElseStatement : BoundStatement
    {
        public BoundElseStatement(BoundStatement statement)
        {
            Statement = statement;
        }

        public override BoundNodeType NodeType => BoundNodeType.ELSE_STATEMENT;

        public BoundStatement Statement { get; }
    }
}