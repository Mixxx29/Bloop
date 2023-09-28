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
                case SyntaxType.MAIN_STATEMENT:
                    return BindMainStatement((MainStatementSyntax)syntax);

                case SyntaxType.BLOCK_STATEMENT:
                    return BindBlockStatement((BlockStatementSyntax)syntax);

                case SyntaxType.VARIABLE_DECLARATION_STATEMENT:
                    return BindVariableDeclarationStatement((VariableDeclarationStatement)syntax);

                case SyntaxType.EXPRESSION_STATEMENT:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);

                case SyntaxType.IF_STATEMENT:
                    return BindIfStatement((IfStatementSyntax)syntax);

                case SyntaxType.ELSE_STATEMENT:
                    return BindElseStatement((ElseStatementSyntax)syntax);

                case SyntaxType.WHILE_STATEMENT:
                    return BindWhileStatement((WhileStatementSyntax)syntax);

                case SyntaxType.FOR_STATEMENT:
                    return BindForStatement((ForStatementSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Type}");
            }
        }

        private BoundMainStatement BindMainStatement(MainStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            return new BoundMainStatement(statements.ToImmutable());
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();

            _scope = new BoundScope(_scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, typeof(bool));
            var thenStatement = BindStatement(syntax.Statement);
            var elseStatement = syntax.ElseStatement == null ? null : BindStatement(syntax.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        private BoundStatement BindElseStatement(ElseStatementSyntax syntax)
        {
            var statement = BindStatement(syntax.Statement);
            return new BoundElseStatement(statement);
        }

        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition);
            var statement = BindStatement(syntax.Statement);
            return new BoundWhileStatement(condition, statement);
        }

        private BoundForStatement BindForStatement(ForStatementSyntax syntax)
        {
            var firstBound = BindExpression(syntax.FirstBound, typeof(int));
            var secondBound = BindExpression(syntax.SecondBound, typeof(int));

            _scope = new BoundScope(_scope);

            var name = syntax.Identifier.Text;
            var variable = new VariableSymbol(name, false, typeof(int));
            _scope.TryDeclare(variable);

            var statement = BindStatement(syntax.Statement);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, firstBound, secondBound, statement);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedVariable(syntax.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.IsReadOnly)
                _diagnostics.ReportReadOnly(syntax.IdentifierToken.Span, name);

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportInvalidConversion(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            return new BoundAssignmentExpressionNode(variable, boundExpression);
        }

        private BoundStatement BindVariableDeclarationStatement(VariableDeclarationStatement syntax)
        {
            var name = syntax.Identifier.Text;
            var isReadOnly = syntax.Keyword.Type == SyntaxType.CONST_KEYWORD;
            var expression = BindExpression(syntax.Expression);
            var variable = new VariableSymbol(name, isReadOnly, expression.Type);

            if (!_scope.TryDeclare(variable))
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

            return new BoundVariableDeclarationStatement(variable, expression);
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        public BoundExpression BindExpression(ExpressionSyntax expressionNode, Type targetType)
        {
            var result = BindExpression(expressionNode);
            if (result.Type != targetType)
                _diagnostics.ReportInvalidConversion(expressionNode.Span, result.Type, targetType);
            
            return result;
        }

        public BoundExpression BindExpression(ExpressionSyntax expressionNode)
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

                case SyntaxType.MISSING_EXPRESSION:
                    return BindMissingExpression((MissingExpressionSyntax)expressionNode);

                default:
                    throw new Exception($"Unexpected syntax {expressionNode.Type}");
            }
        }

        private BoundExpression BindLiteralExpressiom(LiteralExpressionNode expressionNode)
        {
            var value = expressionNode.Value ?? 0;
            return new BoundLiteralExpressionNode(value);
        }

        private BoundExpression BindUnaryExpressiom(UnaryExpressionSyntax expressionNode)
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

        private BoundExpression BindBinaryExpressiom(BinaryExpressionNode expressionNode)
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

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
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

        private BoundExpression BindMissingExpression(MissingExpressionSyntax syntax)
        {
            _diagnostics.ReportMissingExpression(syntax.EndOfFileToken.Span);
            return new BoundMissingExpression();
        }
    }
}
