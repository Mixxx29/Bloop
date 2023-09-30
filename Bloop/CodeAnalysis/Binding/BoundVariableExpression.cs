using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeType NodeType => BoundNodeType.VARIABLE_EXPRESSION;

        public VariableSymbol Variable { get; }
        public string Name => Variable.Name;
        public override Type Type => Variable.Type;
    }
}
