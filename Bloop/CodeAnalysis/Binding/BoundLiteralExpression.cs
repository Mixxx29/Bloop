using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;

            if (value is int)
            {
                Type = TypeSymbol.Number;
            }
            else if (value is bool)
            {
                Type = TypeSymbol.Bool;
            }
            else if (value is string)
            {
                Type = TypeSymbol.String;
            }
        }

        public override BoundNodeType NodeType => BoundNodeType.LITERAL_EXPRESSION;
        public override TypeSymbol Type { get; }

        public object Value { get; }
    }
}
