using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
