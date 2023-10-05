using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    public abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
