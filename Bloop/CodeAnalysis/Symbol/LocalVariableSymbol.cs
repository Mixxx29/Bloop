namespace Bloop.CodeAnalysis.Symbol
{
    public class LocalVariableSymbol : VariableSymbol
    {
        public LocalVariableSymbol(string name, bool isReadOnly, TypeSymbol type)
            : base(name, isReadOnly, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.LOCAL_VARIABLE;
    }
}
