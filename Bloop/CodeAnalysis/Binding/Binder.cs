using Bloop.CodeAnalysis.Symbol;
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

    public sealed class Binder
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

            BoundScope parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);

                parent = scope;
            }

            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);

            foreach (var function in BuiltinFunctions.GetAll())
                result.TryDeclareFunction(function);

            return result;  
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
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
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
            var firstBound = BindExpression(syntax.FirstBound, TypeSymbol.Number);
            var secondBound = BindExpression(syntax.SecondBound, TypeSymbol.Number);

            _scope = new BoundScope(_scope);

            var identifier = syntax.Identifier;
            var variable = BindVariable(identifier, false, firstBound);

            var statement = BindStatement(syntax.Statement);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, firstBound, secondBound, statement);
        }

        private BoundStatement BindVariableDeclarationStatement(VariableDeclarationStatement syntax)
        {
            var isReadOnly = syntax.Keyword.Type == SyntaxType.CONST_KEYWORD;
            var expression = BindExpression(syntax.Expression);
            var identifier = syntax.Identifier;
            VariableSymbol variable = BindVariable(identifier, isReadOnly, expression);

            return new BoundVariableDeclarationStatement(variable, expression);
        }

        private VariableSymbol BindVariable(SyntaxToken identifier, bool isReadOnly, BoundExpression expression)
        {
            var name = identifier.Text;
            if (name == "")
                name = "?";

            var variable = new VariableSymbol(name, isReadOnly, expression.Type);

            if (!_scope.TryDeclareVariable(variable))
                _diagnostics.ReportVariableAlreadyDeclared(identifier.Span, name);
            return variable;
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression, true);
            return new BoundExpressionStatement(expression);
        }

        public BoundExpression BindExpression(ExpressionSyntax expressionNode, TypeSymbol targetType)
        {
            var result = BindExpression(expressionNode);
            if (result.Type != TypeSymbol.Error &&
                targetType != TypeSymbol.Error &&
                result.Type != targetType)
            {
                _diagnostics.ReportInvalidConversion(expressionNode.Span, result.Type, targetType);
            }
            
            return result;
        }

        public BoundExpression BindExpression(ExpressionSyntax expressionNode, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(expressionNode);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportUnexpectedVoidExpression(expressionNode.Span);
                return new BoundErrorExpression();
            }

            return result;
        }

        public BoundExpression BindExpressionInternal(ExpressionSyntax expressionNode)
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

                case SyntaxType.FUNCTION_CALL_EXPRESSION:
                    return BindFunctionCallExpression((FunctionCallExpression)expressionNode);

                case SyntaxType.ASSIGNMENT_EXPRESSION:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)expressionNode);

                case SyntaxType.CONVERSION_EXPRESSION:
                    return BindConversionExpression((ConversionExpression)expressionNode);

                default:
                    throw new Exception($"Unexpected syntax {expressionNode.Type}");
            }
        }

        private BoundExpression BindLiteralExpressiom(LiteralExpressionNode expressionNode)
        {
            var value = expressionNode.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpressiom(UnaryExpressionSyntax expressionNode)
        {
            var boundOperand = BindExpression(expressionNode.ExpressionSyntax);

            if (boundOperand is BoundErrorExpression)
                return new BoundErrorExpression();

            var boundOperator = BoundUnaryOperator.Bind(expressionNode.OperatorToken.Type, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(
                    expressionNode.OperatorToken.Span, 
                    expressionNode.OperatorToken.Text, 
                    boundOperand.Type
                );
                return new BoundErrorExpression();
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpressiom(BinaryExpressionNode expressionNode)
        {
            var boundFirstOperand = BindExpression(expressionNode.FirstNode);
            var boundSecondOperand = BindExpression(expressionNode.SecondNode);

            if (boundFirstOperand.Type == TypeSymbol.Error ||
                boundSecondOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            var boundOperator = BoundBinaryOperator.Bind(expressionNode.OperatorToken.Type, boundFirstOperand.Type, boundSecondOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(expressionNode.OperatorToken.Span, expressionNode.OperatorToken.Text, boundFirstOperand.Type, boundSecondOperand.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(boundFirstOperand, boundOperator, boundSecondOperand);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            if (_scope == null)
                return new BoundErrorExpression();

            var name = syntax.IdentifierToken.Text;
            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedVariable(syntax.IdentifierToken.Span, name);
                return new BoundErrorExpression();
            }
            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindFunctionCallExpression(FunctionCallExpression expressionNode)
        {
            var arguments = ImmutableArray.CreateBuilder<BoundExpression>();
            foreach (var argument in expressionNode.Arguments)
            {
                arguments.Add(BindExpression(argument));
            }

            if (!_scope.TryLookupFunction(expressionNode.Identifier.Text, out var function))
            {
                _diagnostics.ReportUndefinedFunction(expressionNode.Identifier.Span, expressionNode.Identifier.Text);
                return new BoundErrorExpression();
            }

            if (function.Parameters.Length != arguments.Count)
            {
                _diagnostics.ReportInvalidArgumentCount(
                    expressionNode.Span,
                    expressionNode.Identifier.Text,
                    arguments.Count,
                    function.Parameters.Length
                );
                return new BoundErrorExpression();
            }

            for (var i = 0; i < expressionNode.Arguments.Count; i++)
            {
                var argument = arguments[i];
                var parameter = function.Parameters[i];

                if (argument.Type != parameter.Type)
                {
                    _diagnostics.ReportInvalidArgumentType(
                        expressionNode.Span,
                        parameter.Name,
                        argument.Type,
                        parameter.Type
                    );

                    return new BoundErrorExpression();
                }
            }

            return new BoundFunctionCallExpression(function, arguments.ToImmutable());
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookupVariable(name, out var variable))
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

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindConversionExpression(ConversionExpression expressionNode)
        {
            var expression = BindExpression(expressionNode.Expression);
            return new BoundConversionExpression(expression, expressionNode.TargetType);
        }
    }
}
