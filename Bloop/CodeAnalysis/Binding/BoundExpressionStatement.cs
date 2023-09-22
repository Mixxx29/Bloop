namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpressionNode expression)
        {
            Expression = expression;
        }

        public override BoundNodeType NodeType => BoundNodeType.EXPRESSION_STATEMENT;

        public BoundExpressionNode Expression { get; }
    }
}