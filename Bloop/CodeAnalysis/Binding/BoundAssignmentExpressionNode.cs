namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpressionNode : BoundExpression
    {
        public BoundAssignmentExpressionNode(VariableSymbol variable, BoundExpression expressionNode)
        {
            Variable = variable;
            ExpressionNode = expressionNode;
        }

        public override Type Type => Variable.Type;
        public override BoundNodeType NodeType => BoundNodeType.ASSIGNMENT_EXPRESSION;

        public string Name => Variable.Name;
        public VariableSymbol Variable { get; }
        public BoundExpression ExpressionNode { get; }

    }
}
