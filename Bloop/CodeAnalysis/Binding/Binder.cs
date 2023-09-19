using Bloop.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class Binder
    { 
        private readonly DiagnosticsPool _diagnostics = new DiagnosticsPool();
        private readonly Dictionary<VariableSymbol, object?> _variables;

        public Binder(Dictionary<VariableSymbol, object> variables)
        {
            _variables = variables;
        }

        public DiagnosticsPool Diagnostics => _diagnostics;

        public BoundExpressionNode BindExpression(ExpressionSyntax expressionNode)
        {
            switch (expressionNode.Type)
            {
                case SyntaxType.LITERAL_EXPRESSION:
                    return BindLiteralExpressiom((LiteralExpressionNode) expressionNode);

                case SyntaxType.UNARY_EXPRESSION:
                    return BindUnaryExpressiom((UnaryExpressionNode) expressionNode);

                case SyntaxType.BINARY_EXPRESSION:
                    return BindBinaryExpressiom((BinaryExpressionNode)expressionNode);

                case SyntaxType.PARENTHESIZED_EXPRESSION:
                    return BindExpression(((ParenthesizedExpressionNode)expressionNode).ExpressionNode);

                case SyntaxType.NAME_EXPRESSION:
                    return BindNameExpression((NameExpressionSyntax)expressionNode);

                case SyntaxType.ASSIGNMENT_EXPRESSION:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)expressionNode);

                default:
                    throw new Exception($"Unexpected syntax {expressionNode.Type}");
            }
        }

        private BoundExpressionNode BindLiteralExpressiom(LiteralExpressionNode expressionNode)
        {
            var value = expressionNode.Value ?? 0;
            return new BoundLiteralExpressionNode(value);
        }

        private BoundExpressionNode BindUnaryExpressiom(UnaryExpressionNode expressionNode)
        {
            var boundOperand = BindExpression(expressionNode.OperandNode);
            var boundOperator = BoundUnaryOperator.Bind(expressionNode.OperatorToken.Type, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(expressionNode.OperatorToken.Span, expressionNode.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpressionNode(boundOperator, boundOperand);
        }

        private BoundUnaryOperatorType? BindUnaryOperatorKind(SyntaxType type, Type operandType)
        {
            if (operandType == typeof(int))
            {
                switch (type)
                {
                    case SyntaxType.PLUS_TOKEN:
                        return BoundUnaryOperatorType.IDENTITY;

                    case SyntaxType.MINUS_TOKEN:
                        return BoundUnaryOperatorType.NEGATION;

                    default:
                        throw new Exception($"Unexpected unary operator {type}");
                }
            }
            else if (operandType == typeof(bool))
            {
                switch (type)
                {
                    case SyntaxType.EXCLAMATION_MARK_TOKEN:
                        return BoundUnaryOperatorType.LOGIC_NEGATION;

                    default:
                        throw new Exception($"Unexpected unary operator {type}");
                }
            }

            return null;
        }

        private BoundExpressionNode BindBinaryExpressiom(BinaryExpressionNode expressionNode)
        {
            var boundFirstOperand = BindExpression(expressionNode.FirstNode);
            var boundSecondOperand = BindExpression(expressionNode.SecondNode);
            var boundOperator = BoundBinaryOperator.Bind(expressionNode.OperatorToken.Type, boundFirstOperand.Type, boundSecondOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(expressionNode.OperatorToken.Span, expressionNode.OperatorToken.Text, boundFirstOperand.Type, boundSecondOperand.Type);
                return boundFirstOperand;
            }
            return new BoundBinaryExpressionNode(boundFirstOperand, boundOperator, boundSecondOperand);
        }

        private BoundBinaryOperatorType? BindBinaryOperatorType(SyntaxType type, Type firstOperandType, Type secondOperandType)
        {
            if (firstOperandType == typeof(int) && secondOperandType == typeof(int))
            {
                switch (type)
                {
                    case SyntaxType.PLUS_TOKEN:
                        return BoundBinaryOperatorType.ADDITION;

                    case SyntaxType.MINUS_TOKEN:
                        return BoundBinaryOperatorType.SUBSTRACTION;

                    case SyntaxType.ASTERIX_TOKEN:
                        return BoundBinaryOperatorType.MULTIPLICATION;

                    case SyntaxType.SLASH_TOKEN:
                        return BoundBinaryOperatorType.DIVISION;

                    default:
                        throw new Exception($"Unexpected binary operator {type}");
                }
            }
            else if (firstOperandType == typeof(bool) && secondOperandType == typeof(bool))
            {
                switch (type)
                {
                    case SyntaxType.DOUBLE_AMPERSAND_TOKEN:
                        return BoundBinaryOperatorType.LOGIC_AND;

                    case SyntaxType.DOUBLE_PIPE_TOKEN:
                        return BoundBinaryOperatorType.LOGIC_OR;

                    default:
                        throw new Exception($"Unexpected binary operator {type}");
                }
            }

            return null;
        }

        private BoundExpressionNode BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var variable = _variables.Keys.FirstOrDefault(v => v.Name == name);

            if (variable == null)
            {
                _diagnostics.ReportUndefinedIdentifier(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpressionNode(0);
            }
            return new BoundVariableExpressionNode(variable);
        }

        private BoundExpressionNode BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);
            if (existingVariable != null)
                _variables.Remove(existingVariable);

            var variable = new VariableSymbol(name, boundExpression.Type);
            _variables[variable] = null;

            return new BoundAssignmentExpressionNode(variable, boundExpression);
        }
    }
}
