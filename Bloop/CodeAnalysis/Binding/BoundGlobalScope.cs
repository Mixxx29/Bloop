using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope? previous, ImmutableArray<Diagnostic> diagnostics, ImmutableArray<VariableSymbol> variables, BoundExpressionNode expressionNode)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            ExpressionNode = expressionNode;
        }

        public BoundGlobalScope? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundExpressionNode ExpressionNode { get; }
    }
}
