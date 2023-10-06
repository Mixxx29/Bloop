using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    public sealed class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(BoundExpression expression, TypeSymbol targetType)
        {
            Expression = expression;
            TargetType = targetType;
        }

        public override TypeSymbol Type => TargetType;

        public override BoundNodeType NodeType => BoundNodeType.CONVERSION_EXPRESSION;

        public BoundExpression Expression { get; }
        public TypeSymbol TargetType { get; }
    }
}
