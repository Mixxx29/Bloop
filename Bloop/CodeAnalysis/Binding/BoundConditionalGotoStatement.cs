namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool jumpIfTrue = false)
        {
            Label = label;
            Condition = condition;
            JumpIfTrue = jumpIfTrue;
        }

        public override BoundNodeType NodeType => BoundNodeType.CONDITIONAL_GOTO_STATEMENT;

        public LabelSymbol Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfTrue { get; }
    }
}