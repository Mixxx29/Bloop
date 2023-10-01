using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;

        public override BoundNodeType NodeType => BoundNodeType.ERROR_EXPRESSION;
    }
}
