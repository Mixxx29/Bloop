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
                case BoundMainStatement mainStatement:
                    EvaluateMainStatement(mainStatement);
                    break;

                case BoundBlockStatement blockStatement:
                    EvaluateBlockStatement(blockStatement);
                    break;

                case BoundVariableDeclarationStatement variableDeclarationStatement:
                    EvaluateVariableDeclarationStatement(variableDeclarationStatement);
                    break;

                case BoundExpressionStatement expressionStatement:
                    EvaluateExpressionStatement(expressionStatement);
                    break;

                case BoundIfStatement ifStatement:
                    EvaluateIfStatement(ifStatement);
                    break;

                case BoundElseStatement elseStatement:
                    EvaluateStatement(elseStatement.Statement);
                    break;

                case BoundWhileStatement whileStatement:
                    EvaluateWhileStatement(whileStatement);
                    break;

                case BoundForStatement forStatement:
                    EvaluateForStatement(forStatement);
                    break;

                default:
                    throw new Exception($"Unexpected node {statement.NodeType}");
            }
        }

        private void EvaluateMainStatement(BoundMainStatement mainStatement)
        {
            foreach (var statement in mainStatement.Statements)
                EvaluateStatement(statement);
        }

        private void EvaluateBlockStatement(BoundBlockStatement blockStatement)
        {
            foreach (var statement in blockStatement.Statements)
                EvaluateStatement(statement);
        }

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement variableDeclarationStatement)
        {
            var value = EvaluateExpression(variableDeclarationStatement.Expression);
            _variables[variableDeclarationStatement.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement expressionStatement)
        {
            _lastValue = EvaluateExpression(expressionStatement.Expression);
        }

        private void EvaluateIfStatement(BoundIfStatement ifStatement)
        {
            var condition = (bool)EvaluateExpression(ifStatement.Condition);
            if (condition)
                EvaluateStatement(ifStatement.ThenStatement);
            else
                EvaluateStatement(ifStatement.ElseStatement);
        }

        private void EvaluateWhileStatement(BoundWhileStatement whileStatement)
        {
            while ((bool)EvaluateExpression(whileStatement.Condition))
                EvaluateStatement(whileStatement.Statement);
        }

        private void EvaluateForStatement(BoundForStatement forStatement)
        {
            var firstBound = (int) EvaluateExpression(forStatement.FirstBound);
            var secondBound = (int) EvaluateExpression(forStatement.SecondBound);

            if (firstBound == secondBound)
                return;

            _variables[forStatement.Variable] = firstBound;

            var direction = firstBound < secondBound ? 1 : -1;
            if (direction > 0)
            {
                while (firstBound < secondBound)
                {
                    EvaluateStatement(forStatement.Statement);
                    _variables[forStatement.Variable] = ++firstBound;
                }
            }
            else
            {
                while (firstBound > secondBound)
                {
                    EvaluateStatement(forStatement.Statement);
                    _variables[forStatement.Variable] = --firstBound;
                }
            }
        }

        private object? EvaluateExpression(BoundExpression node)
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

                case BoundBinaryOperatorType.MODULO:
                    return (int?)first % (int?)second;

                case BoundBinaryOperatorType.LOGIC_AND:
                    return (bool)first && (bool)second;

                case BoundBinaryOperatorType.LOGIC_OR:
                    return (bool)first || (bool)second;

                case BoundBinaryOperatorType.EQUALS:
                    return Equals(first, second);

                case BoundBinaryOperatorType.NOT_EQUALS:
                    return !Equals(first, second);

                case BoundBinaryOperatorType.LESS_THAN:
                    return (int?)first < (int?)second;

                case BoundBinaryOperatorType.LESS_THAN_EQUALS:
                    return (int?)first <= (int?)second;

                case BoundBinaryOperatorType.GREATER_THAN:
                    return (int?)first > (int?)second;

                case BoundBinaryOperatorType.GREATER_THAN_EQUALS:
                    return (int?)first >= (int?)second;

                default:
                    throw new Exception($"Unexpected bynary operator {binaryExpression.Op}");
            }
        }
    }
}
