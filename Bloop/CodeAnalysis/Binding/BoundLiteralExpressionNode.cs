namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpressionNode : BoundExpression
    {
        public BoundLiteralExpressionNode(object value)
        {
            Value = value;
        }

        public override BoundNodeType NodeType => BoundNodeType.LITERAL_EXPRESSION;
        public override Type Type => Value.GetType();

        public object Value { get; }
    }
}
