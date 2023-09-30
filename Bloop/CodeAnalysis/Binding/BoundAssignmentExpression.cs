namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expressionNode)
        {
            Variable = variable;
            Expression = expressionNode;
        }

        public override Type Type => Variable.Type;
        public override BoundNodeType NodeType => BoundNodeType.ASSIGNMENT_EXPRESSION;

        public string Name => Variable.Name;
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }

    }
}
