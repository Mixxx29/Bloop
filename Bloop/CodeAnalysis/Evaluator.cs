using Bloop.CodeAnalysis.Binding;

namespace Bloop.CodeAnalysis
{

    class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object?> _variables;

        private object? _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object?> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object? Evaluate()
        {
            EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement statement)
        {
            switch (statement)
            {
                case BoundBlockStatement blockStatement:
                    EvaluateBlockStatement(blockStatement);
                    break;
                case BoundExpressionStatement expressionStatement:
                    EvaluateExpressionStatement(expressionStatement);
                    break;
                default:
                    throw new Exception($"Unexpected node {statement.NodeType}");
            }
        }

        private void EvaluateBlockStatement(BoundBlockStatement blockStatement)
        {
            foreach (var statement in blockStatement.Statements)
                EvaluateStatement(statement);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement expressionStatement)
        {
            _lastValue = EvaluateExpression(expressionStatement.Expression);
        }

        private object? EvaluateExpression(BoundExpressionNode node)
        {
            switch (node)
            {
                case BoundLiteralExpressionNode literalExpression:
                    return EvaluateLiteralExpression(literalExpression);
                case BoundVariableExpressionNode variableExpression:
                    return EvaluateVariableExpression(variableExpression);
                case BoundAssignmentExpressionNode assignmentExpression:
                    return EvaluateAssignmentExpression(assignmentExpression);
                case BoundUnaryExpressionNode unaryExpression:
                    return EvaluateUnaryExpression(unaryExpression);
                case BoundBinaryExpressionNode binaryExpression:
                    return EvaluateBinaryExpression(binaryExpression);
                default:
                    throw new Exception($"Unexpected node {node.Type}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpressionNode literalExpression)
        {
            return literalExpression.Value;
        }

        private object? EvaluateVariableExpression(BoundVariableExpressionNode variableExpression)
        {
            return _variables[variableExpression.Variable];
        }

        private object? EvaluateAssignmentExpression(BoundAssignmentExpressionNode assignmentExpression)
        {
            var value = EvaluateExpression(assignmentExpression.ExpressionNode);
            _variables[assignmentExpression.Variable] = value;
            return value;
        }

        private object? EvaluateUnaryExpression(BoundUnaryExpressionNode unaryExpression)
        {
            var operand = EvaluateExpression(unaryExpression.Operand);

            switch (unaryExpression.Op.Type)
            {
                case BoundUnaryOperatorType.IDENTITY:
                    return (int?)operand;

                case BoundUnaryOperatorType.NEGATION:
                    return -(int?)operand;

                case BoundUnaryOperatorType.LOGIC_NEGATION:
                    return !(bool?)operand;

                default:
                    throw new Exception($"Unexpected unary operator {unaryExpression.Op}");
            }
        }

        private object? EvaluateBinaryExpression(BoundBinaryExpressionNode binaryExpression)
        {
            var first = EvaluateExpression(binaryExpression.FirstOperandNode);
            var second = EvaluateExpression(binaryExpression.SecondOperandNode);

            switch (binaryExpression.Op.Type)
            {
                case BoundBinaryOperatorType.ADDITION:
                    return (int?)first + (int?)second;

                case BoundBinaryOperatorType.SUBSTRACTION:
                    return (int?)first - (int?)second;

                case BoundBinaryOperatorType.MULTIPLICATION:
                    return (int?)first * (int?)second;

                case BoundBinaryOperatorType.DIVISION:
                    return (int?)first / (int?)second;

                case BoundBinaryOperatorType.LOGIC_AND:
                    return (bool)first && (bool)second;

                case BoundBinaryOperatorType.LOGIC_OR:
                    return (bool)first || (bool)second;

                case BoundBinaryOperatorType.EQUALS:
                    return Equals(first, second);

                case BoundBinaryOperatorType.NOT_EQUALS:
                    return !Equals(first, second);

                default:
                    throw new Exception($"Unexpected bynary operator {binaryExpression.Op}");
            }
        }
    }
}
