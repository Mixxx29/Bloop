namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public override BoundNodeType NodeType => BoundNodeType.EXPRESSION_STATEMENT;

        public BoundExpression Expression { get; }
    }
}