﻿namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression firstOperandNode, BoundBinaryOperator op, BoundExpression secondOperandNode)
        {
            FirstOperandNode = firstOperandNode;
            Op = op;
            SecondOperandNode = secondOperandNode;
        }

        public override BoundNodeType NodeType => BoundNodeType.BINARY_EXPRESSION;
        public override Type Type => Op.ResultType;

        public BoundExpression FirstOperandNode { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression SecondOperandNode { get; }
    }
}