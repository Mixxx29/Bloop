﻿namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundVariableDeclarationStatement : BoundStatement
    {
        public BoundVariableDeclarationStatement(VariableSymbol variable, BoundExpressionNode expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override BoundNodeType NodeType => BoundNodeType.VARIABLE_DECLARATION_STATEMENT;

        public VariableSymbol Variable { get; }
        public BoundExpressionNode Expression { get; }
    }
}