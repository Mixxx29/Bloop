using Bloop.CodeAnalysis.Binding;

namespace Bloop.CodeAnalysis
{

    class Evaluator
    {
        private readonly BoundExpressionNode _node;
        private readonly Dictionary<VariableSymbol, object?> _variables;

        public Evaluator(BoundExpressionNode node, Dictionary<VariableSymbol, object?> variables)
        {
            _node = node;
            _variables = variables;
        }

        public object? Evaluate()
        {
            return EvaluateExpression(_node);
        }

        private object? EvaluateExpression(BoundExpressionNode node)
        {
            if (node is BoundLiteralExpressionNode literalExpression)
                return literalExpression.Value;

            if (node is BoundVariableExpressionNode variableExpression)
            {
                return _variables[variableExpression.Variable];
            }

            if (node is BoundAssignmentExpressionNode assignmentExpression)
            {
                var value = EvaluateExpression(assignmentExpression.ExpressionNode);
                _variables[assignmentExpression.Variable] = value;
                return value;
            }

            if (node is BoundUnaryExpressionNode unaryExpression)
            {
                var operand = EvaluateExpression(unaryExpression.Operand);

                switch (unaryExpression.Op.Type)
                {
                    case BoundUnaryOperatorType.IDENTITY:
                        return (int?) operand;

                    case BoundUnaryOperatorType.NEGATION:
                        return -(int?) operand;

                    case BoundUnaryOperatorType.LOGIC_NEGATION:
                        return !(bool?) operand;

                    default:
                        throw new Exception($"Unexpected unary operator {unaryExpression.Op}");
                }
            }

            if (node is BoundBinaryExpressionNode binaryExpression)
            {
                var first = EvaluateExpression(binaryExpression.FirstOperandNode);
                var second = EvaluateExpression(binaryExpression.SecondOperandNode);

                switch (binaryExpression.Op.Type)
                {
                    case BoundBinaryOperatorType.ADDITION:
                        return (int?) first + (int?) second;

                    case BoundBinaryOperatorType.SUBSTRACTION:
                        return (int?) first - (int?) second;

                    case BoundBinaryOperatorType.MULTIPLICATION:
                        return (int?) first * (int?) second;

                    case BoundBinaryOperatorType.DIVISION:
                        return (int?) first / (int?) second;

                    case BoundBinaryOperatorType.LOGIC_AND:
                        return (bool) first && (bool) second;

                    case BoundBinaryOperatorType.LOGIC_OR:
                        return (bool) first || (bool) second;

                    case BoundBinaryOperatorType.EQUALS:
                        return Equals(first, second);

                    case BoundBinaryOperatorType.NOT_EQUALS:
                        return !Equals(first, second);

                    default:
                        throw new Exception($"Unexpected bynary operator {binaryExpression.Op}");
                }
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}
