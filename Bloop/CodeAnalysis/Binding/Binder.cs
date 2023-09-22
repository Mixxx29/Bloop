using Bloop.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        private BoundScope? _scope;

        public Binder(BoundScope? parent)
        {
            _scope = new BoundScope(parent);
        }

        public DiagnosticsPool Diagnostics => _diagnostics;

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var statement = binder.BindStatement(syntax.Statement);
            var variables = binder._scope != null ? binder._scope.GetDeclaredVariables() : ImmutableArray<VariableSymbol>.Empty;
            var diagnostics = binder._diagnostics.ToImmutableArray();
            return new BoundGlobalScope(previous, diagnostics, variables, statement);
        }

        private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in previous.Variables)
                    scope.TryDeclare(v);

                parent = scope;
            }

            return parent;
        }

        public BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Type)
            {
                case SyntaxType.BLOCK_STATEMENT:
                    return BindBlockStatement((BLockStatementSyntax)syntax);

                case SyntaxType.EXPRESSION_STATEMENT:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Type}");
            }
        }

        private BoundBlockStatement BindBlockStatement(BLockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        public BoundExpressionNode BindExpression(ExpressionSyntax expressionNode)
        {
            switch (expressionNode.Type)
            {
                case SyntaxType.LITERAL_EXPRESSION:
                    return BindLiteralExpressiom((LiteralExpressionNode) expressionNode);

                case SyntaxType.UNARY_EXPRESSION:
                    return BindUnaryExpressiom((UnaryExpressionSyntax) expressionNode);

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

        private BoundExpressionNode BindUnaryExpressiom(UnaryExpressionSyntax expressionNode)
        {
            var boundOperand = BindExpression(expressionNode.ExpressionSyntax);
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
            if (_scope == null) 
                return new BoundLiteralExpressionNode(0);

            var name = syntax.IdentifierToken.Text;
            if (!_scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedVariable(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpressionNode(0);
            }
            return new BoundVariableExpressionNode(variable);
        }

        private BoundExpressionNode BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookup(name, out var variable))
            {
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryDeclare(variable);
            }

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportInvalidConversion(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            return new BoundAssignmentExpressionNode(variable, boundExpression);
        }
    }
}
