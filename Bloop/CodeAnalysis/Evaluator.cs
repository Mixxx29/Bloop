using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Symbol;
using System;

namespace Bloop.CodeAnalysis
{

    class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object?> _variables;

        private object? _lastValue;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object?> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object? Evaluate()
        {
            var labelToIndex = new Dictionary<LabelSymbol, int>();

            for (var i = 0; i < _root.Statements.Length; i++)
            {
                if (_root.Statements[i] is BoundLabelStatement labelStatement)
                    labelToIndex.Add(labelStatement.Label, i + 1);
            }

            var index = 0;
            while (index < _root.Statements.Length)
            {
                var statement = _root.Statements[index];
                switch (statement.NodeType)
                {
                    case BoundNodeType.VARIABLE_DECLARATION_STATEMENT:
                        EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                        index++;
                        break;

                    case BoundNodeType.EXPRESSION_STATEMENT:
                        EvaluateExpressionStatement((BoundExpressionStatement)statement);
                        index++;
                        break;

                    case BoundNodeType.GOTO_STATEMENT:
                        var gotoStatement = (BoundGotoStatement)statement;
                        index = labelToIndex[gotoStatement.Label];
                        break;

                    case BoundNodeType.CONDITIONAL_GOTO_STATEMENT:
                        var conditionalGotoStatement = (BoundConditionalGotoStatement)statement;
                        var condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition);
                        if (condition && conditionalGotoStatement.JumpIfTrue ||
                            !condition && !conditionalGotoStatement.JumpIfTrue)
                            index = labelToIndex[conditionalGotoStatement.Label];
                        else
                            index++;
                        break;

                    case BoundNodeType.LABEL_STATEMENT:
                        index++;
                        break;

                    default:
                        throw new Exception($"Unexpected node {statement.NodeType}");
                }
            }

            return _lastValue;
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

        private object? EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                case BoundLiteralExpression literalExpression:
                    return EvaluateLiteralExpression(literalExpression);

                case BoundVariableExpression variableExpression:
                    return EvaluateVariableExpression(variableExpression);

                case BoundAssignmentExpression assignmentExpression:
                    return EvaluateAssignmentExpression(assignmentExpression);

                case BoundUnaryExpression unaryExpression:
                    return EvaluateUnaryExpression(unaryExpression);

                case BoundBinaryExpression binaryExpression:
                    return EvaluateBinaryExpression(binaryExpression);

                default:
                    throw new Exception($"Unexpected node {node.Type}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression literalExpression)
        {
            return literalExpression.Value;
        }

        private object? EvaluateVariableExpression(BoundVariableExpression variableExpression)
        {
            return _variables[variableExpression.Variable];
        }

        private object? EvaluateAssignmentExpression(BoundAssignmentExpression assignmentExpression)
        {
            var value = EvaluateExpression(assignmentExpression.Expression);
            _variables[assignmentExpression.Variable] = value;
            return value;
        }

        private object? EvaluateUnaryExpression(BoundUnaryExpression unaryExpression)
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

        private object? EvaluateBinaryExpression(BoundBinaryExpression binaryExpression)
        {
            var first = EvaluateExpression(binaryExpression.FirstOperand);
            var second = EvaluateExpression(binaryExpression.SecondOperand);

            switch (binaryExpression.Op.Type)
            {
                case BoundBinaryOperatorType.ADDITION:
                    if (first is string || second is string)
                        return first?.ToString() + second?.ToString();

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
