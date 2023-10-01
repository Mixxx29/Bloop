using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(VariableSymbol variable, BoundExpression firstBound, BoundExpression secondBound, BoundStatement statement)
        {
            Variable = variable;
            FirstBound = firstBound;
            SecondBound = secondBound;
            Statement = statement;
        }

        public override BoundNodeType NodeType => BoundNodeType.FOR_STATEMENT;

        public VariableSymbol Variable { get; }
        public BoundExpression FirstBound { get; }
        public BoundExpression SecondBound { get; }
        public BoundStatement Statement { get; }
    }
}