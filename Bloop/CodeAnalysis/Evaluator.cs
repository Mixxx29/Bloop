using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Symbol;
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace Bloop.CodeAnalysis
{

    public class Evaluator
    {
        private object? _lastValue;

        public delegate void PrintHandler(string text);
        public event PrintHandler? OnPrint;

        public delegate string ReadHandler();
        public event ReadHandler? OnRead;

        public void Subscribe(CompilationSubscriber subscriber)
        {
            OnPrint += subscriber.OnPrint;
            OnRead+= subscriber.OnRead;
        }

        internal object? Evaluate(BoundBlockStatement root, Dictionary<VariableSymbol, object?> variables)
        {
            var labelToIndex = new Dictionary<LabelSymbol, int>();

            for (var i = 0; i < root.Statements.Length; i++)
            {
                if (root.Statements[i] is BoundLabelStatement labelStatement)
                    labelToIndex.Add(labelStatement.Label, i + 1);
            }

            var index = 0;
            while (index < root.Statements.Length)
            {
                var statement = root.Statements[index];
                switch (statement.NodeType)
                {
                    case BoundNodeType.VARIABLE_DECLARATION_STATEMENT:
                        EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement, variables);
                        index++;
                        break;

                    case BoundNodeType.EXPRESSION_STATEMENT:
                        EvaluateExpressionStatement((BoundExpressionStatement)statement, variables);
                        index++;
                        break;

                    case BoundNodeType.GOTO_STATEMENT:
                        var gotoStatement = (BoundGotoStatement)statement;
                        index = labelToIndex[gotoStatement.Label];
                        break;

                    case BoundNodeType.CONDITIONAL_GOTO_STATEMENT:
                        var conditionalGotoStatement = (BoundConditionalGotoStatement)statement;
                        var condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition, variables);
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

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement variableDeclarationStatement, Dictionary<VariableSymbol, object?> variables)
        {
            var value = EvaluateExpression(variableDeclarationStatement.Expression, variables);
            variables[variableDeclarationStatement.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement expressionStatement, Dictionary<VariableSymbol, object?> variables)
        {
            _lastValue = EvaluateExpression(expressionStatement.Expression, variables);
        }

        private object? EvaluateExpression(BoundExpression node, Dictionary<VariableSymbol, object?> variables)
        {
            switch (node)
            {
                case BoundLiteralExpression literalExpression:
                    return EvaluateLiteralExpression(literalExpression);

                case BoundVariableExpression variableExpression:
                    return EvaluateVariableExpression(variableExpression, variables);

                case BoundAssignmentExpression assignmentExpression:
                    return EvaluateAssignmentExpression(assignmentExpression, variables);

                case BoundUnaryExpression unaryExpression:
                    return EvaluateUnaryExpression(unaryExpression, variables);

                case BoundBinaryExpression binaryExpression:
                    return EvaluateBinaryExpression(binaryExpression, variables);

                case BoundFunctionCallExpression functionCallExpression:
                    return EvaluateFunctionCallExpression(functionCallExpression, variables);

                default:
                    throw new Exception($"Unexpected node {node.Type}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression literalExpression)
        {
            return literalExpression.Value;
        }

        private object? EvaluateVariableExpression(BoundVariableExpression variableExpression, Dictionary<VariableSymbol, object?> variables)
        {
            return variables[variableExpression.Variable];
        }

        private object? EvaluateAssignmentExpression(BoundAssignmentExpression assignmentExpression, Dictionary<VariableSymbol, object?> variables)
        {
            var value = EvaluateExpression(assignmentExpression.Expression, variables);
            variables[assignmentExpression.Variable] = value;
            return value;
        }

        private object? EvaluateUnaryExpression(BoundUnaryExpression unaryExpression, Dictionary<VariableSymbol, object?> variables)
        {
            var operand = EvaluateExpression(unaryExpression.Operand, variables);

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

        private object? EvaluateBinaryExpression(BoundBinaryExpression binaryExpression, Dictionary<VariableSymbol, object?> variables)
        {
            var first = EvaluateExpression(binaryExpression.FirstOperand, variables);
            var second = EvaluateExpression(binaryExpression.SecondOperand, variables);

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

        private object? EvaluateFunctionCallExpression(BoundFunctionCallExpression functionCallExpression, Dictionary<VariableSymbol, object?> variables)
        {
            if (functionCallExpression.Function == BuiltinFunctions.Print)
            {
                var value = (string?)EvaluateExpression(functionCallExpression.Arguments[0], variables);
                if (value != null)
                    OnPrint?.Invoke(value);

                return null;
            }
            else if (functionCallExpression.Function == BuiltinFunctions.Read)
            {
                return OnRead?.Invoke();
            }
            else if (functionCallExpression.Function == BuiltinFunctions.ParseInt)
            {
                var value = (string?)EvaluateExpression(functionCallExpression.Arguments[0], variables);
                if (value != null)
                    try
                    {
                        return int.Parse(value);
                    }
                    catch (Exception ex)
                    {
                    }

                return null;
            }

            throw new Exception("Invalid function");
        }
    }
}
