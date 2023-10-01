namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class LabelSymbol : Symbol
    {
        internal LabelSymbol(string name)
            : base(name)
        {
        }

        public override SymbolType SymbolType => SymbolType.LABEL;
    }
}
