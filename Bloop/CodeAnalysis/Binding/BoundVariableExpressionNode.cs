﻿using Bloop.CodeAnalysis.Syntax;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpressionNode : BoundExpressionNode
    {
        public BoundVariableExpressionNode(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeType NodeType => BoundNodeType.VARIABLE_EXPRESSION;

        public VariableSymbol Variable { get; }
        public string Name => Variable.Name;
        public override Type Type => Variable.Type;
    }
}