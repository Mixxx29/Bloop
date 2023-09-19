using Bloop.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class Binder
    { 
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public BoundExpressionNode BindExpression(ExpressionNode expressionNode)
        {
            switch (expressionNode.Type)
            {
                case SyntaxType.LITERAL_EXPRESSION:
                    return BindLiteralExpressiom((LiteralExpressionNode) expressionNode);

                case SyntaxType.UNARY_EXPRESSION:
                    return BindUnaryExpressiom((UnaryExpressionNode) expressionNode);

                case SyntaxType.BINARY_EXPRESSION:
                    return BindBinaryExpressiom((BinaryExpressionNode)expressionNode);

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
                _diagnostics.Add($"ERROR: Unary operator '{expressionNode.OperatorToken.Text}' is not defined for type {boundOperand.Type}");
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
                _diagnostics.Add($"ERROR: Binary operator '{expressionNode.OperatorToken.Text}' is not defined for types {boundFirstOperand.Type} and {boundSecondOperand.Type}");
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
    }
}
