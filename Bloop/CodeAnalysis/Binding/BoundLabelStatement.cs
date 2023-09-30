namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeType NodeType => BoundNodeType.LABEL_STATEMENT;

        public LabelSymbol Label { get; }
    }
}