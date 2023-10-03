using Bloop.CodeAnalysis.Symbol;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundFunctionCallExpression : BoundExpression
    {
        public BoundFunctionCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public override TypeSymbol Type => Function.Type;

        public override BoundNodeType NodeType => BoundNodeType.FUNCTION_CALL_EXPRESSION;

        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }
    }
}
