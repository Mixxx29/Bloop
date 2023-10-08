using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Symbol;
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace Bloop.CodeAnalysis
{

    public class Evaluator
    {
        private Dictionary<FunctionSymbol, BoundBlockStatement> _functionBodies;
        private BoundBlockStatement _root;
        private Dictionary<VariableSymbol, object?> _global;
        private Stack<Dictionary<VariableSymbol, object?>> _locals;

        public delegate void PrintHandler(string text);
        public event PrintHandler? OnPrint;

        public delegate string ReadHandler();
        public event ReadHandler? OnRead;

        public void Subscribe(CompilationSubscriber subscriber)
        {
            OnPrint += subscriber.OnPrint;
            OnRead+= subscriber.OnRead;
        }

        internal void Evaluate(Dictionary<FunctionSymbol, BoundBlockStatement> functionBodies, BoundBlockStatement root, Dictionary<VariableSymbol, object?> variables)
        {
            _functionBodies = functionBodies;
            _root = root;
            _global = variables;

            _locals = new Stack<Dictionary<VariableSymbol, object?>>();

            var body = _root;
            EvaluateStatement(body);
        }

        private object? EvaluateStatement(BoundBlockStatement body)
        {
            var labelToIndex = new Dictionary<LabelSymbol, int>();

            for (var i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i] is BoundLabelStatement labelStatement)
                    labelToIndex.Add(labelStatement.Label, i + 1);
            }

            var index = 0;
            while (index < body.Statements.Length)
            {
                var statement = body.Statements[index];
                switch (statement.NodeType)
                {
                    case BoundNodeType.VARIABLE_DECLARATION_STATEMENT:
                        EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                        index++;
                        break;

                    case BoundNodeType.EXPRESSION_STATEMENT:
                        var result = EvaluateExpressionStatement((BoundExpressionStatement)statement);
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

            return null;
        }

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement variableDeclarationStatement)
        {
            var value = EvaluateExpression(variableDeclarationStatement.Expression);

            if (variableDeclarationStatement.Variable.SymbolType == SymbolType.GLOBAL_VARIABLE)
            {
                _global[variableDeclarationStatement.Variable] = value;
            }
            else
            {
                var local = _locals.Peek();
                local[variableDeclarationStatement.Variable] = value;
            }
        }

        private object? EvaluateExpressionStatement(BoundExpressionStatement expressionStatement)
        {
            return EvaluateExpression(expressionStatement.Expression);
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

                case BoundConversionExpression castExpression:
                    return EvaluateCastExpression(castExpression);

                case BoundFunctionCallExpression functionCallExpression:
                    return EvaluateFunctionCallExpression(functionCallExpression);

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
            if (variableExpression.Variable.SymbolType == SymbolType.GLOBAL_VARIABLE)
            {
                return _global[variableExpression.Variable];
            }

            var local = _locals.Peek();
            return local[variableExpression.Variable];
        }

        private object? EvaluateAssignmentExpression(BoundAssignmentExpression assignmentExpression)
        {
            var value = EvaluateExpression(assignmentExpression.Expression);

            if (assignmentExpression.Variable.SymbolType == SymbolType.GLOBAL_VARIABLE)
            {
                _global[assignmentExpression.Variable] = value;
            }
            else
            {
                var local = _locals.Peek();
                local[assignmentExpression.Variable] = value;
            }

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

        private object? EvaluateCastExpression(BoundConversionExpression castExpression)
        {
            var result = EvaluateExpression(castExpression.Expression);
            try
            {
                if (castExpression.TargetType == TypeSymbol.Number)
                    return Convert.ToInt32(result);

                if (castExpression.TargetType == TypeSymbol.String)
                    return Convert.ToString(result);

                if (castExpression.TargetType == TypeSymbol.Bool)
                    return Convert.ToBoolean(result);

                throw new Exception($"Invalid type '{castExpression.TargetType}'");
            }
            catch (InvalidCastException e)
            {
                
            }

            return null;
        }

        private object? EvaluateFunctionCallExpression(BoundFunctionCallExpression functionCallExpression)
        {
            if (functionCallExpression.Function == BuiltinFunctions.Print)
            {
                var value = (string?)EvaluateExpression(functionCallExpression.Arguments[0]);
                if (value != null)
                    OnPrint?.Invoke(value);

                return null;
            }

            if (functionCallExpression.Function == BuiltinFunctions.Read)
            {
                return OnRead?.Invoke();
            }

            if (functionCallExpression.Function == BuiltinFunctions.ParseInt)
            {
                var value = (string?)EvaluateExpression(functionCallExpression.Arguments[0]);
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

            var local = new Dictionary<VariableSymbol, object?>();
            for (var i = 0; i < functionCallExpression.Arguments.Length; i++ )
            {
                var parameter = functionCallExpression.Function.Parameters[i];
                var value = EvaluateExpression(functionCallExpression.Arguments[i]);
                local.Add(parameter, value);
            }

            _locals.Push(local);

            var statement = _functionBodies[functionCallExpression.Function];
            var result = EvaluateStatement(statement);

            _locals.Pop();

            return result;
        }
    }
}
