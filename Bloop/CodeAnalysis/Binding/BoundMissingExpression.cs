namespace Bloop.CodeAnalysis.Binding
{
    internal class BoundMissingExpression : BoundExpression
    {
        public override Type Type => typeof(object);

        public override BoundNodeType NodeType => BoundNodeType.MISSING_EXPRESSION;
    }
}