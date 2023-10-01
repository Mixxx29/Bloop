using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Binding
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeType NodeType => BoundNodeType.GOTO_STATEMENT;

        public LabelSymbol Label { get; }
    }
}