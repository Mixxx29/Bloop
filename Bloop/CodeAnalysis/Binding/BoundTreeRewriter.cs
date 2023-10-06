using Bloop.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bloop.CodeAnalysis.Binding
{
    internal abstract class BoundTreeRewriter
    {
        protected virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.NodeType)
            {
                case BoundNodeType.MAIN_STATEMENT:
                    return RewriteMainStatement((BoundMainStatement)node);

                case BoundNodeType.BLOCK_STATEMENT:
                    return RewriteBlockStatement((BoundBlockStatement)node);

                case BoundNodeType.EXPRESSION_STATEMENT:
                    return RewriteExpressionStatement((BoundExpressionStatement)node);

                case BoundNodeType.VARIABLE_DECLARATION_STATEMENT:
                    return RewriteVariableDeclarationStatement((BoundVariableDeclarationStatement)node);

                case BoundNodeType.IF_STATEMENT:
                    return RewriteIfStatement((BoundIfStatement)node);

                case BoundNodeType.ELSE_STATEMENT:
                    return RewriteElseStatement((BoundElseStatement)node);

                case BoundNodeType.WHILE_STATEMENT:
                    return RewriteWhileStatement((BoundWhileStatement)node);

                case BoundNodeType.FOR_STATEMENT:
                    return RewriteForStatement((BoundForStatement)node);

                case BoundNodeType.GOTO_STATEMENT:
                    return RewriteGotoStatement((BoundGotoStatement)node);

                case BoundNodeType.LABEL_STATEMENT:
                    return RewriteLabelStatement((BoundLabelStatement)node);

                case BoundNodeType.CONDITIONAL_GOTO_STATEMENT:
                    return RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node);

                default:
                    throw new Exception($"Unexpected node {node.NodeType}");
            }
        }

        protected virtual BoundStatement RewriteMainStatement(BoundMainStatement node)
        {
            ImmutableArray<BoundStatement>.Builder? builder = null;

            for (var i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(node.Statements[i]);
                if (newStatement != oldStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);

                        for (var j = 0; j < i; j++)
                            builder.Add(node.Statements[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newStatement);
            }

            if (builder == null)
                return new BoundBlockStatement(node.Statements);

            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder? builder = null;

            for (var i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(node.Statements[i]);
                if (newStatement != oldStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);
                        
                        for (var j = 0; j < i; j++)
                            builder.Add(node.Statements[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newStatement);
            }

            if (builder == null)
                return node;

            return new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteVariableDeclarationStatement(BoundVariableDeclarationStatement node)
        {
            var expression = node.Expression;
            if (expression == node.Expression)
                return node;

            return new BoundVariableDeclarationStatement(node.Variable, expression);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var thenStatement = RewriteStatement(node.ThenStatement);
            var elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);
            if (condition == node.Condition && thenStatement == node.ThenStatement && elseStatement == node.ElseStatement)  
                return node;

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        protected virtual BoundStatement RewriteElseStatement(BoundElseStatement node)
        {
            var statement = RewriteStatement(node.Statement);
            if (statement == node.Statement)
                return node;

            return new BoundElseStatement(statement);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var statement = RewriteStatement(node.Statement);
            if (condition == node.Condition && statement == node.Statement)
                return node;

            return new BoundWhileStatement(condition, statement);
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var firstBound = RewriteExpression(node.FirstBound);
            var secondBound = RewriteExpression(node.SecondBound);
            var statement = RewriteStatement(node.Statement);
            if (firstBound == node.FirstBound && secondBound == node.SecondBound && statement == node.Statement)
                return node;

            return new BoundForStatement(node.Variable, firstBound, secondBound, statement);
        }

        private BoundStatement RewriteGotoStatement(BoundGotoStatement node)
        {
            return node;
        }

        private BoundStatement RewriteLabelStatement(BoundLabelStatement node)
        {
            return node;
        }

        private BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            if (condition == node.Condition)
                return node;

            return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfTrue);
        }

        protected virtual BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.NodeType)
            {
                case BoundNodeType.ERROR_EXPRESSION:
                    return RewriteErrorExpression((BoundErrorExpression)node);

                case BoundNodeType.LITERAL_EXPRESSION:
                    return RewriteLiteralExpression((BoundLiteralExpression)node);

                case BoundNodeType.UNARY_EXPRESSION:
                    return RewriteUnaryExpression((BoundUnaryExpression)node);

                case BoundNodeType.BINARY_EXPRESSION:
                    return RewriteBinaryExpression((BoundBinaryExpression)node);

                case BoundNodeType.VARIABLE_EXPRESSION:
                    return RewriteVariableExpression((BoundVariableExpression)node);

                case BoundNodeType.ASSIGNMENT_EXPRESSION:
                    return RewriteAssignmentExpression((BoundAssignmentExpression)node);

                case BoundNodeType.CONVERSION_EXPRESSION:
                    return RewriteConversionExpression((BoundConversionExpression)node);

                case BoundNodeType.FUNCTION_CALL_EXPRESSION:
                    return RewriteFunctionCallExpression((BoundFunctionCallExpression)node);

                default:
                    throw new Exception($"Unexpected node {node.NodeType}");
            }
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var operand = RewriteExpression(node.Operand);
            if (operand == node.Operand)
                return node;

            return new BoundUnaryExpression(node.Op, operand);
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var firstOperand = RewriteExpression(node.FirstOperand);
            var secondOperand = RewriteExpression(node.SecondOperand);
            if (firstOperand == node.FirstOperand && secondOperand == node.SecondOperand)
                return node;

            return new BoundBinaryExpression(firstOperand, node.Op, secondOperand);
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundAssignmentExpression(node.Variable, expression);
        }

        private BoundExpression RewriteConversionExpression(BoundConversionExpression node)
        {
            var expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new BoundConversionExpression(expression, node.Type);
        }

        private BoundExpression RewriteFunctionCallExpression(BoundFunctionCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder? builder = null;

            for (var i = 0; i < node.Arguments.Length; i++)
            {
                var oldArgument = node.Arguments[i];
                var newArgument = RewriteExpression(node.Arguments[i]);
                if (newArgument != oldArgument)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);

                        for (var j = 0; j < i; j++)
                            builder.Add(node.Arguments[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newArgument);
            }

            if (builder == null)
                return node;

            return new BoundFunctionCallExpression(node.Function, builder.MoveToImmutable());
        }
    }
}
