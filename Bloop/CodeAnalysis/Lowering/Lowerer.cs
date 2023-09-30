using Bloop.CodeAnalysis.Binding;
using Bloop.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int _labelCount;

        private Lowerer()
        {
        }

        private LabelSymbol GenerateLabel()
        {
            var name = $"Label {++_labelCount}";
            return new LabelSymbol(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowering = new Lowerer();
            var result = lowering.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement node)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    foreach (var statement in block.Statements.Reverse()) 
                        stack.Push(statement);
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                // if <condition>
                //     <then>
                //
                // --->
                //
                // gotoIfFalse <condition> end
                // <then>
                // end:

                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoFalse,
                    node.ThenStatement,
                    endLabelStatement
                ));
                return RewriteStatement(result);
            }
            else
            {
                // if <condition>
                //     <then>
                // else
                //     <else>
                //
                // --->
                //
                // gotoIfFalse <condition> else
                // <then>
                // goto end
                // else:
                // <else>
                // end:

                var elseLabel = GenerateLabel();
                var endLabel = GenerateLabel();

                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, false);
                var gotoEnd = new BoundGotoStatement(endLabel);

                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);

                var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoFalse,
                    node.ThenStatement,
                    gotoEnd,
                    elseLabelStatement,
                    node.ElseStatement,
                    endLabelStatement
                ));
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            // while <condition>
            //     <body>
            //
            // --->
            // 
            // goto check
            // continue:
            // <body>
            // check:
            // gotoTrue <condition> continue
            // end:

            var continueLabel = GenerateLabel();
            var checkLabel = GenerateLabel();
            var endLabel = GenerateLabel();

            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var gotoCheck = new BoundGotoStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, true);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                    gotoCheck,
                    continueLabelStatement,
                    node.Statement,
                    checkLabelStatement,
                    gotoTrue,
                    endLabelStatement
                ));
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            // for <it> = <lowerBound> to <upperBound>
            //     <body>
            //
            // --->
            //
            //     var <it> = <lowerBound>
            //     while <lowerBound> < <upperBound>
            //     {
            //         <body>
            //         <it> = <it> + 1
            //     }

            var variableDeclaration = new BoundVariableDeclarationStatement(node.Variable, node.FirstBound);
            var variableExpression = new BoundVariableExpression(node.Variable);
            var condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxType.LESS_THAN_TOKEN, typeof(int), typeof(int)),
                node.SecondBound
            );

            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxType.PLUS_TOKEN, typeof(int), typeof(int)),
                        new BoundLiteralExpression(1)
                    )
                )
            );

            var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Statement, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBody);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));
            return RewriteStatement(result);
        }
    }
}
