namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpressionNode : BoundExpressionNode
    {
        public BoundAssignmentExpressionNode(VariableSymbol variable, BoundExpressionNode expressionNode)
        {
            Variable = variable;
            ExpressionNode = expressionNode;
        }

        public override Type Type => Variable.Type;
        public override BoundNodeType NodeType => BoundNodeType.ASSIGNMENT_EXPRESSION;

        public string Name => Variable.Name;
        public VariableSymbol Variable { get; }
        public BoundExpressionNode ExpressionNode { get; }

    }
}
