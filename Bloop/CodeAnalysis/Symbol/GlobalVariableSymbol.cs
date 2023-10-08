namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class GlobalVariableSymbol : VariableSymbol
    {
        public GlobalVariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name, isReadOnly, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.GLOBAL_VARIABLE;
    }
}
