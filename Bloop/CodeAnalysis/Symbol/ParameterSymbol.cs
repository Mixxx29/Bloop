namespace Bloop.CodeAnalysis.Symbol
{
    public sealed class ParameterSymbol : LocalVariableSymbol
    {
        public ParameterSymbol(string name, TypeSymbol type)
            : base(name, true, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.PARAMETER;
    }
}
